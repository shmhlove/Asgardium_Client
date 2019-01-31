using UnityEngine;

using System;
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

    public T GetRoot<T>(string strName) where T :  SHUIRoot
    {
        if (m_dicRoots.ContainsKey(strName))
        {
            return m_dicRoots[strName] as T;
        }
        else
        {
            return default(T);
        }
    }

    public void GetRoot<T>(string strName, Action<T> pCallback) where T :  SHUIRoot
    {
        var pUIRoot = GetRoot<T>(strName);
        if (null != pUIRoot)
        {
            pCallback(pUIRoot);
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

    public SHUIRootGlobal GetGlobalRoot()
    {
        return GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
    }
}