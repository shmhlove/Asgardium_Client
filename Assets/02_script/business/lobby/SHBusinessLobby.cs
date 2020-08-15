using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;
using socket.io;
using LunarConsolePlugin;

public partial class SHBusinessLobby : MonoBehaviour
{
    private SHBusinessPresenter m_pPresenters = new SHBusinessPresenter();

    private void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    private async void Start()
    {
        m_pPresenters.Add(new SHBusinessLobby_Mining());
        m_pPresenters.Add(new SHBusinessLobby_Storage());
        m_pPresenters.Add(new SHBusinessLobby_Upgrade());

        // 테이블 정보 얻기
        var pConfigTable = await Single.Table.GetTable<SHTableClientConfig>();
        var pUserInfo    = await Single.Table.GetTable<SHTableUserInfo>();
        var pInventory   = await Single.Table.GetTable<SHTableServerUserInventory>();
        var pUpgrade     = await Single.Table.GetTable<SHTableServerUserUpgradeInfo>();
        
        // 개발용 : 로그인 체크 후 테스트 계정으로 로그인 시켜주기
        // 이 코드에 진입하기 위해서는 로그인 이후 진입된다.
        // 즉, 로그인이 되어 있다면 실제 유저의 정보가 넘어오기 때문에 실제 배포시에도 이 코드는 유지해도 된다.
        ////////////////////////////////////////////////////////////////////////////////////
        pUserInfo.RequestLoginForDevelop(async (signinReply) =>
        {
            if (false == signinReply.isSucceed)
            {
                Single.Global.GetAlert().Show(signinReply, (action) =>
                {
                    SHUtils.GameQuit();
                });
            }
            else
            {
                // 유저 데이터 요청 (인벤토리와 업그레이드 정보)
                pInventory.RequestGetUserInventory((reply) => { });
                pUpgrade.RequestGetUserUpgradeInfo((reply) => { });

                // 소켓 연결 및 이벤트 바인딩
                Single.Network.ConnectWebSocket();
                Single.Network.AddEventObserver(SystemEvents.connect.ToString(), OnSocketEventForReconnect);

                // Lobby MainMenu UI 이벤트 바인딩
                var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
                var pMenubar = await pUIRoot.GetPanel<SHUIPanelMenubar>(SHUIConstant.PANEL_MENUBAR);
                pMenubar.SetEventForChangeLobbyMenu(OnUIEventForChangeLobbyMenu);
                
                // Presenter 초기화
                m_pPresenters.OnInitialize();

                // 초기화면설정 : Mining 탭 으로 초기화
                pMenubar.ExecuteClick(eLobbyMenuType.Mining);
            }
        });

        // 디버그 기능
        ////////////////////////////////////////////////////////////////////////////////////
        LunarConsole.RegisterAction("Socket Disconnect", OnClickDebugSocketDisconnect);
        LunarConsole.RegisterAction("Socket SendMessage", OnClickDebugSocketSendMessage);
        LunarConsole.RegisterAction("Socket Mining Subscribe", OnClickDebugSocketMiningSubscribe);
        LunarConsole.RegisterAction("Socket Mining Unsubscribe", OnClickDebugSocketMiningUnubscribe);
        LunarConsole.RegisterAction("Web GetMyInventoryInfo", OnClickDebugGetMyInventoryInfo);
        LunarConsole.RegisterAction("Web GetMyUpgradeInfo", OnClickDebugGetMyUpgradeInfo);
        ////////////////////////////////////////////////////////////////////////////////////
    }

    SHBusinessPresenter GetByMenuType(eLobbyMenuType type)
    {
        switch(type)
        {
            case eLobbyMenuType.Mining:     return m_pPresenters.Get<SHBusinessLobby_Mining>();
            case eLobbyMenuType.Storage:    return m_pPresenters.Get<SHBusinessLobby_Storage>();
            case eLobbyMenuType.Market:     return m_pPresenters.Get<SHBusinessPresenter>();
            case eLobbyMenuType.Upgrade:    return m_pPresenters.Get<SHBusinessLobby_Upgrade>();
            case eLobbyMenuType.Menu:       return m_pPresenters.Get<SHBusinessPresenter>();
            default:                        return m_pPresenters.Get<SHBusinessPresenter>();
        }
    }

    private void OnUIEventForChangeLobbyMenu(eLobbyMenuType eEnter, eLobbyMenuType eLeave)
    {
        if (eEnter == eLeave)
        {
            return;
        }

        GetByMenuType(eEnter).OnEnter();
        GetByMenuType(eLeave).OnLeave();
    }

    private async void OnSocketEventForReconnect(SHReply pReply)
    {
        // 현재 머물고있는 UI 업데이트
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pMenubar = await pUIRoot.GetPanel<SHUIPanelMenubar>(SHUIConstant.PANEL_MENUBAR);
        OnUIEventForChangeLobbyMenu(pMenubar.GetCurrentMenu(), eLobbyMenuType.None);
    }
    

    //////////////////////////////////////////////////////////////////////
    // 테스트 코드
    //////////////////////////////////////////////////////////////////////
    [FuncButton]
    public void OnClickDebugSocketDisconnect()
    {
        Single.Network.SendRequestSocket(SHAPIs.SH_SOCKET_REQ_FORCE_DISCONNECT, null, (reply) => 
        {
            //Single.Global.GetAlert().Show(reply);
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
            //Single.Global.GetAlert().Show(reply);
        });
    }

    [FuncButton]
    public void OnClickDebugSocketMiningSubscribe()
    {
        //RequestSubscribeMiningActiveInfo();
    }

    [FuncButton]
    public void OnClickDebugSocketMiningUnubscribe()
    {
        //RequestUnsubscribeMiningActiveInfo();
    }

    [FuncButton]
    public async void OnClickDebugGetMyInventoryInfo()
    {
        var pInventoryInfo = await Single.Table.GetTable<SHTableServerUserInventory>();
        pInventoryInfo.RequestGetUserInventory((reply) => 
        {
            Single.Global.GetAlert().Show(reply);
        });
    }

    [FuncButton]
    public async void OnClickDebugGetMyUpgradeInfo()
    {
        var UpgradeInfo = await Single.Table.GetTable<SHTableServerUserUpgradeInfo>();
        UpgradeInfo.RequestGetUserUpgradeInfo((reply) =>
        {
            Single.Global.GetAlert().Show(reply);
        });
    }
}
