﻿using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;
using socket.io;

public partial class SHBusinessLobby : MonoBehaviour
{
    [Header("UI Objects")]
    private SHUIPanelMining  m_pUIPanelMining = null;
    private SHUIPanelStorage m_pUIPanelStorage = null;

    private eLobbyMenuType m_eCurrentLobbyMenuType = eLobbyMenuType.None;
    private eMiningTabType m_eCurrentMiningTabType = eMiningTabType.None;

    void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    async void Start()
    {
        // 테이블 정보 얻기
        var pConfigTable = await Single.Table.GetTable<SHTableClientConfig>();
        var pUserInfo    = await Single.Table.GetTable<SHTableUserInfo>();
        var pInventory   = await Single.Table.GetTable<SHTableServerInventoryInfo>();

        // 개발용 : 로그인 체크 후 테스트 계정으로 로그인 시켜주기
        // UserInfo가 로드되었는지 확인하고 있기때문에 실제 배포시에도 이 코드는 유지해도 된다.
        ////////////////////////////////////////////////////////////////////////////////////
        pUserInfo.RequestGetUserInfoForDevelop((userInfoReply) =>
        {
            pInventory.RequestGetInventoryInfo(pUserInfo.UserId, (inventoryReply) => { });
        });
        ////////////////////////////////////////////////////////////////////////////////////
        
        // 소켓 연결 및 이벤트 바인딩
        Single.Network.ConnectWebSocket();
        Single.Network.AddEventObserver(SystemEvents.connect.ToString(), OnEventForSocketReconnect);
        Single.Network.AddEventObserver(SHAPIs.SH_SOCKET_POLLING_MINING_ACTIVE_INFO, OnEventForSocketPollingMiningActiveInfo);
        
        // UI 초기화
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);

        // Lobby MainMenu 이벤트 바인딩
        var pMenubar = await pUIRoot.GetPanel<SHUIPanelMenubar>(SHUIConstant.PANEL_MENUBAR);
        pMenubar.SetEventForChangeLobbyMenu(OnEventForChangeLobbyMenu);
        
        // Lobby Mining UIs 로드 및 이벤트 바인딩
        m_pUIPanelMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        m_pUIPanelMining.SetEventForChangeTab(OnEventForChangeMiningTab);
        m_pUIPanelMining.SetEventForFilter(OnEventForMiningFilter);

        // Lobby Storage UIs 로드 및 이벤트 바인딩
        m_pUIPanelStorage = await pUIRoot.GetPanel<SHUIPanelStorage>(SHUIConstant.PANEL_STORAGE);
        
        // 초기화면설정 : Mining Tab 초기화
        m_eCurrentLobbyMenuType = eLobbyMenuType.Mining;
        SetChangeMiningTab(eMiningTabType.Active);
        StartCoroutine("CoroutineForUpdateUIForActiveInformation");
    }

    private void OnEventForChangeLobbyMenu(eLobbyMenuType eType)
    {
        // @@ 리팩토링이 필요하다.
        // 조건 분기문도 마음에 안들고, Mining 탭 처리과정도 마음에 안듬

        // On
        if ((eLobbyMenuType.Mining == eType) && (eLobbyMenuType.Mining != m_eCurrentLobbyMenuType)) {
            var currentMiningTabType = m_eCurrentMiningTabType;
            m_eCurrentMiningTabType = eMiningTabType.None;
            SetChangeMiningTab(currentMiningTabType);
        }
        
        // On
        if ((eLobbyMenuType.Storage == eType) && (eLobbyMenuType.Storage != m_eCurrentLobbyMenuType)) {
            ResetStorage();
        }

        // Off
        if ((eLobbyMenuType.Mining != eType) && (eLobbyMenuType.Mining == m_eCurrentLobbyMenuType)) {
            SetChangeMiningTab(eMiningTabType.None);
        }
        
        m_eCurrentLobbyMenuType = eType;
    }

    private async void OnEventForMiningFilter()
    {
        // @@ 필터오픈코드가 여기 있는게 맞나??
        // 마이닝에 있어야될듯해보이는디??

        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pPanel = await pUIRoot.GetPanel<SHUIPopupPanelMiningActiveUnitFilter>(SHUIConstant.PANEL_MINING_FILTER);

        // 필터링 대상유닛 데이터 생성
        var pUnitDatas = new List<SHActiveFilterUnitData>();
        var pUnitTable = await Single.Table.GetTable<SHTableServerGlobalUnitData>();
        foreach (var kvp in pUnitTable.m_dicDatas)
        {
            var pData = new SHActiveFilterUnitData();
            pData.m_iUnitId = kvp.Value.m_iUnitId;
            pData.m_strIconImage = kvp.Value.m_strIconImage;

            var bIsOn = SHPlayerPrefs.GetBool(kvp.Value.m_iUnitId.ToString());
            pData.m_bIsOn = (null == bIsOn) ? true : bIsOn.Value;

            pUnitDatas.Add(pData);
        }
        
        // 필터링 UI가 닫힐때 PlayerPreb에 셋팅하고, UI를 업데이트 한다.
        Action<List<SHActiveFilterUnitData>> pCloseEvent = (pDatas) =>
        {
            foreach (var pData in pDatas)
            {
                SHPlayerPrefs.SetBool(pData.m_iUnitId.ToString(), pData.m_bIsOn);
            }

            UpdateUIForActiveCompany(() => { });
            UpdateUIForActiveFilterbar();
        };
        
        // 필터링 UI Open
        pPanel.Show(pUnitDatas, pCloseEvent);
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
    
    // @@ 디버그기능은 추후에 루나콘솔로 옮기자!!
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
