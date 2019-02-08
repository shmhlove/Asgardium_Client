using UnityEngine;

using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHUIManager : SHSingleton<SHUIManager>
{
    Dictionary<string, SHUIRoot> m_dicRoots = new Dictionary<string, SHUIRoot>();

    public override void OnInitialize()
    {
        SetDontDestroy();
    }
    
    public void AddRoot(string strName, SHUIRoot root)
    {
        if (false == m_dicRoots.ContainsKey(strName))
        {
            m_dicRoots.Add(strName, root);
        }
        else
        {
            m_dicRoots[strName] = root;
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
        var promise = new TaskCompletionSource<T>();

        GetRoot<T>(strName, (pUIRoot) =>
        {
            promise.TrySetResult(pUIRoot);
        });

        await promise.Task;
        return promise.Task.Result;
    }

    public void GetRoot<T>(string strName, Action<T> pCallback) where T :  SHUIRoot
    {
        if (m_dicRoots.ContainsKey(strName))
        {
            pCallback(m_dicRoots[strName] as T);
            return;
        }

        Single.Resources.GetComponentByObject<T>(strName, (pObject) => 
        {
            if (null != pObject)
            {
                AddRoot(strName, pObject);
                pCallback(pObject);
            }
            else
            {
                pCallback(default(T));
            }
        });
    }

    public async Task<SHUIRootGlobal> GetGlobalRoot()
    {
        return await GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
    }
}