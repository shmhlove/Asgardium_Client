using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHBusinessPatch : MonoBehaviour
{
    void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }
    
    async void Start()
    {
        await Single.Data.Load(eSceneType.Patch, async (pLoadInfo)=>
        {
            if (pLoadInfo.IsSucceed())
            {
                await Single.Scene.LoadScene(eSceneType.Login, pCallback: (pReply) => 
                {

                });
            }
            else
            {
                // 업데이트 실패!! 재시도 유도 처리
            }
        }, 
        (pProgressInfo)=>
        {
            // UI 표현 처리

            // 콘솔로그 처리
            string strFiles = string.Empty;
            pProgressInfo.m_pLoadingDatas.ForEach((pItem) => strFiles += pItem.m_pLoadDataInfo.m_strName + " / ");
            strFiles = string.IsNullOrEmpty(strFiles) ? "empty" : strFiles;

            Debug.LogFormat("Data Load Progress : <color=yellow>{0}%({1}/{2}), Loading Now : {3}</color>",
                            pProgressInfo.GetPercent(),
                            pProgressInfo.m_iLoadDoneCount,
                            pProgressInfo.m_iTotalDataCount,
                            strFiles);
        });
    }
}
