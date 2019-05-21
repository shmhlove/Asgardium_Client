using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHCoroutine : SHSingleton<SHCoroutine>
{
    public override void OnInitialize()
    {
        SetDontDestroy();
        Single.Scene.AddEventForBeforeLoadScene(OnEventForLoadedScene);
    }

    public override void OnFinalize()
    {
        StopAllCoroutines();
        Single.Scene.DelEventForBeforeLoadScene(OnEventForLoadedScene);
    }

    public void OnEventForLoadedScene(eSceneType eType)
    {
        StopAllCoroutines();
    }
    
    // yield return null : 다음 Update까지 대기
    //-----------------------------------------------
    public void NextUpdate(Action pAction)
    {
        if (null == pAction)
        {
            pAction = () => {};
        }

        StartCoroutine(InvokeToNextUpdate(pAction));
    }
    private IEnumerator InvokeToNextUpdate(Action pAction)
    {
        yield return null;

        pAction.Invoke();
    }


    // yield return new WaitForFixedUpdate() : 다음 FixedUpdate까지 대기
    //-----------------------------------------------
    public void NextFixedUpdate(Action pAction)
    {
        if (null == pAction)
        {
            pAction = () => {};
        }

        StartCoroutine(InvokeToNextFixedUpdate(pAction));
    }
    private IEnumerator InvokeToNextFixedUpdate(Action pAction)
    {
        yield return new WaitForFixedUpdate();

        pAction.Invoke();
    }
    

    // yield return new WaitForEndOfFrame() : 렌더링 작업이 끝날 때 까지 대기
    //-----------------------------------------------
    public void EndOfFrame(Action pAction)
    {
        if (null == pAction)
        {
            pAction = () => {};
        }

        StartCoroutine(InvokeToEndOfFrame(pAction));
    }
    private IEnumerator InvokeToEndOfFrame(Action pAction)
    {
        yield return new WaitForEndOfFrame();

        pAction.Invoke();
    }
    

    // yield return new WaitForSeconds : 지정한 시간까지 대기
    //-----------------------------------------------
    public void WaitTime(float fDelay, Action pAction)
    {
        if (null == pAction)
        {
            pAction = () => {};
        }

        StartCoroutine(InvokeToWaitTime(fDelay, pAction));
    }
    private IEnumerator InvokeToWaitTime(float fDelay, Action pAction)
    {
        if (0.0f >= fDelay)
            yield return null;
        else
            yield return new WaitForSeconds(fDelay);

        pAction.Invoke();
    }
    

    //yield return new WWW(string) : 웹 통신 작업이 끝날 때까지 대기
    //-----------------------------------------------
    public UnityWebRequest WWW(UnityWebRequest pWWW, Action<UnityWebRequest> pAction)
    {
        if (null == pAction)
        {
            pAction = (p) => {};
        }

        StartCoroutine(InvokeToWWW(pWWW, pAction));
        return pWWW;
    }
    private IEnumerator InvokeToWWW(UnityWebRequest pWWW, Action<UnityWebRequest> pAction)
    {
        yield return pWWW.SendWebRequest();

        pAction.Invoke(pWWW);
    }

    //(false == Caching.ready) : 캐싱이 완료될때 까지 대기
    //-----------------------------------------------
    public void CachingWait(Action pAction)
    {
        if (null == pAction)
        {
            pAction = () => {};
        }

        StartCoroutine(InvokeToCachingWait(pAction));
    }
    private IEnumerator InvokeToCachingWait(Action pAction)
    {
        while (false == Caching.ready)
        {
            yield return null;
        }

        pAction.Invoke();
    }


    //yield return new AsyncOperation : 비동기 작업이 끝날 때 까지 대기 (씬로딩)
    //-----------------------------------------------
    public AsyncOperation Async(AsyncOperation pAsync, Action<AsyncOperation> pAction)
    {
        if (null == pAction)
        {
            pAction = (p) => {};
        }

        StartCoroutine(InvokeToAsync(pAsync, pAction));
        return pAsync;
    }
    private IEnumerator InvokeToAsync(AsyncOperation pAsync, Action<AsyncOperation> pAction)
    {
        while (!pAsync.isDone)
        {
            yield return null;
        }

        pAction.Invoke(pAsync);
    }
}