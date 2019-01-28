using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using LitJson;

public class SHSceneManager : SHSingleton<SHSceneManager>
{
    private List<Action<eSceneType>> beforeLoadSceneEvents = new List<Action<eSceneType>>();
    private List<Action<eSceneType>> afterLoadSceneEvents = new List<Action<eSceneType>>();

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
                SendEventToBeforeLoad(eType);
                Single.Coroutine.Async(SceneManager.LoadSceneAsync(eType.ToString(), eMode), (pAsync) => 
                {
                    if (true == bIsUseFade)
                        PlayFadeOut(() => pCallback(new SHReply()));
                    else
                        pCallback(new SHReply());

                    SendEventToAfterLoad(eType);
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
    
    public void AddEventForBeforeLoadScene(Action<eSceneType> callback)
    {
        if (null == callback)
            return;

        if (true == beforeLoadSceneEvents.Contains(callback))
            return;

        beforeLoadSceneEvents.Add(callback);
    }

    public void DelEventForBeforeLoadScene(Action<eSceneType> callback)
    {
        if (null == callback)
            return;

        if (false == beforeLoadSceneEvents.Contains(callback))
            return;

        beforeLoadSceneEvents.Remove(callback);
    }

    public void AddEventForAfterLoadScene(Action<eSceneType> callback)
    {
        if (null == callback)
            return;

        if (true == afterLoadSceneEvents.Contains(callback))
            return;

        afterLoadSceneEvents.Add(callback);
    }

    public void DelEventForAfterLoadScene(Action<eSceneType> callback)
    {
        if (null == callback)
            return;

        if (false == afterLoadSceneEvents.Contains(callback))
            return;

        afterLoadSceneEvents.Remove(callback);
    }

    public bool IsLoadedScene(eSceneType eType)
    {
        return SceneManager.GetSceneByName(eType.ToString()).isLoaded;
    }
    
    public eSceneType GetActiveScene()
    {
        return SHUtils.GetSceneTypeByString(SceneManager.GetActiveScene().name);
    }
    
    private void SendEventToBeforeLoad(eSceneType eLoadScene)
    {
        foreach (var callback in beforeLoadSceneEvents)
        {
            callback(eLoadScene);
        }
    }

    private void SendEventToAfterLoad(eSceneType eLoadScene)
    {
        foreach (var callback in afterLoadSceneEvents)
        {
            callback(eLoadScene);
        }
    }

    private void PlayFadeIn(Action pCallback)
    {
        Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL, (pUIRoot) => 
        {
            pUIRoot.ShowFadePanel(pCallback);
        });
    }
    
    private void PlayFadeOut(Action pCallback)
    {
        Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL, (pUIRoot) => 
        {
            pUIRoot.CloseFadePanel(pCallback);
        });
    }
}