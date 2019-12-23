using UnityEngine;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class SHUIManager : SHSingleton<SHUIManager>
{
    private Dictionary<string, SHUIRoot> m_dicRoots = new Dictionary<string, SHUIRoot>();
    private SemaphoreSlim m_pSemaphoreSlim = new SemaphoreSlim(1,1);

    public override void OnInitialize()
    {
        SetDontDestroy();
    }
    
    public void AddRoot(string strName, SHUIRoot pRoot)
    {
        if (null == pRoot)
        {
            return;
        }

        if (false == m_dicRoots.ContainsKey(strName))
        {
            m_dicRoots.Add(strName, pRoot);
        }
        else
        {
            m_dicRoots[strName] = pRoot;
        }
    }

    public void DelRoot(string strName)
    {
        if (true == m_dicRoots.ContainsKey(strName))
        {
            m_dicRoots.Remove(strName);
        }
    }
    
    public async Task<T> GetRoot<T>(string strName) where T : SHUIRoot
    {
        // 아래 코드를 한순간에 한번만 실행 될 수 있도록 세마포어로 대기시켜준다.
        // 비동기 실행시 똑같은 리소스를 여러번 동적로드 할 수 있다.
        await m_pSemaphoreSlim.WaitAsync();
        
        // Single.Resources.GetComponentByObject에 의한 비동기 동적로드가 있을 수 있으므로
        // Promise 개념으로 Async-Await 처리를 한다.
        var pPromise = new TaskCompletionSource<T>();
        try 
        {
            if (m_dicRoots.ContainsKey(strName))
            {
                pPromise.TrySetResult(m_dicRoots[strName] as T);
            }
            else
            {
                var pObject = await Single.Resources.GetComponentByObject<T>(strName);
                AddRoot(strName, pObject);
                pPromise.TrySetResult(pObject);
            }
        }
        finally
        {
            m_pSemaphoreSlim.Release(1);
        }
        
        return await pPromise.Task;
    }
}