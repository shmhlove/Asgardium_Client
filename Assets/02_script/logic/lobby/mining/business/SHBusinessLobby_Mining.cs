using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public partial class SHBusinessLobby : MonoBehaviour
{
    private Dictionary<string, Action> m_dicMiningMenuEnableDelegate = new Dictionary<string, Action>();
    private Dictionary<string, Action> m_dicMiningMenuDisableDelegate = new Dictionary<string, Action>();

    private void StartMining()
    {
        AddEnableDelegate(eLobbyMenuType.Mining, EnableMiningMenu);
        AddDisableDelegate(eLobbyMenuType.Mining, DisableMiningMenu);
        AddEnableMiningTabDelegate(eMiningTabType.Active, EnableMiningActiveTab);
        AddDisableMiningTabDelegate(eMiningTabType.Active, DisableMiningActiveTab);
    }

    private async void EnableMiningMenu()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        
        OnEventForChangeMiningTab(pMining.GetCurrentTab(), eMiningTabType.None);
    }

    private async void DisableMiningMenu()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);

        OnEventForChangeMiningTab(eMiningTabType.None, pMining.GetCurrentTab());
    }

    private void AddEnableMiningTabDelegate(eMiningTabType eTabType, Action pCallback)
    {
        if (true == m_dicMiningMenuEnableDelegate.ContainsKey(eTabType.ToString()))
        {
            m_dicMiningMenuEnableDelegate[eTabType.ToString()] = pCallback;
        }
        else
        {
            m_dicMiningMenuEnableDelegate.Add(eTabType.ToString(), pCallback);
        }
    }

    private void AddDisableMiningTabDelegate(eMiningTabType eTabType, Action pCallback)
    {
        if (true == m_dicMiningMenuDisableDelegate.ContainsKey(eTabType.ToString()))
        {
            m_dicMiningMenuDisableDelegate[eTabType.ToString()] = pCallback;
        }
        else
        {
            m_dicMiningMenuDisableDelegate.Add(eTabType.ToString(), pCallback);
        }
    }

    public void OnEventForChangeMiningTab(eMiningTabType eTo, eMiningTabType eFrom)
    {
        if (eTo == eFrom)
        {
            return;
        }

        if (m_dicMiningMenuEnableDelegate.ContainsKey(eTo.ToString()))
        {
            m_dicMiningMenuEnableDelegate[eTo.ToString()]();
        }

        if (m_dicMiningMenuDisableDelegate.ContainsKey(eFrom.ToString()))
        {
            m_dicMiningMenuDisableDelegate[eFrom.ToString()]();
        }
    }
}
