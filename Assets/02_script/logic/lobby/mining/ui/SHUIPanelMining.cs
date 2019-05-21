using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public enum eMiningTabType
{
    None,
    Active,
    Passive,
    Company,
}

public class SHUIPanelMining : SHUIPanel
{
    [Header("Information")]
    public SHUIActiveInformation m_pActiveInfo;

    [Header("ScrollView")]
    public SHUIScrollViewForActive m_pActiveScrollView;

    private Action<eMiningTabType> m_pEventOfChangeStage;

    public void SetEventForChangeStage(Action<eMiningTabType> pCallback)
    {
        m_pEventOfChangeStage = pCallback;
    }

    public void SetActiveInformation(string strPower, string strTimer)
    {
        m_pActiveInfo.SetMiningPower(strPower);
        m_pActiveInfo.SetTimer(strTimer);
    }

    public void SetActiveScrollview(List<SHActiveSlotData> pDatas)
    {
        m_pActiveScrollView.ResetDatas(pDatas);
    }

    public void OnClickActive()
    {
        m_pEventOfChangeStage?.Invoke(eMiningTabType.Active);
    }

    public void OnClickPassive()
    {
        m_pEventOfChangeStage?.Invoke(eMiningTabType.Passive);
    }

    public void OnClickCompany()
    {
        m_pEventOfChangeStage?.Invoke(eMiningTabType.Company);
    }
}
