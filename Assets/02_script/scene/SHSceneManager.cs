using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using LitJson;

public class SHSceneManager : SHSingleton<SHSceneManager>
{
    private List<Action<eSceneType>> loadSceneEvents = new List<Action<eSceneType>>();
    
    public override void OnInitialize()
    {
        SetDontDestroy();
    }

    public void LoadScene(eSceneType eType, LoadSceneMode eMode = LoadSceneMode.Single, bool bIsUseFade = false, Action<SHReply> pCallback = null)
    {
        if (null == pCallback)
        {
            pCallback = (SHReply pReply) => { };
        }

        if (true == IsLoadedScene(eType))
        {
            pCallback(new SHReply(new SHError(eErrorCode.Scene_Already_Loaded, "[LSH] Aleady Loaded Scene")));
            return;
        }

        Action LoadScene = () =>
        {
            Single.Coroutine.CachingWait(()=> 
            {
                LoadProcess(SceneManager.LoadSceneAsync(eType.ToString(), eMode), (pAsyncOperation) =>
                {
                    if (true == bIsUseFade)
                        PlayFadeOut(() => pCallback(new SHReply(JsonMapper.ToObject("{}"))));
                    else
                        pCallback(new SHReply(JsonMapper.ToObject("{}")));

                    SendLoadEvent(eType);
                });
            });
        };

        if (true == bIsUseFade)
            PlayFadeIn(() => LoadScene());
        else
            LoadScene();
    }
    
    public void UnloadScene(eSceneType eType)
    {
        if (false == IsLoadedScene(eType))
            return;

        SceneManager.UnloadSceneAsync(eType.ToString());
    }
    
    public void AddEventForLoadedScene(Action<eSceneType> callback)
    {
        if (null == callback)
            return;

        if (true == loadSceneEvents.Contains(callback))
            return;

        loadSceneEvents.Add(callback);
    }

    public void DelEventForLoadedScene(Action<eSceneType> callback)
    {
        if (false == loadSceneEvents.Contains(callback))
            return;

        loadSceneEvents.Remove(callback);
    }

    public bool IsLoadedScene(eSceneType eType)
    {
        return SceneManager.GetSceneByName(eType.ToString()).isLoaded;
    }
    
    public eSceneType GetActiveScene()
    {
        return SHUtils.GetSceneTypeByString(SceneManager.GetActiveScene().name);
    }
    
    private void LoadProcess(AsyncOperation pAsyncInfo, Action<AsyncOperation> pDone)
    {
        Single.Coroutine.Async(() => pDone(pAsyncInfo), pAsyncInfo);
    }
    
    private void SendLoadEvent(eSceneType eLoadScene)
    {
        foreach (var callback in loadSceneEvents)
        {
            callback(eLoadScene);
        }
    }

    private void PlayFadeIn(Action pCallback)
    {
        Single.UI.GetRoot<SHUIRootGlobal>((pUIRoot) => 
        {
            // if (false == pUIRoot.ShowFade("Panel_FadeIn", pCallback))
            {
                if (null != pCallback)
                    pCallback();
            }
        });
    }
    
    private void PlayFadeOut(Action pCallback)
    {
        Single.UI.GetRoot<SHUIRootGlobal>((pUIRoot) => 
        {
            //if (false == uiRoot.ShowFade("Panel_FadeOut", pCallback))
            {
                if (null != pCallback)
                    pCallback();
            }
        });
    }
}