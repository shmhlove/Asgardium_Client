using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;
using socket.io;
using LunarConsolePlugin;

public partial class SHBusinessLobby : MonoBehaviour
{
    private SHBusinessPresenter m_pPresenters = new SHBusinessPresenter();

    [Header("UI Objects")]
    private SHUIPanelMining  m_pUIPanelMining = null;
    private SHUIPanelStorage m_pUIPanelStorage = null;
    private SHUIPanelUpgrade m_pUIPanelUpgrade = null;

    private Dictionary<string, Action> m_dicEnableMainMenuDelegate = new Dictionary<string, Action>();
    private Dictionary<string, Action> m_dicDisableMainMenuDelegate = new Dictionary<string, Action>();

    private void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    private async void Start()
    {
        m_pPresenters.Add(new SHBusinessLobby_Mining());
        m_pPresenters.OnInitialize();

        // 탭별 시작함수 호출
        StartStorage();
        //StartMarket();
        StartUpgrade();
        //StartMenu();

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
                // 유저 데이터 요청
                pInventory.RequestGetUserInventory((reply) => { });
                pUpgrade.RequestGetUserUpgradeInfo((reply) => { });

                // 소켓 연결 및 이벤트 바인딩
                Single.Network.ConnectWebSocket();
                Single.Network.AddEventObserver(SystemEvents.connect.ToString(), OnEventForSocketReconnect);

                // UI 초기화
                var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);

                // Lobby MainMenu
                var pMenubar = await pUIRoot.GetPanel<SHUIPanelMenubar>(SHUIConstant.PANEL_MENUBAR);
                pMenubar.SetEventForChangeLobbyMenu(OnEventForChangeLobbyMenu);

                // Lobby Mining
                m_pUIPanelMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
                //m_pUIPanelMining.SetEventForChangeMiningTab(OnEventForChangeMiningTab);
                m_pUIPanelMining.SetEventForClickFilter(OnEventForMiningFilter);

                // Lobby Storage
                m_pUIPanelStorage = await pUIRoot.GetPanel<SHUIPanelStorage>(SHUIConstant.PANEL_STORAGE);

                // Lobby Upgrade
                m_pUIPanelUpgrade = await pUIRoot.GetPanel<SHUIPanelUpgrade>(SHUIConstant.PANEL_UPGRADE);
                m_pUIPanelUpgrade.AddEventForActiveUpgrade(gameObject);
                
                // 초기화면설정 : Mining 탭 으로 초기화
                pMenubar.ExecuteClick(eLobbyMenuType.Mining);
                StartCoroutine("CoroutineForUpdateUIForActiveInformation");
            }
        });

        // 디버그 기능
        LunarConsole.RegisterAction("Socket Disconnect", OnClickDebugSocketDisconnect);
        LunarConsole.RegisterAction("Socket SendMessage", OnClickDebugSocketSendMessage);
        LunarConsole.RegisterAction("Socket Mining Subscribe", OnClickDebugSocketMiningSubscribe);
        LunarConsole.RegisterAction("Socket Mining Unsubscribe", OnClickDebugSocketMiningUnubscribe);
        LunarConsole.RegisterAction("Web GetMyInventoryInfo", OnClickDebugGetMyInventoryInfo);
        LunarConsole.RegisterAction("Web GetMyUpgradeInfo", OnClickDebugGetMyUpgradeInfo);
        ////////////////////////////////////////////////////////////////////////////////////
    }

    private void AddEnableDelegate(eLobbyMenuType eMenuType, Action pCallback)
    {
        if (true == m_dicEnableMainMenuDelegate.ContainsKey(eMenuType.ToString()))
        {
            m_dicEnableMainMenuDelegate[eMenuType.ToString()] = pCallback;
        }
        else
        {
            m_dicEnableMainMenuDelegate.Add(eMenuType.ToString(), pCallback);
        }
    }

    private void AddDisableDelegate(eLobbyMenuType eMenuType, Action pCallback)
    {
        if (true == m_dicDisableMainMenuDelegate.ContainsKey(eMenuType.ToString()))
        {
            m_dicDisableMainMenuDelegate[eMenuType.ToString()] = pCallback;
        }
        else
        {
            m_dicDisableMainMenuDelegate.Add(eMenuType.ToString(), pCallback);
        }
    }

    SHBusinessPresenter GetByMenuType(eLobbyMenuType type)
    {
        switch(type)
        {
            case eLobbyMenuType.Mining:     return m_pPresenters.Get<SHBusinessLobby_Mining>();
            case eLobbyMenuType.Storage:    return m_pPresenters.Get<SHBusinessLobby_Mining>();
            case eLobbyMenuType.Market:     return m_pPresenters.Get<SHBusinessLobby_Mining>();
            case eLobbyMenuType.Upgrade:    return m_pPresenters.Get<SHBusinessLobby_Mining>();
            case eLobbyMenuType.Menu:       return m_pPresenters.Get<SHBusinessLobby_Mining>();
            default:                        return m_pPresenters.Get<SHBusinessPresenter>();
        }
    }

    private void OnEventForChangeLobbyMenu(eLobbyMenuType eTo, eLobbyMenuType eFrom)
    {
        if (eTo == eFrom)
        {
            return;
        }

        GetByMenuType(eTo).OnEnter();
        GetByMenuType(eFrom).OnLeave();
    }

    private async void OnEventForSocketReconnect(SHReply pReply)
    {
        // 현재 머물고있는 UI 업데이트
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pMenubar = await pUIRoot.GetPanel<SHUIPanelMenubar>(SHUIConstant.PANEL_MENUBAR);
        OnEventForChangeLobbyMenu(pMenubar.GetCurrentMenu(), eLobbyMenuType.None);
    }
    
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
