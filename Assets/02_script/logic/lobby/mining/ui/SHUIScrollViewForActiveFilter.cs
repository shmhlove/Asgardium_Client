using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUIScrollViewForActiveFilter : SHUIMassiveScrollView
{
    List<SHActiveFilterUnitData> m_pDatas = new List<SHActiveFilterUnitData>();

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

        // 검증필요
        // GetRange 시 EndIndex가 Last보다 클때 어떻게 되는가?
        // m_pDatas 데이터가 GetRange로 가져온 데이터와 공유되는가?

        var pHorizontalBox = go.GetComponent<SHUIHorizontalBox>();
        var iStartIndex = pHorizontalBox.m_iMaxCount * index;
        var pSubDatas = m_pDatas.GetRange(iStartIndex, iStartIndex + pHorizontalBox.m_iMaxCount);

        go.GetComponent<SHUIScrollSlotForActiveFilter>().SetData(pSubDatas);
    }

    public void ResetDatas(List<SHActiveFilterUnitData> pDatas)
    {
        if (m_pDatas.Count != pDatas.Count)
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

    public void OnClick
}
