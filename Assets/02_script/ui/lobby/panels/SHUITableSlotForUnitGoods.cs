using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUITableSlotForUnitGoods : MonoBehaviour
{
    public UISprite     m_pSpriteBG;
    public UIGrid       m_pGrid;
    public GameObject   m_pUnitSample;

    private List<GameObject> m_pUnitObjects = new List<GameObject>();

    private void Awake()
    {
        if (null != m_pUnitSample)
        {
            m_pUnitSample.SetActive(false);
        }
    }

    public void ResetData(List<SHTableGridSlotForUnitData> pData, Action<int> pEventTransaction)
    {
        if (m_pSpriteBG)
        {
            m_pSpriteBG.height = (int)(pData.Count * m_pGrid.cellHeight) + 10;
        }

        MakeSpareSlots(pData.Count);
        
        for (int iLoop = 0; iLoop < m_pUnitObjects.Count; ++iLoop)
        {
            if (iLoop < pData.Count)
            {
                NGUITools.SetActive(m_pUnitObjects[iLoop], true);
                
                var pSlot = m_pUnitObjects[iLoop].GetComponent<SHUITableGridSlotForUnit>();
                pSlot.SetData(pData[iLoop], pEventTransaction);
            }
            else
            {
                NGUITools.SetActive(m_pUnitObjects[iLoop], false);
            }
        }

        m_pGrid.repositionNow = true;
        m_pGrid.Reposition();

        var pUITable = transform.GetComponentInParent<UITable>();
        if (pUITable)
        {
            pUITable.repositionNow = true;
            pUITable.Reposition();
        }
    }

    private void MakeSpareSlots(int iCount)
    {
        if (null == m_pUnitSample)
        {
            return;
        }

        int iMakeSlotCount = iCount - m_pUnitObjects.Count;
        for (int iLoop = 0; iLoop < iMakeSlotCount; ++iLoop)
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
}
