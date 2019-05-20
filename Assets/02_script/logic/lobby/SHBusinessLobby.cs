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

    private eLobbyMenuType m_eLobbyMenuType = eLobbyMenuType.None;
    private eMiningStageType m_eCurrentMiningStageType = eMiningStageType.None;

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

        // 소켓 연결
        Single.Network.ConnectWebSocket();
        Single.Network.AddSystemEventObserver(socket.io.SystemEvents.connect, () => 
        {
            switch(m_eLobbyMenuType)
            {
                case eLobbyMenuType.Mining:
                    SetChangeMiningStage(m_eCurrentMiningStageType);
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
                    SetChangeMiningStage(m_eCurrentMiningStageType);
                    break;
            }
        });
        
        // UI 초기화
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);

        // Lobby MainMenu Contact 처리
        var pMenubar = await pUIRoot.GetPanel<SHUIPanelMenubar>(SHUIConstant.PANEL_MENUBAR);
        pMenubar.SetEventOfChangeLobbyMenu(OnEventOfChangeLobbyMenu);
        
        // Mining UI와 Contact 처리        
        m_pUIPanelMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        m_pUIPanelMining.SetEventOfChangeStage(OnEventOfChangeMiningStage);

        // 초기화
        SetChangeMiningStage(eMiningStageType.Active);
        StartCoroutine("CoroutineForMiningActiveInformation");
    }

    public void OnEventOfChangeLobbyMenu(eLobbyMenuType eType)
    {
        m_eLobbyMenuType = eType;

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
