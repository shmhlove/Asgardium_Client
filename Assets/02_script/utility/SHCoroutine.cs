using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHCoroutine : SHSingleton<SHCoroutine>
{
    public override void OnInitialize()
    {
        SetDontDestroy();
        Single.Scene.AddEventForLoadedScene(OnEventLoadedScene);
    }

    public void OnEventLoadedScene(eSceneType eType)
    {
        StopAllCoroutines();
    }
    
    // yield return null : 다음 Update까지 대기
    //-----------------------------------------------
    public void NextUpdate(Action pAction)
    {
        if (null == pAction)
            return;

        StartCoroutine(InvokeToNextUpdate(pAction));
    }
    IEnumerator InvokeToNextUpdate(Action pAction)
    {
        yield return null;
        pAction.Invoke();
    }


    // yield return new WaitForFixedUpdate() : 다음 FixedUpdate까지 대기
    //-----------------------------------------------
    public void NextFixedUpdate(Action pAction)
    {
        if (null == pAction)
            return;

        StartCoroutine(InvokeToNextFixedUpdate(pAction));
    }
    IEnumerator InvokeToNextFixedUpdate(Action pAction)
    {
        yield return new WaitForFixedUpdate();
        pAction.Invoke();
    }
    

    // yield return new WaitForEndOfFrame() : 렌더링 작업이 끝날 때 까지 대기
    //-----------------------------------------------
    public void EndOfFrame(Action pAction)
    {
        if (null == pAction)
            return;

        StartCoroutine(InvokeToEndOfFrame(pAction));
    }
    IEnumerator InvokeToEndOfFrame(Action pAction)
    {
        yield return new WaitForEndOfFrame();
        pAction.Invoke();
    }
    

    // yield return new WaitForSeconds : 지정한 시간까지 대기
    //-----------------------------------------------
    public void WaitTime(Action pAction, float fDelay)
    {
        if (null == pAction)
            return;

        StartCoroutine(InvokeToWaitTime(pAction, fDelay));
    }
    IEnumerator InvokeToWaitTime(Action pAction, float fDelay)
    {
        if (0.0f >= fDelay)
            yield return null;
        else
            yield return new WaitForSeconds(fDelay);

        pAction.Invoke();
    }
    

    //yield return new WWW(string) : 웹 통신 작업이 끝날 때까지 대기
    //-----------------------------------------------
    public WWW WWW(Action<WWW> pAction, WWW pWWW)
    {
        StartCoroutine(InvokeToWWW(pAction, pWWW));
        return pWWW;
    }
    IEnumerator InvokeToWWW(Action<WWW> pAction, WWW pWWW)
    {
        yield return pWWW;

        if (null != pAction)
            pAction.Invoke(pWWW);
    }
    public WWW WWWOfSync(WWW pWWW)
    {
        InvokeToWWW(null, pWWW);
        while (false == pWWW.isDone);
        return pWWW;
    }

    //(false == Caching.ready) : 캐싱이 완료될때 까지 대기
    //-----------------------------------------------
    public void CachingWait(Action pAction)
    {
        StartCoroutine(InvokeToCachingWait(pAction));
    }
    IEnumerator InvokeToCachingWait(Action pAction)
    {
        while (false == Caching.ready)
            yield return null;

        if (null != pAction)
            pAction.Invoke();
    }


    //yield return new AsyncOperation : 비동기 작업이 끝날 때 까지 대기 (씬로딩)
    //-----------------------------------------------
    public AsyncOperation Async(Action pAction, AsyncOperation pAsync)
    {
        StartCoroutine(InvokeToAsync(pAction, pAsync));
        return pAsync;
    }
    IEnumerator InvokeToAsync(Action pAction, AsyncOperation pAsync)
    {
        while((null != pAsync) && (false == pAsync.isDone))
            yield return null;

        if (null != pAction)
            pAction.Invoke();
    }
}