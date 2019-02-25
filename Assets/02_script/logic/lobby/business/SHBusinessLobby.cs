using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHBusinessLobby : MonoBehaviour
{
    [Header("UI Objects")]
    private SHUIPanelMining m_pUIPanelMining = null;

    private void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    private async void Start()
    {
        // 로그인 체크 후 테스트 계정으로 로그인 시켜주기
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
        pUserInfo.CheckUserInfoLoadedForDevelop();

        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        m_pUIPanelMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        
        // 필터 이벤트를 받아 Active를 업데이트 시켜줘야한다.
        // 버튼이벤트를 받아 각 Object별 업데이트를 시켜줘야한다.
    }

    private void OnEnable()
    {
        StartCoroutine(CoroutineForMiningActive());
    }

    private void OnDisable()
    {
        StopCoroutine(CoroutineForMiningActive());
    }
}
