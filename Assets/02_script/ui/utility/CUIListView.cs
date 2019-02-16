using UnityEngine;
using System;
using System.Collections.Generic;

public class CListViewItem
{
    public int m_Index;
    public GameObject m_GameObject;
}

[Serializable]
public class CUIListView
{
    public enum eShowCondition
    {
        Always,
        OnlyIfNeeded,
        WhenDragging,
    }

    public enum eTransition
    {
        Smooth,
        Instant,
    }

    public GameObject m_scItemList;
    public GameObject m_pfItem;
    public GameObject[] m_goCreatedItem;

    public int m_AddRowORColumn = 2;

    public Vector2 m_ItemPadding;
    public UIScrollBar m_Scrollbar;
    public eShowCondition m_ShowScrollBar = eShowCondition.Always;
    public eTransition m_TransitionScrollBar = eTransition.Instant;

    protected UIPanel m_Panel;
    public UIPanel Panel { get { return m_Panel; } }
    protected UIScrollView m_ScrollView;
    public UIScrollView ScrollView { get { return m_ScrollView; } }
    protected LinkedList<CListViewItem> m_listItem = new LinkedList<CListViewItem>();

    private Vector2 m_ItemSize;
    private Vector3 m_TotalItemSize;
    private Vector3 m_InitPosition;
    private float m_InitBound;

    private int m_rowCount;
    private int m_columnCount;
    private int m_MaxItemCountInView;
    private int m_MaxItemCount;

    public float m_ExtentX2;
    public float m_ExtentY2;
    public float m_ExtentX;
    public float m_ExtentY;

    public bool m_bInitOnMovePanel = false;

    private bool m_bHorizontal = false;
    private float m_fVIewYSize = 0;
    private bool m_bIsDragging = false;
    public bool IsDragging { get { return m_bIsDragging; } }
    private bool m_bLockChangeScrollBar = false;
    public bool LockChangeScrollBar { get { return m_bLockChangeScrollBar; } set { m_bLockChangeScrollBar = value; } }
    private SpringPanel.OnFinished m_onFinished = null;

    public System.Action<int, GameObject> OnUpdateItem = null;
    public System.Func<int> OnGetItemCount = null;
    // ScrollView Drag 체크용[blueasa / 2106-10-24]
    public System.Action<int> OnUpdateScrollViewDragging = null;

    public UIScrollView.OnDragNotification onDragStarted;
    public UIScrollView.OnDragNotification onStoppedMoving;
    public UIScrollView.OnDragNotification onDragFinished;

    public void Init()
    {
        Clear();

        if (m_scItemList == null || m_pfItem == null)
            return;

        m_ScrollView = m_scItemList.GetComponent<UIScrollView>();
        m_Panel = m_scItemList.GetComponent<UIPanel>();
        m_Panel.onClipMove = OnMove;
        if (m_Scrollbar != null)
        {
            EventDelegate.Add(m_Scrollbar.onChange, OnChangeScrollBar);
            ShowScrollbars(false);
        }
        
        Vector4 clip = m_Panel.baseClipRegion;
        Vector2 viewSize = new Vector2(clip.z, clip.w);
        m_fVIewYSize = viewSize.y;

        m_pfItem.SetActive(false);
        
        // original
        //Bounds bound = NGUIMath.CalculateRelativeWidgetBounds(m_Item.transform, true);
        //m_ItemSize = new Vector2(bound.size.x, bound.size.y);
        
        // modified[blueasa /2015-06-23]
        BoxCollider boxCollider = m_pfItem.GetComponent<BoxCollider>();
        m_ItemSize = new Vector2(boxCollider.size.x, boxCollider.size.y);

        m_TotalItemSize = m_ItemSize + m_ItemPadding;

        m_columnCount = Mathf.RoundToInt(viewSize.x / m_TotalItemSize.x);
        m_rowCount = Mathf.RoundToInt(viewSize.y / m_TotalItemSize.y);
        m_MaxItemCountInView = m_columnCount * m_rowCount;

        if (m_ScrollView.movement == UIScrollView.Movement.Horizontal)
        {
            m_bHorizontal = true;
            m_columnCount = m_columnCount + m_AddRowORColumn;

            m_InitPosition.x = -(viewSize.x * 0.5f) + (m_TotalItemSize.x / 2.0f);
            m_InitPosition.y = m_TotalItemSize.y * (m_rowCount - 1) / 2.0f;
            m_InitBound = -(viewSize.x + m_ItemPadding.x) * 0.5f;
        }
        else if (m_ScrollView.movement == UIScrollView.Movement.Vertical)
        {
            m_bHorizontal = false;
            m_rowCount = m_rowCount + m_AddRowORColumn;

            m_InitPosition.x = -m_TotalItemSize.x * (m_columnCount - 1) / 2.0f;
            m_InitPosition.y = (viewSize.y / 2.0f) - (m_TotalItemSize.y / 2.0f);
            m_InitBound = (viewSize.y - m_ItemPadding.y) * 0.5f;
        }
        m_MaxItemCount = m_columnCount * m_rowCount;

        m_ExtentX2 = m_TotalItemSize.x * m_columnCount;
        m_ExtentY2 = m_TotalItemSize.y * m_rowCount;
        m_ExtentX = m_ExtentX2 * 0.5f;
        m_ExtentY = m_ExtentY2 * 0.5f;

        // 아이템 생성.
        CListViewItem item = null;
        for (int i = 0; i < m_MaxItemCount; ++i)
        {
            item = new CListViewItem();
            item.m_Index = i;

            if (null != m_goCreatedItem && m_goCreatedItem.Length > i)
            {
                item.m_GameObject = m_goCreatedItem[i];
            }
            else
            {
                item.m_GameObject = NGUITools.AddChild(m_scItemList, m_pfItem);
            }
            m_listItem.AddLast(item);
        }

        AddEvent();

        Reset();

        // 스크롤바 초기화[blueasa / 2018-07-26]
        if (null != m_Scrollbar)
            UpdateScrollBars();
    }

    private void Clear()
    {   
        foreach (CListViewItem item in m_listItem)
        {
            if (null != m_goCreatedItem && m_goCreatedItem.Length > item.m_Index)
            {
                continue;
            }

            GameObject.Destroy(item.m_GameObject);
        }
        m_listItem.Clear();
    }

    private void OnMove(UIPanel panel)
    {
        Update();

        if (true == m_bLockChangeScrollBar || true == m_bIsDragging && null != m_Scrollbar)
            UpdateScrollBars();
    }

    private void Update()
    {
        int realIndex = 0;
        bool flag = true;
        float distance = 0.0f;
        Vector3 pos;
        Vector4 clip = m_Panel.finalClipRegion;
        Transform trans = null;
        CListViewItem item = null;

        // 앵커로 인한 view 사이즈 변동 확인
        if(m_fVIewYSize != m_Panel.baseClipRegion.w)
        {
            if(false == m_bInitOnMovePanel)
                Init();
//             else
//                 Refresh();
        }

        if (m_bHorizontal)
        {
            while (flag)
            {
                item = m_listItem.First.Value;
                trans = item.m_GameObject.transform;
                pos = trans.localPosition;
                distance = pos.x - clip.x;

                if (distance < -m_ExtentX)
                {
                    for (int i = 0; i < m_rowCount; ++i)
                    {
                        item = m_listItem.First.Value;
                        trans = item.m_GameObject.transform;
                        pos = trans.localPosition;

                        realIndex = item.m_Index + m_MaxItemCount;
                        if (i == 0 && realIndex >= OnGetItemCount())
                        {
                            flag = false;
                            break;
                        }

                        item.m_Index = realIndex;
                        pos.x += m_ExtentX2;
                        trans.localPosition = pos;
                        trans.name = realIndex.ToString();

                        if (realIndex < OnGetItemCount())
                        {
                            OnUpdateItem(realIndex, item.m_GameObject);
                            item.m_GameObject.SetActive(true);
                        }
                        else
                        {
                            item.m_GameObject.SetActive(false);
                        }

                        m_listItem.RemoveFirst();
                        m_listItem.AddLast(item);
                        m_ScrollView.InvalidateBounds();
                    }
                }
                else
                {
                    break;
                }
            }

            flag = true;
            while (flag)
            {
                item = m_listItem.Last.Value;
                trans = item.m_GameObject.transform;
                pos = trans.localPosition;
                distance = pos.x - clip.x;

                if (distance > m_ExtentX)
                {
                    for (int i = 0; i < m_rowCount; ++i)
                    {
                        item = m_listItem.Last.Value;
                        trans = item.m_GameObject.transform;
                        pos = trans.localPosition;

                        realIndex = item.m_Index - m_MaxItemCount;
                        if (realIndex >= 0 && realIndex < OnGetItemCount())
                        {
                            item.m_Index = realIndex;
                            pos.x -= m_ExtentX2;
                            trans.localPosition = pos;
                            trans.name = realIndex.ToString();

                            OnUpdateItem(realIndex, item.m_GameObject);

                            item.m_GameObject.SetActive(true);
                            m_listItem.RemoveLast();
                            m_listItem.AddFirst(item);
                            m_ScrollView.InvalidateBounds();
                        }
                        else
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            while (flag)
            {
                item = m_listItem.First.Value;
                trans = item.m_GameObject.transform;
                pos = trans.localPosition;
                distance = pos.y - clip.y;

                if (distance > m_ExtentY)
                {
                    for (int i = 0; i < m_columnCount; ++i)
                    {
                        item = m_listItem.First.Value;
                        trans = item.m_GameObject.transform;
                        pos = trans.localPosition;

                        realIndex = item.m_Index + m_MaxItemCount;
                        if (i == 0 && realIndex >= OnGetItemCount())
                        {
                            flag = false;
                            break;
                        }

                        item.m_Index = realIndex;
                        pos.y -= m_ExtentY2;
                        trans.localPosition = pos;
                        trans.name = realIndex.ToString();

                        if (realIndex < OnGetItemCount())
                        {
                            OnUpdateItem(realIndex, item.m_GameObject);
                            item.m_GameObject.SetActive(true);
                        }
                        else
                        {
                            item.m_GameObject.SetActive(false);
                        }

                        m_listItem.RemoveFirst();
                        m_listItem.AddLast(item);
                        m_ScrollView.InvalidateBounds();
                    }
                }
                else
                {
                    break;
                }
            }

            flag = true;
            while (flag)
            {
                item = m_listItem.Last.Value;
                trans = item.m_GameObject.transform;
                pos = trans.localPosition;
                distance = pos.y - clip.y;

                if (distance < -m_ExtentY)
                {
                    for (int i = 0; i < m_columnCount; ++i)
                    {
                        item = m_listItem.Last.Value;
                        trans = item.m_GameObject.transform;
                        pos = trans.localPosition;

                        realIndex = int.Parse(trans.name) - m_MaxItemCount;
                        if (realIndex >= 0 && realIndex < OnGetItemCount())
                        {
                            item.m_Index = realIndex;
                            pos.y += m_ExtentY2;
                            trans.localPosition = pos;
                            trans.name = realIndex.ToString();

                            OnUpdateItem(realIndex, item.m_GameObject);

                            item.m_GameObject.SetActive(true);
                            m_listItem.RemoveLast();
                            m_listItem.AddFirst(item);
                            m_ScrollView.InvalidateBounds();
                        }
                        else
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        UpdateScrollViewDragging();
    }

    void UpdateScrollViewDragging()
    {
        if (null != m_ScrollView)
        {

            //if (true == m_ScrollView.isDragging)
            if(true == m_bIsDragging)
            {
                //Debug.LogWarningFormat("[m_bIsDragging] {0}", m_bIsDragging);

                if (null != OnUpdateScrollViewDragging)
                {
                    OnUpdateScrollViewDragging(m_listItem.First.Value.m_Index);
                }
            }
        }
    }

    void AddEvent()
    {
        if (null != m_ScrollView)
        {
            m_ScrollView.onDragStarted += OnDragStarted;
            m_ScrollView.onStoppedMoving += OnStoppedMoving;
            m_ScrollView.onDragFinished += OnDragFinished;
        }
    }

    void OnDragStarted()
    {
        //Debug.LogWarningFormat("[[[[ OnDragStarted() ]]]]");
        m_bIsDragging = true;
        ShowScrollbars(true);
        if (null != onDragStarted)
            onDragStarted();
    }

    void OnStoppedMoving()
    {
        //Debug.LogWarningFormat("[[[[ OnStoppedMoving() ]]]]");
        m_bIsDragging = false;
        ShowScrollbars(false);
        if (null != onStoppedMoving)
            onStoppedMoving();
    }

    void OnDragFinished()
    {
        //Debug.LogWarningFormat("[[[[ OnDragFinished() ]]]]");
        if (null != onDragFinished)
            onDragFinished();
    }

    void ShowScrollbars(bool _bShow)
    {
        if (null == m_Scrollbar)
            return;

        float fAlpha = 0f;

        switch(m_ShowScrollBar)
        {
            case eShowCondition.Always:
                {
                    fAlpha = 1f;
                }
                break;

            case eShowCondition.OnlyIfNeeded:
            case eShowCondition.WhenDragging:
                {
                    fAlpha = (false == _bShow) ? 0f : 1f;
                }
                break;
        }

        m_Scrollbar.alpha = fAlpha;
    }

    //IEnumerator TweenAlphaScrollbar(bool _bShow)
    //{
    //    if(true == _bShow)
    //    {
    //        while(m_Scrollbar.alpha < 1f)
    //        {

    //            yield return null;
    //        }
    //    }
    //    else
    //    {
    //        while (m_Scrollbar.alpha < 1f)
    //        {

    //            yield return null;
    //        }
    //    }

    //}

    public void UpdateScrollBars()
    {
        Bounds b = m_ScrollView.bounds;
        Vector2 bmin = b.min;
        Vector2 bmax = b.max;

        if (m_bHorizontal && bmax.x > bmin.x)
        {
            Vector4 clip = m_Panel.finalClipRegion;
            int intViewSize = Mathf.RoundToInt(clip.z);
            if ((intViewSize & 1) != 0) intViewSize -= 1;
            float halfViewSize = intViewSize * 0.5f;
            halfViewSize = Mathf.Round(halfViewSize);

            if (m_Panel.clipping == UIDrawCall.Clipping.SoftClip)
                halfViewSize -= m_Panel.clipSoftness.x;

            float contentSize = GetContentsSize();
            float viewSize = halfViewSize * 2f;

            float contentMin = m_InitBound;
            float contentMax = contentMin + contentSize;

            float viewMin = clip.x - halfViewSize;
            float viewMax = clip.x + halfViewSize;

            contentMin = viewMin - contentMin;
            contentMax = contentMax - viewMax;

            UpdateScrollBars(m_Scrollbar, contentMin, contentMax, contentSize, viewSize, false);
        }

        if (!m_bHorizontal && bmax.y > bmin.y)
        {
            Vector4 clip = m_Panel.finalClipRegion;
            int intViewSize = Mathf.RoundToInt(clip.w);
            if ((intViewSize & 1) != 0) intViewSize -= 1;
            float halfViewSize = intViewSize * 0.5f;
            halfViewSize = Mathf.Round(halfViewSize);

            if (m_Panel.clipping == UIDrawCall.Clipping.SoftClip)
                halfViewSize -= m_Panel.clipSoftness.y;

            float contentSize = GetContentsSize();
            float viewSize = halfViewSize * 2f;

            float contentMax = m_InitBound;
            float contentMin = contentMax - contentSize;

            float viewMin = clip.y - halfViewSize;
            float viewMax = clip.y + halfViewSize;

            contentMin = viewMin - contentMin;
            contentMax = contentMax - viewMax;

            UpdateScrollBars(m_Scrollbar, contentMin, contentMax, contentSize, viewSize, true);
        }
    }

    private void UpdateScrollBars(UIProgressBar slider, float contentMin, float contentMax, float contentSize, float viewSize, bool inverted)
    {
        if (slider == null)
            return;

        float contentPadding;

        if (viewSize < contentSize)
        {
            contentMin = Mathf.Clamp01(contentMin / contentSize);
            contentMax = Mathf.Clamp01(contentMax / contentSize);

            contentPadding = contentMin + contentMax;
            slider.value = inverted ? ((contentPadding > 0.001f) ? 1f - contentMin / contentPadding : 0f) :
                ((contentPadding > 0.001f) ? contentMin / contentPadding : 1f);
        }
        else
        {
            contentMin = Mathf.Clamp01(-contentMin / contentSize);
            contentMax = Mathf.Clamp01(-contentMax / contentSize);

            contentPadding = contentMin + contentMax;
            slider.value = inverted ? ((contentPadding > 0.001f) ? 1f - contentMin / contentPadding : 0f) :
                ((contentPadding > 0.001f) ? contentMin / contentPadding : 1f);

            if (contentSize > 0)
            {
                contentMin = Mathf.Clamp01(contentMin / contentSize);
                contentMax = Mathf.Clamp01(contentMax / contentSize);
                contentPadding = contentMin + contentMax;
            }
        }

        UIScrollBar sb = slider as UIScrollBar;
        if (sb != null)
            sb.barSize = 1f - contentPadding;
    }

    private void OnChangeScrollBar()
    {
        if (true == m_bIsDragging)
            return;

        if (true == m_bLockChangeScrollBar)
            return;

        if (null == m_Scrollbar || null == OnGetItemCount)
            return;

        int iItemCount = OnGetItemCount();
        int iIndex = Convert.ToInt32(m_Scrollbar.value * OnGetItemCount());
        SetFocusWithoutScrollBar(iIndex);
    }

    // 스크롤뷰 리셋.
    public void Reset()
    {
        int index = 0;
        int rowIndex = 0, columnIndex = 0;
        Vector3 position = new Vector3();

        foreach (CListViewItem item in m_listItem)
        {
            item.m_Index = index;
            item.m_GameObject.name = index.ToString();
            if (index < OnGetItemCount())
            {
                OnUpdateItem(index, item.m_GameObject);
                item.m_GameObject.SetActive(true);
            }
            else
            {
                item.m_GameObject.SetActive(false);
            }

            if (m_bHorizontal)
            {
                rowIndex = index % m_rowCount;
                columnIndex = index / m_rowCount;
            }
            else
            {
                rowIndex = index / m_columnCount;
                columnIndex = index % m_columnCount;
            }

            position.x = m_InitPosition.x + (m_TotalItemSize.x * columnIndex);
            position.y = m_InitPosition.y - (m_TotalItemSize.y * rowIndex);
            item.m_GameObject.transform.localPosition = position;
            ++index;
        }

        Vector4 clip = m_Panel.finalClipRegion;
        if (m_bHorizontal)
        {
            if (GetContentsSize() < clip.z)
            // blueasa
            //if (GetContentsSizeMinusOne() < clip.z)
            {
                m_ScrollView.ResetPosition();
                m_ScrollView.SetDragAmount(m_ItemPadding.x / clip.z, 0.0f, false);
                m_ScrollView.enabled = false;
            }
            else
            {
                m_ScrollView.enabled = true;
                m_ScrollView.ResetPosition();
            }
        }
        else
        {
            if (GetContentsSize() < clip.w)
            // blueasa
            //if (GetContentsSizeMinusOne() < clip.w)
            {
                m_ScrollView.ResetPosition();
                m_ScrollView.SetDragAmount(0.0f, m_ItemPadding.y / clip.w, false);
                m_ScrollView.enabled = false;
            }
            else
            {
                m_ScrollView.enabled = true;
                m_ScrollView.ResetPosition();
            }
        }
    }

    // 현재 스크롤뷰 위치에서 아이템만 갱신.
    public void Refresh()
    {
        foreach (CListViewItem item in m_listItem)
        {
            if (item.m_Index < OnGetItemCount())
            {
                OnUpdateItem(item.m_Index, item.m_GameObject);
                item.m_GameObject.SetActive(true);
            }
            else
            {
                item.m_GameObject.SetActive(false);
            }
        }

        // 아이템이 줄어들어 빈공간이 보이면 뷰를 옮겨준다.
        Vector3 movePos = new Vector3();
        Vector4 clip = m_Panel.finalClipRegion;
        float contentsSize = GetContentsSize();
        // blueasa
        //float contentsSize = GetContentsSizeMinusOne();
        float viewSize;
        if (m_bHorizontal)
        {
            m_InitBound = -(clip.z + m_ItemPadding.x) * 0.5f;
            viewSize = clip.z;
            float viewPos = clip.x + clip.z * 0.5f;
            //float startPos = (-clip.z) * 0.5f + clip.z * 0.5f;
            //float limitPos = (-clip.z) * 0.5f + contentsSize - clip.z * 0.5f;
            float startPos = m_InitBound + clip.z * 0.5f;
            float limitPos = m_InitBound + contentsSize - clip.z * 0.5f;

            if (contentsSize < viewSize && viewPos > contentsSize - clip.z * 0.5f)
            {
                movePos.x = Mathf.Max(limitPos, startPos) - clip.x;
                SpringPanel.Begin(m_Panel.cachedGameObject, m_Panel.cachedTransform.localPosition - movePos, 8.0f);
            }
        }
        else
        {
            m_InitBound = (clip.w - m_ItemPadding.y) * 0.5f;
            viewSize = clip.w;
            float viewPos = clip.y - clip.w * 0.5f;
            float startPos = m_InitBound - clip.w * 0.5f;
            float limitPos = m_InitBound - contentsSize + clip.w * 0.5f;
            // blueasa
            //float limitPos = m_InitBound - GetContentsSizeMinusOne() + clip.w * 0.5f;

            // 위치 조절 안되는 버그 수정[blueasa / 2015-12-17]
            if (contentsSize < viewSize || viewPos < limitPos)
            {
                movePos.y = Mathf.Min(limitPos, startPos) - clip.y;
                SpringPanel.Begin(m_Panel.cachedGameObject, m_Panel.cachedTransform.localPosition - movePos, 8.0f);
            }
        }

        if (contentsSize < viewSize)
        {
            m_ScrollView.enabled = false;
        }
        else
        {
            m_ScrollView.enabled = true;
        }
    }

    // 전체 아이템 크기.
    public float GetContentsSize()
    {
        if (m_bHorizontal)
        {
            float columnCount = (float)OnGetItemCount() / m_rowCount;
            int totalColumnCount = Mathf.CeilToInt(columnCount);
            return m_TotalItemSize.x * totalColumnCount;
        }
        else
        {
            float rowCount = (float)OnGetItemCount() / m_columnCount;
            int totalRowCount = Mathf.CeilToInt(rowCount);
            return m_TotalItemSize.y * totalRowCount - m_ItemPadding.y;
        }
    }

    // 전체 아이템 크기 - 1
    // 빈공간 채우는 카드를 넣어도 안움직이도록 하기 위해..
    public float GetContentsSizeMinusOne()
    {
        if (m_bHorizontal)
        {
            float columnCount = (float)(OnGetItemCount() - 1 ) / m_rowCount;
            int totalColumnCount = Mathf.CeilToInt(columnCount);
            return m_TotalItemSize.x * totalColumnCount;
        }
        else
        {
            float rowCount = (float)(OnGetItemCount() - 1) / m_columnCount;
            int totalRowCount = Mathf.CeilToInt(rowCount);
            return m_TotalItemSize.y * totalRowCount - m_ItemPadding.y;
        }
    }
     
    // 현재 뷰를 해당 인덱스가 있는 곳으로 옮겨준다.
    public void SetFocus(int index, SpringPanel.OnFinished onFinished = null)
    {
        if (index >= OnGetItemCount())
            return;

        Vector3 movePos = new Vector3();
        Vector4 clip = m_Panel.finalClipRegion;
        
        float contentsSize = GetContentsSize();
        float viewSize = 0f;

        if (m_bHorizontal)
        {
            viewSize = clip.z;
            int columnCount = index / m_rowCount;
            float targetPos = m_InitBound + columnCount * m_TotalItemSize.x;
            float startPos = m_InitBound + clip.z * 0.5f;
            float limitPos = m_InitBound + GetContentsSize() - clip.z * 0.5f;
            targetPos = Mathf.Max(targetPos, startPos);
            targetPos = Mathf.Min(targetPos, limitPos);
            movePos.x = targetPos - clip.x;
        }
        else
        {
            viewSize = clip.w;
            int rowCount = index / m_columnCount;
            float targetPos = m_InitBound - rowCount * m_TotalItemSize.y;
            float startPos = m_InitBound - clip.w * 0.5f;
            float limitPos = m_InitBound - GetContentsSize() + clip.w * 0.5f;
            // Vertical에서 아이템이 아래로 붙던 문제 수정[blueasa / 2015-12-08]
            targetPos = Mathf.Max(targetPos, limitPos);
            targetPos = Mathf.Min(targetPos, startPos);

            movePos.y = targetPos - clip.y;
        }

        // 컨텐츠 사이즈가 패널 사이즈보다 작을 때, 포커싱 이동 안하도록 수정[blueasa / 2015-11-16]
       // if (contentsSize > viewSize)
        {
            m_bLockChangeScrollBar = true;
            SpringPanel springPanel = SpringPanel.Begin(m_Panel.cachedGameObject, m_Panel.cachedTransform.localPosition - movePos, 8.0f);
            if(null != springPanel)
            {
                m_onFinished = onFinished;
                springPanel.onFinished = OnFinishedSetFocus;
                //springPanel.onFinished = onFinished;
            }
        }
    }

    void OnFinishedSetFocus()
    {
        m_bLockChangeScrollBar = false;
        if (null != m_onFinished)
        {
            m_onFinished();
            m_onFinished = null;
        }
    }

    void SetFocusWithoutScrollBar(int index, SpringPanel.OnFinished onFinished = null)
    {
        if (index >= OnGetItemCount())
            return;

        Vector3 movePos = new Vector3();
        Vector4 clip = m_Panel.finalClipRegion;

        float contentsSize = GetContentsSize();
        float viewSize = 0f;

        if (m_bHorizontal)
        {
            viewSize = clip.z;
            int columnCount = index / m_rowCount;
            float targetPos = m_InitBound + columnCount * m_TotalItemSize.x;
            float startPos = m_InitBound + clip.z * 0.5f;
            float limitPos = m_InitBound + GetContentsSize() - clip.z * 0.5f;
            targetPos = Mathf.Max(targetPos, startPos);
            targetPos = Mathf.Min(targetPos, limitPos);
            movePos.x = targetPos - clip.x;
        }
        else
        {
            viewSize = clip.w;
            int rowCount = index / m_columnCount;
            float targetPos = m_InitBound - rowCount * m_TotalItemSize.y;
            float startPos = m_InitBound - clip.w * 0.5f;
            float limitPos = m_InitBound - GetContentsSize() + clip.w * 0.5f;
            // Vertical에서 아이템이 아래로 붙던 문제 수정[blueasa / 2015-12-08]
            targetPos = Mathf.Max(targetPos, limitPos);
            targetPos = Mathf.Min(targetPos, startPos);

            movePos.y = targetPos - clip.y;
        }

        // 컨텐츠 사이즈가 패널 사이즈보다 작을 때, 포커싱 이동 안하도록 수정[blueasa / 2015-11-16]
        // if (contentsSize > viewSize)
        {
            SpringPanel springPanel = SpringPanel.Begin(m_Panel.cachedGameObject, m_Panel.cachedTransform.localPosition - movePos, 8.0f);
            if (null != springPanel)
            {
                springPanel.onFinished = onFinished;
            }
        }
    }

    // 현재 뷰를 해당 인덱스가 있는 곳으로 옮겨준다.
    public void SetFocusOnCenter(int index, float strength = 8.0f)
    {
        if (index >= OnGetItemCount())
            return;

        Vector3 movePos = new Vector3();
        Vector4 clip = m_Panel.finalClipRegion;

        float contentsSize = GetContentsSize();

        // Center 제대로 안맞는 버그 수정[blueasa / 2015-12-04]
        if (m_bHorizontal)
        {
            int columnCount = index / m_rowCount;
            float targetPos = columnCount * m_TotalItemSize.x;
            float startPos = clip.z * 0.5f;
            float limitPos = GetContentsSize() - clip.z * 0.5f;
            //float targetPos = m_InitBound + columnCount * m_TotalItemSize.x;
            //float startPos = m_InitBound + clip.z * 0.5f;
            //float limitPos = m_InitBound + GetContentsSize() - clip.z * 0.5f;
            targetPos = Mathf.Max(targetPos, startPos);
            targetPos = Mathf.Min(targetPos, limitPos);

            targetPos = targetPos - (clip.z * 0.5f) - (m_TotalItemSize.x * 0.5f);
            movePos.x = targetPos - clip.x;
            //movePos.x = targetPos + m_InitBound;
            //movePos.x = targetPos - clip.z * 0.5f;
            //movePos.x = movePos.x - (clip.z * 0.5f) + m_TotalItemSize.x;
            //movePos.x = movePos.x - (clip.z * 0.5f) + (m_TotalItemSize.x * 0.5f);
        }
        else
        {
            int rowCount = index / m_columnCount;
            float targetPos = rowCount * m_TotalItemSize.y;
            float startPos = clip.w * 0.5f;
            float limitPos = GetContentsSize() + clip.w * 0.5f;
            //float targetPos = m_InitBound - rowCount * m_TotalItemSize.y;
            //float startPos = m_InitBound - clip.w * 0.5f;
            //float limitPos = m_InitBound - GetContentsSize() + clip.w * 0.5f;
            targetPos = Mathf.Max(targetPos, limitPos);
            targetPos = Mathf.Min(targetPos, startPos);
            
            targetPos = targetPos - (clip.w * 0.5f) - (m_TotalItemSize.y * 0.5f);
            movePos.y = targetPos - clip.y;
            //movePos.y = movePos.y - (clip.w * 0.5f) + m_TotalItemSize.y;
            //movePos.y = movePos.y - (clip.w * 0.5f) + (m_TotalItemSize.y * 0.5f);
        }

        // 컨텐츠 사이즈가 패널 사이즈보다 작을 때, 포커싱 이동 안하도록 수정[blueasa / 2015-11-16]
      //  if (contentsSize > viewSize)
        {
            SpringPanel.Begin(m_Panel.cachedGameObject, m_Panel.cachedTransform.localPosition - movePos, strength);
        }
    }

    public LinkedList<CListViewItem> GetList()
    {
        return m_listItem;
    }

    public GameObject GetItem(int index)
    {
        foreach (CListViewItem item in m_listItem)
        {
            if (item.m_Index == index)
                return item.m_GameObject;
        }

        return null;
    }
}