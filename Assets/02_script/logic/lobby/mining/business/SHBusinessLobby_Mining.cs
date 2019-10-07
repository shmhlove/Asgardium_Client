using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public partial class SHBusinessLobby : MonoBehaviour
{
    private Dictionary<string, Action> m_dicMiningMenuEnableDelegate;
    private Dictionary<string, Action> m_dicMiningMenuDisableDelegate;

    private void StartMining()
    {
        m_dicMiningMenuEnableDelegate = new Dictionary<string, Action>
        {
            { eMiningTabType.Active.ToString(), EnableMiningActiveTab }
            //{ eMiningTabType.Passive.ToString(), EnableMiningPassiveTab }
            //{ eMiningTabType.Company.ToString(), EnableMiningCompanyTab }
        };

        m_dicMiningMenuDisableDelegate = new Dictionary<string, Action>
        {
            { eMiningTabType.Active.ToString(), DisableMiningActiveTab },
            //{ eMiningTabType.Passive.ToString(), DisableMiningPassiveTab },
            //{ eMiningTabType.Company.ToString(), DisableMiningCompanyTab }
        };
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
