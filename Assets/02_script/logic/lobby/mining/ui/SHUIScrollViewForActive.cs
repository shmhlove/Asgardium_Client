using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUIScrollViewForActive : SHUIMassiveScrollView
{
    List<SHActiveSlotData> m_pDatas = new List<SHActiveSlotData>();

    protected override void OnInitialized()
    {
        SetSlotCount(50);
        SetFocus(0, true);
    }

    protected override void SetSlotData(GameObject go, int index)
    {
        if (null == go)
        {
            return;
        }

        if (0 > index || index >= m_pDatas.Count)
        {
            return;
        }

        go.GetComponent<SHUIScrollSlotForActive>().SetData(m_pDatas[index]);
    }

    public void ResetDatas(List<SHActiveSlotData> pDatas)
    {
        m_pDatas = pDatas;
        SetSlotCount(m_pDatas.Count);
    }
}
