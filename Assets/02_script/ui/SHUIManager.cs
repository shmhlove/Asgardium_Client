using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHUIManager : SHSingleton<SHUIManager>
{
    Dictionary<Type, SHUIRoot> m_dicRoots = new Dictionary<Type, SHUIRoot>();

    public override void OnInitialize()
    {
        SetDontDestroy();
    }
    
    public void AddRoot(Type type, SHUIRoot root)
    {
        if (false == m_dicRoots.ContainsKey(type))
        {
            m_dicRoots.Add(type, root);
        }
        else
        {
            m_dicRoots[type] = root;
        }
    }

    public void DelRoot(Type type)
    {
        if (true == m_dicRoots.ContainsKey(type))
        {
            m_dicRoots.Remove(type);
        }
    }

    public void GetRoot<T>(Action<T> pCallback) where T :  SHUIRoot
    {
        if (m_dicRoots.ContainsKey(typeof(T)))
        {
            pCallback(m_dicRoots[typeof(T)] as T);
            return;
        }

        Single.Resources.GetComponentByObject<T>(typeof(T).ToString(), (pObject) => 
        {
            if (null != pObject)
            {
                AddRoot(typeof(T), pObject);
                pCallback(pObject);
            }
            else
            {
                pCallback(default(T));
            }
        });
    }
}