﻿using UnityEngine;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHBusinessLobby_Mining : SHBusinessPresenter
{
    private SHBusinessPresenter m_pPresenters = new SHBusinessPresenter();

    public async override void OnInitialize()
    {
        m_pPresenters.Add(new SHBusinessLobby_Mining_Active());
        m_pPresenters.OnInitialize();

        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pUIPanel = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        pUIPanel.SetEventForChangeMiningTab(OnUIEventForChangeMiningTab);
    }

    public override async void OnEnter()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        OnUIEventForChangeMiningTab(pMining.GetCurrentTab(), eMiningTabType.None);
    }

    public override async void OnLeave()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        OnUIEventForChangeMiningTab(eMiningTabType.None, pMining.GetCurrentTab());
    }

    SHBusinessPresenter GetPresenter(eMiningTabType type)
    {
        switch(type)
        {
            case eMiningTabType.Active:     return m_pPresenters.Get<SHBusinessLobby_Mining_Active>();
            case eMiningTabType.Passive:    return m_pPresenters.Get<SHBusinessPresenter>();
            case eMiningTabType.Company:    return m_pPresenters.Get<SHBusinessPresenter>();
            default:                        return m_pPresenters.Get<SHBusinessPresenter>();
        }
    }

    public void OnUIEventForChangeMiningTab(eMiningTabType eEnter, eMiningTabType eLeave)
    {
        GetPresenter(eEnter).OnEnter();
        GetPresenter(eLeave).OnLeave();
    }
}