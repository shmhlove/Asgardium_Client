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

        // 필요한 테이블 Get해서 로드되어 있도록
        var pOracleCompanyAM = await Single.Table.GetTable<SHTableServerOracleCompanyAM>();

        // UI와 Contact 처리
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        m_pUIPanelMining = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        m_pUIPanelMining.SetEventOfChangeStage(OnEventOfChangeMiningStage);
        OnEventOfChangeMiningStage(eMiningStageType.Active);
    }

    private void OnEnable()
    {
        StartCoroutine(CoroutineForMiningActiveInformation());
    }

    private void OnDisable()
    {
        StopCoroutine(CoroutineForMiningActiveInformation());
    }
}
