using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUIScrollViewForActiveFilter : SHUIMassiveScrollView
{
    List<SHActiveFilterUnitData> m_pDatas;

    protected override void SetSlotData(GameObject go, int index)
    {
        if (null == go)
        {
            return;
        }

        if (0 > index)
        {
            return;
        }

        if (null == m_pDatas)
        {
            return;
        }

        var pSlot = go.GetComponent<SHUIScrollSlotForActiveFilter>();
        var iStartIndex = pSlot.m_iMaxCount * index;
        if (iStartIndex >= m_pDatas.Count)
        {
            return;
        }

        var iLength = pSlot.m_iMaxCount;
        if ((iStartIndex + iLength) > m_pDatas.Count)
        {
            iLength = m_pDatas.Count - iStartIndex;
        }

        Debug.LogFormat("StartIndex : {0}, Length : {1}", iStartIndex, iLength);
        pSlot.SetData(m_pDatas.GetRange(iStartIndex, iLength), OnToggleUnit);
    }

    public void ResetDatas(List<SHActiveFilterUnitData> pDatas)
    {
        if ((null == m_pDatas) || 
            (m_pDatas.Count != pDatas.Count))
        {
            m_pDatas = pDatas;
            SetSlotCount(m_pDatas.Count);
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
