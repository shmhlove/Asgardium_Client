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

    [Header("FilterBar")]
    public SHUIScrollViewForActiveFilterbar m_pActiveFilterbarScrollView;

    [Header("ScrollView")]
    public SHUIScrollViewForActive m_pActiveScrollView;

    private Action<eMiningTabType> m_pEventOfChangeTab;
    private Action m_pEventOfFilter;

    public void SetEventForChangeTab(Action<eMiningTabType> pCallback)
    {
        m_pEventOfChangeTab = pCallback;
    }

    public void SetEventForFilter(Action pCallback)
    {
        m_pEventOfFilter = pCallback;
    }

    public void SetActiveInformation(string strPower, string strTimer)
    {
        m_pActiveInfo.SetMiningPower(strPower);
        m_pActiveInfo.SetTimer(strTimer);
    }

    public void SetActiveFilterbarScrollview(List<SHActiveFilterUnitData> pDatas)
    {
        m_pActiveFilterbarScrollView.ResetDatas(pDatas);
    }

    public void SetActiveScrollview(List<SHActiveSlotData> pDatas)
    {
        m_pActiveScrollView.ResetDatas(pDatas);
    }

    public void OnClickActive()
    {
        m_pEventOfChangeTab?.Invoke(eMiningTabType.Active);
    }

    public void OnClickPassive()
    {
        m_pEventOfChangeTab?.Invoke(eMiningTabType.Passive);
    }

    public void OnClickCompany()
    {
        m_pEventOfChangeTab?.Invoke(eMiningTabType.Company);
    }

    public void OnClickFilter()
    {
        m_pEventOfFilter?.Invoke();
    }
}
