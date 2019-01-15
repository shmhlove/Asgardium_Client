using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class SHSceneManager : SHSingleton<SHSceneManager>
{
    public override void OnInitialize()
    {
        SetDontDestroy();
    }

    public void Addtive(eSceneType eType, bool bIsUseFade = false, Action<SHReply> pCallback = null)
    {
        if (null == pCallback)
            pCallback = (SHReply pReply) => { };

        if (true == IsLoadedScene(eType))
        {
            pCallback(new SHReply(new SHError(eErrorCode.Scene_Already_Loaded, "Aleady Loaded Scene")));
            return;
        }

        Action LoadScene = () =>
        {
            Single.Coroutine.CachingWait(()=> 
            {
            //     Single.Firebase.Storage.DownloadForBundle(eType, (pReply) =>
            //     {
            //         if (false == pReply.IsSucceed)
            //         {
            //             pCallback(new SHReply(pReply.Error));
            //             return;
            //         }

            //         var pAsReply  = pReply.GetAs<Firebase.Storage.SHReplyDownloadForBundle>();
            //         var strScenes = pAsReply.m_pWWW.assetBundle.GetAllScenePaths();
            //         var strLoadScenePath = string.Empty;
            //         foreach (string strScene in strScenes)
            //         {
            //             if (true == strScene.Contains(eType.ToString()))
            //             {
            //                 strLoadScenePath = strScene;
            //                 break;
            //             }
            //         }
                    
            //         if (true == string.IsNullOrEmpty(strLoadScenePath))
            //         {
            //             Debug.LogErrorFormat("[LSH] Scene bundle Not matching of name");
            //             pCallback(new SHReply(new SHError(eErrorCode.Failed, "Scene bundle Not matching of name")));
            //             return;
            //         }
                    
            //         LoadProcess(SceneManager.LoadSceneAsync(strLoadScenePath, LoadSceneMode.Additive), (pAsyncOperation) =>
            //         {
            //             pAsReply.m_pWWW.assetBundle.Unload(false);
            //             pCallback(new SHReply(new SHError(eErrorCode.Failed, "Scene bundle Not matching of name")));
            //             if (true == bIsUseFade)
            //                 PlayFadeOut(() => pCallback(new SHReply()));
            //             else
            //                 pCallback(new SHReply());
                    
            //             CallEventOfAddtiveScene(eType);
            //         });
            //     });
            });
        };

        if (true == bIsUseFade)
            PlayFadeIn(() => LoadScene());
        else
            LoadScene();
    }
    
    public void Remove(eSceneType eType)
    {
        if (false == IsLoadedScene(eType))
            return;

        SceneManager.UnloadSceneAsync(eType.ToString());
    }
    
    public bool IsLoadedScene(eSceneType eType)
    {
        return SceneManager.GetSceneByName(eType.ToString()).isLoaded;
    }
    
    public eSceneType GetActiveScene()
    {
        return SHHard.GetSceneTypeByString(SceneManager.GetActiveScene().name);
    }
    
    void LoadProcess(AsyncOperation pAsyncInfo, Action<AsyncOperation> pDone)
    {
        Single.Coroutine.Async(() => pDone(pAsyncInfo), pAsyncInfo);
    }
    
    void PlayFadeIn(Action pCallback)
    {
        //if (false == Single.UI.Show("Panel_FadeIn", pCallback))
        {
            if (null != pCallback)
                pCallback();
        }

        //SHCoroutine.Instance.NextUpdate(() => Single.UI.Close("Panel_FadeOut"));
    }
    
    void PlayFadeOut(Action pCallback)
    {
        //if (false == Single.UI.Show("Panel_FadeOut", pCallback))
        {
            if (null != pCallback)
                pCallback();
        }

        //SHCoroutine.Instance.NextUpdate(() => Single.UI.Close("Panel_FadeIn"));
    }
}