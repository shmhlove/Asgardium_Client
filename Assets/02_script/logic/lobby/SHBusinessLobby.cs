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
    private SHUIPanelMiningSubActiveCompany m_pUIPanelMiningSubActiveCompany = null;

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
        var pInventory = await Single.Table.GetTable<SHTableServerInventoryInfo>();
        pUserInfo.RequestGetUserInfoForDevelop((userInfoReply) =>
        {
            pInventory.RequestGetInventoryInfo(pUserInfo.UserId, (inventoryReply) => { });
        });

        // 소켓 연결 및 이벤트 바인딩
        Single.Network.ConnectWebSocket();
        Single.Network.AddEventObserver(SystemEvents.connect.ToString(), OnEventForSocketReconnect);
        Single.Network.AddEventObserver(SHAPIs.SH_SOCKET_POLLING_MINING_ACTIVE_INFO, OnEventForSocketPollingMiningActiveInfo);
        
        // UI 초기화
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);

        // Lobby MainMenu 이벤트 바인딩
        var pMenubar = await pUIRoot.GetPanel<SHUIPanelMenubar>(SHUIConstant.PANEL_MENUBAR);
        pMenubar.SetEventForChangeLobbyMenu(OnEventForChangeLobbyMenu);
        
        // MiningTab UIs 로드 및 이벤트 바인딩
        m_pUIPanelMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        m_pUIPanelMining.SetEventForChangeTab(OnEventForChangeMiningTab);
        m_pUIPanelMining.SetEventForFilter(OnEventForMiningFilter);
        m_pUIPanelMiningSubActiveCompany = await pUIRoot.GetPanel<SHUIPanelMiningSubActiveCompany>(SHUIConstant.PANEL_MINING_SUB_ACTIVE_COMPANY);

        // 초기화면설정 : Mining Tab 초기화
        m_eCurrentLobbyMenuType = eLobbyMenuType.Mining;
        SetChangeMiningTab(eMiningTabType.Active);
        StartCoroutine("CoroutineForUpdateUIForActiveInformation");
    }

    private void OnEventForChangeLobbyMenu(eLobbyMenuType eType)
    {
        // On
        if ((eLobbyMenuType.Mining == eType) 
            && (eLobbyMenuType.Mining != m_eCurrentLobbyMenuType)) {
            var currentMiningTabType = m_eCurrentMiningTabType;
            m_eCurrentMiningTabType = eMiningTabType.None;
            SetChangeMiningTab(currentMiningTabType);
        }

        // Off
        if ((eLobbyMenuType.Mining != eType) 
            && (eLobbyMenuType.Mining == m_eCurrentLobbyMenuType)) {
            SetChangeMiningTab(eMiningTabType.None);
        }
        
        m_eCurrentLobbyMenuType = eType;
    }

    private async void OnEventForMiningFilter()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pPanel = await pUIRoot.GetPanel<SHUIPanelMiningActiveUnitFilter>(SHUIConstant.PANEL_MINING_FILTER);
        pPanel.Show(null);
    }

    private void OnEventForSocketReconnect(SHReply pReply)
    {
        switch (m_eCurrentLobbyMenuType)
        {
            case eLobbyMenuType.Mining:
                var currentMiningTabType = m_eCurrentMiningTabType;
                m_eCurrentMiningTabType = eMiningTabType.None;
                SetChangeMiningTab(currentMiningTabType);
                break;
            default:
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

    [FuncButton]
    public async void OnClickDebugGetMyInventoryInfo()
    {
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
        var pInventoryInfo = await Single.Table.GetTable<SHTableServerInventoryInfo>();
        
        pInventoryInfo.RequestGetInventoryInfo(pUserInfo.UserId, (reply) => 
        {
            Single.BusinessGlobal.ShowAlertUI(reply);
            pInventoryInfo.LoadJsonTable(reply.data);
        });
    }
}
