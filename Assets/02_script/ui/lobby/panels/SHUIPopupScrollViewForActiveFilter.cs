using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUIPopupScrollViewForActiveFilter : SHUIMassiveScrollView
{
    List<SHActiveFilterUnitData> m_pDatas;

    protected override void SetSlotData(GameObject go, int index)
    {
        if ((null == go) || (null == m_pDatas))
        {
            return;
        }

        var pSlot = go.GetComponent<SHUIPopupScrollSlotForActiveFilter>();
        var iLength = pSlot.m_iMaxCount;
        var iStartIndex = pSlot.m_iMaxCount * index;

        if (iStartIndex >= m_pDatas.Count)
        {
            return;
        }

        if ((iStartIndex + iLength) > m_pDatas.Count)
        {
            iLength = m_pDatas.Count - iStartIndex;
        }

        pSlot.SetData(m_pDatas.GetRange(iStartIndex, iLength), OnToggleUnit);
    }

    public void ResetDatas(List<SHActiveFilterUnitData> pDatas)
    {
        if ((null == m_pDatas) || (m_pDatas.Count != pDatas.Count))
        {
            m_pDatas = pDatas;

            var pSlot = m_pSample.GetComponent<SHUIPopupScrollSlotForActiveFilter>();
            if ((null == pSlot) || (0 == pSlot.m_iMaxCount))
            {
                return;
            }
            
            SetSlotCount(Mathf.RoundToInt((float)m_pDatas.Count / (float)pSlot.m_iMaxCount));
        }
        else
        {
            m_pDatas = pDatas;
            SetDirty();
            Paint();
        }
    }

    public void OnClickAllOn()
    {
        if (null == m_pDatas)
            return;

        m_pDatas.ForEach((pData) => pData.m_bIsOn = true);
        ResetDatas(m_pDatas);
    }

    public void OnClickAllOff()
    {
        if (null == m_pDatas)
            return;

        m_pDatas.ForEach((pData) => pData.m_bIsOn = false);
        ResetDatas(m_pDatas);
    }

    public void OnToggleUnit(int iUnitId, bool bIsOn)
    {
        if (null == m_pDatas)
        {
            return;
        }
        
        var pUnit = m_pDatas.Find((pData) => { return pData.m_iUnitId == iUnitId; });
        if (null == pUnit)
        {
            return;
        }

        pUnit.m_bIsOn = bIsOn;
    }
}
