using UnityEngine;
using UnityEngine.Events;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIMassiveScrollView : MonoBehaviour 
{
    #region Members : Inspector
    [Header("Basic parameters")]
    [SerializeField][SelectOnChildren] protected UIScrollView       m_pScrView              = null; 
	[SerializeField]                   protected GameObject		    m_pEmptyView            = null; // 아이템 개수가 0개일 때 표시되는 GameObject
	[SerializeField][SelectOnChildren] protected UIGrid 			m_pGrid                 = null;
    [SerializeField]                   private   UISprite           m_pSpriteArea           = null; // 아이템이 보여질 영역
    
    [Header("Option parameters")]
    [SerializeField]                   protected int                m_iMaxColumn            = 4;    // 한 화면에 표시되는 최대 열 수
    [SerializeField]                   protected int                m_iMaxRow               = 6;    // 한 화면에 표시되는 최대 행 수
    [SerializeField]                   protected int                m_iAreaSize             = 0;    // 최소한으로 차지되는 길이(또는 너비)
    [SerializeField]                   private int                  m_iStartCache           = 0;    // 앞쪽에 배치되는 빈공간(아이템 개수)
    [SerializeField]                   protected int                m_iOffsetStart          = 0;    // 앞쪽에 배치되는 빈공간(아이템 라인 개수)
    [SerializeField]                   private int                  m_iOffsetEnd            = 0;    // 뒷쪽에 배치되는 빈공간(아이템 라인 개수)
    [SerializeField]                   private bool                 m_bEmptyCheckOnEnable   = true; // m_pEmptyView를 위한 Check를 OnEnable 함수에서 하는지에 대한 Flag
    [SerializeField]                   private bool                 m_bAreaSizeLock         = false;// minSize보다 적은 개수의 아이템일 때 센터정렬에 DragScrollView를 막음

    [Header("Sample Slot")]
    [SerializeField]                   protected GameObject         m_pSample               = null;
    #endregion


    #region Members : Info
    protected Dictionary<int, GameObject>   m_dicSlots          = new Dictionary<int, GameObject>();
    private Queue<GameObject>               m_pSpareSlots       = new Queue<GameObject>();
    private Action<GameObject, int>         m_pEventForSetData   = null;

    private int     m_iTotalCount           = -1;
    private int     m_iReservedCount        = -1;
    private int     m_iReservedIndex        = -1;
    private bool    m_bReservedWithOffset   = true;
    private bool    m_bReservedLastCheck    = true;
    private bool    m_bInitialized          = false;
    private bool    m_bIsDragging           = false;
    private bool    m_bIsDirty              = false;
    private bool    m_bMinSizeLockActivated = false;
    private Vector3 m_vGridOriginalPos      = Vector3.zero;
    #endregion


    #region System Functions
    void Awake()
    {
        if (null != m_pScrView)
        {
            if ((UIScrollView.Movement.Vertical != m_pScrView.movement) &&
                (UIScrollView.Movement.Horizontal != m_pScrView.movement))
            {
                Debug.LogWarning("[SHUIMassiveScrollView] not supported type of UIScrollView.");
            }
        }

        if (null != m_pSample)
            m_pSample.SetActive(false);

        m_vGridOriginalPos = m_pGrid.transform.localPosition;
    }

    IEnumerator Start()
    {
        if (false == m_bInitialized)
        {
            m_pScrView.onDragStarted    += DragStart;
            m_pScrView.onDragFinished   += DragEnd;
            m_pScrView.onMomentumMove   += OnDragging;
            m_pScrView.onStoppedMoving  += DragEnd;

            if (null != m_pScrView.horizontalScrollBar)
            {
                EventDelegate.Add(m_pScrView.horizontalScrollBar.onChange, OnScrollBar);
            }
            if (null != m_pScrView.verticalScrollBar)
            {
                EventDelegate.Add(m_pScrView.verticalScrollBar.onChange, OnScrollBar);
            }

            MakeSpareSlots();

            m_bInitialized = true;

            OnInitialized();
        }

        m_bIsDragging = false;

        //	NGUI의 UIGrid Component의 Start함수 이후에 호출되어야 하는 Needs가 있어서 한 프레임의 딜레이를 추가함..
        //	상세 설명 : _offset_Start의 값을 적용하여 각 아이템들을 배치시켜도 
        //  NGUI의 UIGrid Component의 Start에서 내부적으로 아이템들을 다시 배치시켜 m_iOffsetStart 값이 무시되는 이슈가 발생..
        yield return null;

        bool bIsNeedToPaint = true;

        if (0 < m_iReservedCount)
        {
            SetSlotCount(m_iReservedCount);

            m_iReservedCount = -1;
            bIsNeedToPaint = false;
        }

        if (0 <= m_iReservedIndex)
        {
            SetFocus(m_iReservedIndex, m_bReservedWithOffset, m_bReservedLastCheck);

            m_iReservedIndex = -1;
            m_bReservedWithOffset = true;
            m_bReservedLastCheck = true;
        }

        if (true == bIsNeedToPaint)
        {
            Paint();
        }
    }

    public virtual void OnDestroy()
    {
        if (true == m_bInitialized)
        {
            m_bInitialized = false;

            ClearSpareSlots();

            m_pScrView.onDragStarted    -= DragStart;
            m_pScrView.onDragFinished   -= DragEnd;
            m_pScrView.onMomentumMove   -= OnDragging;
            m_pScrView.onStoppedMoving  -= DragEnd;
        }
    }

    public virtual void OnEnable()
    {
        m_bIsDragging = false;

        if (m_bEmptyCheckOnEnable == true && null != m_pEmptyView)
        {
            m_pEmptyView.SetActive(m_iTotalCount <= 0);
        }

        if (null != m_pScrView && null != m_pScrView.verticalScrollBar)
        {
            m_pScrView.verticalScrollBar.gameObject.SetActive(m_iTotalCount > 0);
        }

        if (null != m_pScrView && null != m_pScrView.horizontalScrollBar)
        {
            m_pScrView.horizontalScrollBar.gameObject.SetActive(m_iTotalCount > 0);
        }
    }

    void OnReturn()
    {
        if (m_bInitialized == true)
        {
            m_bInitialized = false;

            ClearSpareSlots();

            m_pScrView.onDragStarted    -= DragStart;
            m_pScrView.onDragFinished   -= DragEnd;
            m_pScrView.onMomentumMove   -= OnDragging;
            m_pScrView.onStoppedMoving  -= DragEnd;
        }
    }
    
    protected virtual void Update()
    {
        if (false == m_bIsDragging)
            return;

        Paint();
    }
    #endregion


    #region Virtual Functions
    protected virtual void OnInitialized() { }
    protected virtual void SetSlotData(GameObject pSlot, int iIndex) { }
    #endregion


    #region Interface Functions
    public void SetSlotCount(int count)
    {
        if ((null == m_pScrView) || (null == m_pScrView.panel) || (null == m_pGrid))
        {
            m_iReservedCount = count;
            return;
        }

        m_bIsDirty = false;

        foreach (var pSlot in m_dicSlots.Values)
        {
            PushSlot(pSlot);
        }

        m_dicSlots.Clear();

        switch (m_pScrView.movement)
        {
            case UIScrollView.Movement.Horizontal:
                SetSlotCount_Horizontal(count);
                break;

            case UIScrollView.Movement.Vertical:
                SetSlotCount_Vertical(count);
                break;
        }

        if (null != m_pEmptyView)
        {
            m_pEmptyView.SetActive(count <= 0);
        }

        if ((null != m_pScrView) && (null != m_pScrView.verticalScrollBar))
        {
            m_pScrView.verticalScrollBar.gameObject.SetActive(count > 0);
        }

        if ((null != m_pScrView) && (null != m_pScrView.horizontalScrollBar))
        {
            m_pScrView.horizontalScrollBar.gameObject.SetActive(count > 0);
        }
    }

    public List<GameObject> GetActiveSlots()
    {
        return new List<GameObject>(m_dicSlots.Values);
    }

    public void SetFocus(int index, bool withOffset, bool lastCheck = false)
    {
        if ((0 > index) || 
            (index >= m_iReservedCount && index >= m_iTotalCount))
        {
            return;
        }

        if (m_pScrView == null || m_pScrView.panel == null || m_pGrid == null)
        {
            m_iReservedIndex = index;
            m_bReservedWithOffset = withOffset;
            m_bReservedLastCheck = lastCheck;
            return;
        }

        // 가야할 위치 고르기!!!!
        Vector3 cp = Vector3.zero;

        switch (m_pScrView.movement)
        {
            case UIScrollView.Movement.Horizontal:
                {
                    float x = (index / m_iMaxRow) * m_pGrid.cellWidth + ((withOffset == true) ? (float)m_iOffsetStart : 0f);
                    float y = -(index % m_iMaxRow) * m_pGrid.cellHeight;

                    if (lastCheck == true)
                    {
                        //Vector3 gridPos = _scrView.transform.worldToLocalMatrix * _grid.transform.position;
                        Vector3 gridPos = m_pGrid.transform.localPosition;

                        float maxX = gridPos.x + ((m_iTotalCount / m_iMaxRow) - 0.5f) * m_pGrid.cellWidth - m_pScrView.panel.baseClipRegion.z;

                        if (maxX < 0f)
                        {
                            maxX = 0f;
                        }

                        if (x > maxX)
                        {
                            x = maxX;
                        }
                    }

                    cp = new Vector3(x, y, 0f);
                }
                break;

            case UIScrollView.Movement.Vertical:
                {
                    float x = (index % m_iMaxColumn) * m_pGrid.cellWidth;
                    float y = -(index / m_iMaxColumn) * m_pGrid.cellHeight - ((withOffset == true) ? (float)m_iOffsetStart : 0f);

                    if (lastCheck == true)
                    {
                        //Vector3 gridPos = _scrView.transform.worldToLocalMatrix * _grid.transform.position;
                        Vector3 gridPos = m_pGrid.transform.localPosition;

                        float minY = -gridPos.y - ((m_iTotalCount / m_iMaxColumn) - 0.5f) * m_pGrid.cellHeight + m_pScrView.panel.baseClipRegion.w;

                        if (minY > 0f)
                        {
                            minY = 0f;
                        }

                        if (y < minY)
                        {
                            y = minY;
                        }
                    }

                    cp = new Vector3(x, y, 0f);
                }
                break;
        }

        Vector3[] corners = m_pScrView.panel.worldCorners;
        Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;

        Transform panelTrans = m_pScrView.panel.cachedTransform;

        // Figure out the difference between the chosen child and the panel's center in local coordinates
        Vector3 cc = panelTrans.InverseTransformPoint(panelCenter);
        Vector3 localOffset = cp - cc;

        // Offset shouldn't occur if blocked
        if (!m_pScrView.canMoveHorizontally) localOffset.x = 0f;
        if (!m_pScrView.canMoveVertically) localOffset.y = 0f;
        localOffset.z = 0f;

        Vector3 target = panelTrans.localPosition - localOffset;
        Vector3 before = panelTrans.localPosition;

        panelTrans.localPosition = target;

        Vector3 offset = (target - before);
        Vector2 cr = m_pScrView.panel.clipOffset;
        cr.x -= offset.x;
        cr.y -= offset.y;
        m_pScrView.panel.clipOffset = cr;
        m_pScrView.UpdateScrollbars(false);

        SetDirty();
        Paint();

        // Spring the panel to this calculated position
        //SpringPanel.Begin(_scrView.panel.cachedGameObject, panelTrans.localPosition - localOffset, 8f);
    }

    public void SetDirty()
    {
        m_bIsDirty = true;
    }


    public void Paint()
    {
        if (m_pScrView == null || m_pScrView.panel == null || m_pGrid == null)
        {
            return;
        }

        if (m_dicSlots.Count <= 0 && m_pSpareSlots.Count <= 0)
        {
            return;
        }

        if (m_iTotalCount <= 0)
        {
            foreach (GameObject slot in m_dicSlots.Values)
            {
                PushSlot(slot);
            }

            m_dicSlots.Clear();
            return;
        }

        switch (m_pScrView.movement)
        {
            case UIScrollView.Movement.Horizontal:
                Paint_Horizontal();
                break;

            case UIScrollView.Movement.Vertical:
                Paint_Vertical();
                break;
        }

        if (m_bMinSizeLockActivated == true)
        {
            m_pGrid.enabled = true;
            m_pGrid.Reposition();

            m_pScrView.enabled = false;
        }
    }

    public void Reinitialize()
    {
        if (m_bInitialized == false)
        {
            return;
        }

        ClearSpareSlots();
        MakeSpareSlots();
    }

    public bool IsInitialized()
    {
        return m_bInitialized;
    }

    public void ResetPosition()
    {
        if (m_pScrView == null)
            return;

        m_pScrView.ResetPosition();
        Paint();
    }

    public void SetListenerForSetData(System.Action<GameObject, int> listener)
    {
        m_pEventForSetData = listener;
    }
    #endregion

    
    #region Utility Functions
    private void SetSlotCount_Horizontal(int count)
    {
        m_iTotalCount = count;

        if (m_pSpriteArea != null)
        {
            if (count <= 0)
            {
                m_pSpriteArea.width = 0;
                return;
            }

            m_pSpriteArea.width = (int)((((count - 1) / m_iMaxRow) + 1f) * m_pGrid.cellWidth);
            m_pSpriteArea.width += (m_iOffsetStart + m_iOffsetEnd);

            if (m_pSpriteArea.width < m_iAreaSize)
            {
                m_pSpriteArea.width = (int)m_iAreaSize;

                if (m_bAreaSizeLock == true)
                {
                    m_bMinSizeLockActivated = true;

                    m_pGrid.pivot = UIWidget.Pivot.Center;
                    m_pGrid.transform.localPosition = Vector3.zero;
                }
                else
                {
                    m_bMinSizeLockActivated = false;
                    m_pScrView.enabled = true;

                    m_pGrid.pivot = UIWidget.Pivot.TopLeft;
                    m_pGrid.transform.localPosition = m_vGridOriginalPos;
                }
            }
            else
            {
                m_bMinSizeLockActivated = false;
                m_pScrView.enabled = true;

                m_pGrid.pivot = UIWidget.Pivot.TopLeft;
                m_pGrid.transform.localPosition = m_vGridOriginalPos;
            }
        }

        m_pScrView.ResetPosition();

        Paint();
    }

    private void SetSlotCount_Vertical(int count)
    {
        m_iTotalCount = count;

        if (m_pSpriteArea != null)
        {
            if (count <= 0)
            {
                m_pSpriteArea.height = 0;
                return;
            }

            m_pSpriteArea.height = (int)((((count - 1) / m_iMaxColumn) + 1f) * m_pGrid.cellHeight);
            m_pSpriteArea.height += (m_iOffsetStart + m_iOffsetEnd);

            if (m_pSpriteArea.height < m_iAreaSize)
            {
                m_pSpriteArea.height = (int)m_iAreaSize;

                if (m_bAreaSizeLock == true)
                {
                    m_bMinSizeLockActivated = true;

                    m_pGrid.pivot = UIWidget.Pivot.Center;
                    m_pGrid.transform.localPosition = Vector3.zero;
                }
                else
                {
                    m_bMinSizeLockActivated = false;
                    m_pScrView.enabled = true;

                    m_pGrid.pivot = UIWidget.Pivot.TopLeft;
                    m_pGrid.transform.localPosition = m_vGridOriginalPos;
                }
            }
            else
            {
                m_bMinSizeLockActivated = false;
                m_pScrView.enabled = true;

                m_pGrid.pivot = UIWidget.Pivot.TopLeft;
                m_pGrid.transform.localPosition = m_vGridOriginalPos;
            }
        }

        m_pScrView.ResetPosition();

        Paint();
    }

    private void Paint_Horizontal()
    {
        int start_Index = m_iMaxRow * (int)((-m_pScrView.panel.transform.localPosition.x - m_iOffsetStart) / m_pGrid.cellWidth) - m_iStartCache;

        if (start_Index < 0 || m_iTotalCount < (m_iMaxColumn * m_iMaxRow))
        {
            start_Index = 0;
        }

        if (start_Index >= m_iTotalCount)
        {
            start_Index = m_iTotalCount - 1;
        }

        int end_Index = start_Index + (m_iMaxColumn * m_iMaxRow);

        if (end_Index > m_iTotalCount)
        {
            end_Index = m_iTotalCount;
        }

        List<int> removeKeys = new List<int>();

        foreach (int index in m_dicSlots.Keys)
        {
            if (index < start_Index || index >= end_Index)
            {
                removeKeys.Add(index);
            }
        }

        foreach (int index in removeKeys)
        {
            PushSlot(m_dicSlots[index]);
            m_dicSlots.Remove(index);
        }

        for (int i = start_Index; i < end_Index; ++i)
        {
            float x = (i / m_iMaxRow) * m_pGrid.cellWidth + (float)m_iOffsetStart;
            float y = -(i % m_iMaxRow) * m_pGrid.cellHeight;

            GameObject slot = null;

            if (m_dicSlots.ContainsKey(i) == true)
            {
                slot = m_dicSlots[i];

                if (m_bIsDirty == true)
                {
                    SetSlotData(slot, i);

                    if (m_pEventForSetData != null)
                    {
                        m_pEventForSetData(slot, i);
                    }
                }
            }
            else
            {
                slot = PopSlot();
                m_dicSlots.Add(i, slot);

                slot.transform.localPosition = new Vector2(x, y);

                SetSlotData(slot, i);

                if (m_pEventForSetData != null)
                {
                    m_pEventForSetData(slot, i);
                }
            }
        }

        m_bIsDirty = false;
    }

    private void Paint_Vertical()
    {
        int start_Index = m_iMaxColumn * (int)((m_pScrView.panel.transform.localPosition.y - m_iOffsetStart) / m_pGrid.cellHeight) - m_iStartCache;

        if (start_Index < 0 || m_iTotalCount < (m_iMaxColumn * m_iMaxRow))
        {
            start_Index = 0;
        }

        if (start_Index >= m_iTotalCount)
        {
            start_Index = m_iTotalCount - 1;
        }

        int end_Index = start_Index + (m_iMaxColumn * m_iMaxRow);

        if (end_Index > m_iTotalCount)
        {
            end_Index = m_iTotalCount;
        }

        List<int> removeKeys = new List<int>();

        foreach (int index in m_dicSlots.Keys)
        {
            if (index < start_Index || index >= end_Index)
            {
                removeKeys.Add(index);
            }
        }

        foreach (int index in removeKeys)
        {
            PushSlot(m_dicSlots[index]);
            m_dicSlots.Remove(index);
        }

        for (int i = start_Index; i < end_Index; ++i)
        {
            float x = (i % m_iMaxColumn) * m_pGrid.cellWidth;
            float y = -(i / m_iMaxColumn) * m_pGrid.cellHeight - (float)m_iOffsetStart;

            GameObject slot = null;

            if (m_dicSlots.ContainsKey(i) == true)
            {
                slot = m_dicSlots[i];

                if (m_bIsDirty == true)
                {
                    SetSlotData(slot, i);

                    if (m_pEventForSetData != null)
                    {
                        m_pEventForSetData(slot, i);
                    }
                }
            }
            else
            {
                slot = PopSlot();
                m_dicSlots.Add(i, slot);

                slot.transform.localPosition = new Vector2(x, y);

                SetSlotData(slot, i);

                if (m_pEventForSetData != null)
                {
                    m_pEventForSetData(slot, i);
                }
            }
        }

        m_bIsDirty = false;
    }

    private void MakeSpareSlots()
    {
        int iCount = m_iMaxColumn * m_iMaxRow;

        for (int iLoop = 0; iLoop < iCount; ++iLoop)
        {
            var pSlot = Single.Resources.Instantiate<GameObject>(m_pSample);
            pSlot.transform.SetParent(m_pGrid.transform);
            pSlot.transform.localPosition   = Vector2.zero;
            pSlot.transform.localScale      = m_pSample.transform.localScale;
            pSlot.transform.localRotation   = Quaternion.Euler(Vector3.zero);

            PushSlot(pSlot);
        }
    }

    private void ClearSpareSlots()
    {
        foreach (var pSlot in m_dicSlots.Values)
        {
            PushSlot(pSlot);
        }

        m_dicSlots.Clear();

        foreach (var pSlot in m_pSpareSlots)
        {
            GameObject.Destroy(pSlot);
        }

        m_pSpareSlots.Clear();
    }

    private GameObject PopSlot()
    {
        if (m_pSpareSlots.Count <= 0)
        {
            return null;
        }

        var pSlot = m_pSpareSlots.Dequeue();

        NGUITools.SetActive(pSlot.gameObject, true);

        return pSlot;
    }

    private void PushSlot(GameObject pSlot)
    {
        NGUITools.SetActive(pSlot.gameObject, false);

        m_pSpareSlots.Enqueue(pSlot);
    }
    #endregion


    #region Event Handler
    private void DragStart()
    {
        m_bIsDragging = true;
    }

    private void DragEnd()
    {
        m_bIsDragging = false;
    }

    private void OnDragging()
    {
        Paint();
    }

    private void OnScrollBar()
    {
        Paint();
    }
    #endregion
}