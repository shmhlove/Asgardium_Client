using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

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
    public void OnClickDebugSocketConnect()
    {
        // 소켓 통신 해보자
        Single.Network.ConnectWebSocket((reply) => 
        {
            Single.BusinessGlobal.ShowAlertUI(reply);
        });
    }
}
