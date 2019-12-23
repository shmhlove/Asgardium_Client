using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
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

    public async Task LoadScene(eSceneType eType, LoadSceneMode eMode = LoadSceneMode.Single, bool bIsUseFade = false, Action<SHReply> pCallback = null)
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
                SendEventForBeforeLoad(eType);
                Single.Coroutine.Async(SceneManager.LoadSceneAsync(eType.ToString(), eMode), async (pAsync) => 
                {
                    if (true == bIsUseFade)
                        await PlayFadeOut(() => pCallback(new SHReply()));
                    else
                        pCallback(new SHReply());

                    SendEventForAfterLoad(eType);
                });
            });
        };

        if (true == bIsUseFade)
            await PlayFadeIn(() => LoadScene());
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
    
    private void SendEventForBeforeLoad(eSceneType eLoadScene)
    {
        foreach (var callback in beforeLoadSceneEvents)
        {
            callback(eLoadScene);
        }
    }

    private void SendEventForAfterLoad(eSceneType eLoadScene)
    {
        foreach (var callback in afterLoadSceneEvents)
        {
            callback(eLoadScene);
        }
    }

    private async Task PlayFadeIn(Action pCallback)
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        await pUIRoot.ShowFadePanel(pCallback);
    }
    
    private async Task PlayFadeOut(Action pCallback)
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        await pUIRoot.CloseFadePanel(pCallback);
    }
}