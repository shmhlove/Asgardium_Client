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
        Single.Data.Load(eSceneType.Update, (pLoadInfo)=>
        {
            if (pLoadInfo.IsSucceed())
            {
                Single.Scene.LoadScene(eSceneType.Login, pCallback: (pReply) => 
                {

                });
            }
            else
            {
                // 재시도 처리
            }
        }, 
        (pProgressInfo)=>
        {
            // UI 표현 처리
            
            Debug.LogFormat("Data Load Progress : <color=red>{0}%({1}/{2})</color>",
                            pProgressInfo.GetPercent(),
                            pProgressInfo.m_iLoadDoneCount,
                            pProgressInfo.m_iTotalDataCount);
        });
    }
}
