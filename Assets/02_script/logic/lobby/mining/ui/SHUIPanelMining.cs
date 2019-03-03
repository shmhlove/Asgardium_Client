using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelMining : SHUIPanel
{
    [Header("Information")]
    public SHUIActiveInformation m_pActiveInfo;

    [Header("ScrollView")]
    public SHUIScrollViewForActive m_pActiveScrollView;

    public void SetActiveInformation(string strPower, string strTimer)
    {
        m_pActiveInfo.SetMiningPower(strPower);
        m_pActiveInfo.SetTimer(strTimer);
    }

    public void SetActiveScrollview(List<SHActiveSlotData> pDatas)
    {
        m_pActiveScrollView.ResetDatas(pDatas);
    }
}
