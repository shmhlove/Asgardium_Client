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


        // 정렬 테스트
        List<KeyValuePair<int,int>> pList = new List<KeyValuePair<int,int>>();
        pList.Add(new KeyValuePair<int, int>(1, 100));
        pList.Add(new KeyValuePair<int, int>(2, 10));
        pList.Add(new KeyValuePair<int, int>(3, 50));
        pList.Add(new KeyValuePair<int, int>(2, 50));
        pList.Add(new KeyValuePair<int, int>(3, 100));
        pList.Add(new KeyValuePair<int, int>(2, 100));
        pList.Add(new KeyValuePair<int, int>(1, 50));
        pList.Add(new KeyValuePair<int, int>(3, 10));
        pList.Add(new KeyValuePair<int, int>(1, 10));

        pList.Sort((x, y) =>
        {
            return (x.Key == y.Key) ? 
                ((x.Value < y.Value) ? 1 : -1) : ((x.Key < y.Key) ? 1 : -1);
        });

        string strBuff = string.Empty;
        foreach (var element in pList)
        {
            strBuff += string.Format("{0} : {1}\n", element.Key, element.Value);
        }
        Debug.Log(strBuff);
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
