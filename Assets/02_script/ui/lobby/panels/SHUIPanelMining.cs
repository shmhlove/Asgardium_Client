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
    [Header("Tabs")]
    public SHUIButton m_pActiveButton;
    public SHUIButton m_pPassiveButton;
    public SHUIButton m_pCompanyButton;

    [Header("Information")]
    public SHUIActiveInformation m_pActiveInfo;

    [Header("FilterBar")]
    public SHUIScrollViewForActiveFilterbar m_pActiveFilterbarScrollView;

    [Header("ScrollView")]
    public SHUIScrollViewForActive m_pActiveScrollView;

    private eMiningTabType m_eCurrentTab = eMiningTabType.None;
    private Action<eMiningTabType, eMiningTabType> m_pEventOfChangeTab;
    private Action m_pEventOfFilterbar;

    public void SetEventForChangeMiningTab(Action<eMiningTabType, eMiningTabType> pCallback)
    {
        m_pEventOfChangeTab = pCallback;
    }

    public void SetEventForClickFilterbar(Action pCallback)
    {
        m_pEventOfFilterbar = pCallback;
    }

    public void SetActiveInformation(string strPower, string strTimer)
    {
        m_pActiveInfo.SetMiningPower(strPower);
        m_pActiveInfo.SetTimer(strTimer);
    }

    public void SetActiveFilterbarScrollview(List<SHActiveFilterUnitData> pDatas, bool bIsAllOn)
    {
        m_pActiveFilterbarScrollView.ResetDatas(pDatas, bIsAllOn);
    }

    public void SetActiveScrollview(List<SHActiveSlotData> pDatas)
    {
        m_pActiveScrollView.ResetDatas(pDatas);
    }

    public void ExecuteClick(eMiningTabType eType)
    {
        switch(eType)
        {
            case eMiningTabType.Active:
                m_pActiveButton.ExecuteClick();
                break;
            case eMiningTabType.Passive:
                m_pPassiveButton.ExecuteClick();
                break;
            case eMiningTabType.Company:
                m_pCompanyButton.ExecuteClick();
                break;
        }
    }

    public eMiningTabType GetCurrentTab()
    {
        return m_eCurrentTab;
    }

    private void SetMoveTab(eMiningTabType eType)
    {
        m_eCurrentTab = eType;
    }

    public void OnClickActive()
    {
        m_pEventOfChangeTab?.Invoke(eMiningTabType.Active, m_eCurrentTab);
        SetMoveTab(eMiningTabType.Active);
    }

    public void OnClickPassive()
    {
        m_pEventOfChangeTab?.Invoke(eMiningTabType.Passive, m_eCurrentTab);
        SetMoveTab(eMiningTabType.Passive);
    }

    public void OnClickCompany()
    {
        m_pEventOfChangeTab?.Invoke(eMiningTabType.Company, m_eCurrentTab);
        SetMoveTab(eMiningTabType.Company);
    }

    public void OnClickFilter()
    {
        m_pEventOfFilterbar?.Invoke();
    }
}
