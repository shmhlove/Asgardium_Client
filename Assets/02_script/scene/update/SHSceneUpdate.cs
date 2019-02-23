using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHSceneUpdate : MonoBehaviour
{
    void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    void Start()
    {
        UpdateData();
    }

    void UpdateData()
    {
        Single.Network.GET(SHAPIs.SH_API_GET_CONFIG, null, async (reply) => 
        {
            if (reply.isSucceed)
            {
                var pConfig = await Single.Table.GetTable<JsonServerConfig>();
                pConfig.LoadJsonTable(reply.data);

                Single.Scene.LoadScene(eSceneType.Login, pCallback: (pReply) => 
                {

                });
            }
            else
            {
                var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
                pUIRoot.ShowAlert("Config 테이블 다운로드 실패!!");
            }
        });
    }
}
