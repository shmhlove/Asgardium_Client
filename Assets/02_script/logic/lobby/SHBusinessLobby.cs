using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;
using socket.io;

public partial class SHBusinessLobby : MonoBehaviour
{
    [Header("UI Objects")]
    private SHUIPanelMining m_pUIPanelMining = null;

    private eLobbyMenuType m_eCurrentLobbyMenuType = eLobbyMenuType.None;
    private eMiningTabType m_eCurrentMiningTabType = eMiningTabType.None;

    void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    async void Start()
    {
        // 개발용 : 로그인 체크 후 테스트 계정으로 로그인 시켜주기
        var pConfigTable = await Single.Table.GetTable<SHTableClientConfig>();
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
        pUserInfo.SetUserInfoForDevelop();

        // 소켓 연결 및 이벤트 바인딩
        Single.Network.ConnectWebSocket();
        Single.Network.AddEventObserver(SystemEvents.connect.ToString(), OnEventForSocketReconnect);
        Single.Network.AddEventObserver(SHAPIs.SH_SOCKET_POLLING_MINING_ACTIVE_INFO, OnEventForSocketPollingMiningActiveInfo);
        
        // UI 초기화
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);

        // Lobby MainMenu 이벤트 바인딩
        var pMenubar = await pUIRoot.GetPanel<SHUIPanelMenubar>(SHUIConstant.PANEL_MENUBAR);
        pMenubar.SetEventForChangeLobbyMenu(OnEventForChangeLobbyMenu);
        
        // Mining Tab UI 이벤트 바인딩
        m_pUIPanelMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        m_pUIPanelMining.SetEventForChangeStage(OnEventForChangeMiningTab);

        // 초기화면 : Mining Tab 초기화
        SetChangeMiningStage(eMiningTabType.Active);
        // 소켓 폴링 연결되면 이코드 지워야한다.
        StartCoroutine("CoroutineForMiningActiveInformation");
    }

    private void OnEventForChangeLobbyMenu(eLobbyMenuType eType)
    {
        m_eCurrentLobbyMenuType = eType;

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

    private void OnEventForSocketReconnect(SHReply pReply)
    {
        switch(m_eCurrentLobbyMenuType)
        {
            case eLobbyMenuType.Mining:
                SetChangeMiningStage(m_eCurrentMiningTabType);
                break;
            case eLobbyMenuType.Storage:
                break;
            case eLobbyMenuType.Market:
                break;
            case eLobbyMenuType.Upgrade:
                break;
            case eLobbyMenuType.Menu:
                break;
            default:
                SetChangeMiningStage(m_eCurrentMiningTabType);
                break;
        }
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
