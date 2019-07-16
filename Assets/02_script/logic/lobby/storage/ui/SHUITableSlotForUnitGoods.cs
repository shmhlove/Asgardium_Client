using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUITableSlotForUnitGoods : MonoBehaviour
{
    public UISprite m_pSpriteBG;
    public UIGrid m_pGrid;
    public GameObject m_pUnitSample;

    private List<GameObject> m_pUnitObjects = new List<GameObject>();
    private Action<int> m_pEventTransaction;

    private void Awake()
    {
        if (null != m_pUnitSample)
        {
            m_pUnitSample.SetActive(false);
        }
    }

    private void OnEnable()
    {
        m_pSpriteBG.height = (int)(m_pGrid.GetChildList().Count * m_pGrid.cellHeight) + 10;
    }

    public void SetData(List<SHTableGridSlotForUnitData> pDatas, Action<int> pEventTransaction)
    {
        MakeSpareSlots(pDatas.Count);
        
        for (int iLoop = 0; iLoop < m_pUnitObjects.Count; ++iLoop)
        {
            if (iLoop < pDatas.Count)
            {
                NGUITools.SetActive(m_pUnitObjects[iLoop], true);
                
                var pSlot = m_pUnitObjects[iLoop].GetComponent<SHUITableGridSlotForUnit>();
                pSlot.SetData(pDatas[iLoop], OnClickButton);
            }
            else
            {
                NGUITools.SetActive(m_pUnitObjects[iLoop], false);
            }
        }

        m_pEventTransaction = pEventTransaction;

        m_pGrid.repositionNow = true;
        var pUITable = transform.GetComponentInParent<UITable>();
        pUITable.repositionNow = true;

        m_pSpriteBG.height = (int)(m_pGrid.GetChildList().Count * m_pGrid.cellHeight) + 10;
    }

    private void MakeSpareSlots(int iCount)
    {
        if (0 != m_pUnitObjects.Count)
        {
            for (int iLoop = 0; iLoop < m_pUnitObjects.Count; ++iLoop)
            {
                Destroy(m_pUnitObjects[iLoop]);
            }
            m_pUnitObjects.Clear();
        }

        for (int iLoop = 0; iLoop < iCount; ++iLoop)
        {
            var pSlot = Single.Resources.Instantiate<GameObject>(m_pUnitSample);
            pSlot.transform.SetParent(m_pGrid.gameObject.transform);
            pSlot.transform.localPosition = Vector2.zero;
            pSlot.transform.localScale = m_pUnitSample.transform.localScale;
            pSlot.transform.localRotation = Quaternion.Euler(Vector3.zero);

            NGUITools.SetActive(pSlot, false);
            m_pUnitObjects.Add(pSlot);
        }

        NGUITools.SetActive(m_pUnitSample, false);
    }

    public void OnClickButton(int iUnitId)
    {
        if (null == m_pEventTransaction)
        {
            return;
        }

        m_pEventTransaction(iUnitId);
    }
}
