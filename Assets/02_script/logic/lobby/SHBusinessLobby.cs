using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public partial class SHBusinessLobby : MonoBehaviour
{
    [Header("UI Objects")]
    private SHUIPanelMining m_pUIPanelMining = null;

    void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    async void Start()
    {
        // 로그인 체크 후 테스트 계정으로 로그인 시켜주기
        var pConfigTable = await Single.Table.GetTable<SHTableClientConfig>();
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
        pUserInfo.CheckUserInfoLoadedForDevelop();

        // UI 초기화
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);

        // Lobby MainMenu Contact 처리
        var pMenubar = await pUIRoot.GetPanel<SHUIPanelMenubar>(SHUIConstant.PANEL_MENUBAR);
        pMenubar.SetEventOfChangeLobbyMenu(OnEventOfChangeLobbyMenu);
        
        // Mining UI와 Contact 처리        
        m_pUIPanelMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        m_pUIPanelMining.SetEventOfChangeStage(OnEventOfChangeMiningStage);
    }

    public void OnEventOfChangeLobbyMenu(eLobbyMenuType eType)
    {
        // Mining
        if (eLobbyMenuType.Mining == eType)
        {
            SetEnableMiningMenu();
        }
        if (eLobbyMenuType.Mining != eType)
        {
            SetDisableMiningMenu();
        }

        // Storage
        // Market
        // Upgrade
        // Menu
    }

    [FuncButton]
    public void OnClickDebugSocketDisconnect()
    {
        Single.Network.SendRequestSocket(SHAPIs.SH_SOCKET_REQ_FORCE_DISCONNECT, null, (reply) => 
        {
            //Single.BusinessGlobal.ShowAlertUI(reply);
        });
    }

    [FuncButton]
    public void OnClickDebugSocketSendMessage()
    {
        JsonData json = new JsonData
        {
            ["device_name"] = Single.AppInfo.GetDeviceName()
        };
        Single.Network.SendRequestSocket(SHAPIs.SH_SOCKET_REQ_TEST, json, (reply) => 
        {
            //Single.BusinessGlobal.ShowAlertUI(reply);
        });
    }

    [FuncButton]
    public void OnClickDebugSocketMiningSubscribe()
    {
        RequestSubscribeMiningActiveInfo();
    }

    [FuncButton]
    public void OnClickDebugSocketMiningUnubscribe()
    {
        RequestUnsubscribeMiningActiveInfo();
    }
}
