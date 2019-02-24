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
        await m_pSemaphoreSlim.WaitAsync();
        
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
            m_pSemaphoreSlim.Release();
        }
        
        return await pPromise.Task;
    }
}