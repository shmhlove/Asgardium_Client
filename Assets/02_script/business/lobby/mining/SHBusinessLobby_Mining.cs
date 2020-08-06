using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHBusinessLobby_Mining : SHBusinessPresenter
{
    private SHBusinessPresenter m_pPresenters = new SHBusinessPresenter();

    public override void OnInitialize()
    {
        m_pPresenters.Add(new SHBusinessLobby_Mining_Active());
        m_pPresenters.OnInitialize();
    }

    public override async void OnEnter()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);

        OnEventForChangeMiningTab(pMining.GetCurrentTab(), eMiningTabType.None);
    }

    public override async void OnLeave()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);

        OnEventForChangeMiningTab(eMiningTabType.None, pMining.GetCurrentTab());
    }

    SHBusinessPresenter GetByMiningTabType(eMiningTabType type)
    {
        
        switch(type)
        {
            case eMiningTabType.Active:     return m_pPresenters.Get<SHBusinessLobby_Mining_Active>();
            case eMiningTabType.Passive:    return m_pPresenters.Get<SHBusinessLobby_Mining_Active>();
            case eMiningTabType.Company:    return m_pPresenters.Get<SHBusinessLobby_Mining_Active>();
            default:                        return m_pPresenters.Get<SHBusinessPresenter>();
        }
    }

    public void OnEventForChangeMiningTab(eMiningTabType eTo, eMiningTabType eFrom)
    {
        if (eTo == eFrom)
        {
            return;
        }

        GetByMiningTabType(eTo).OnEnter();
        GetByMiningTabType(eFrom).OnLeave();
    }
}