using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHUIManager : SHSingleton<SHUIManager>
{
    Dictionary<Type, SHUIRoot> dicRoots = new Dictionary<Type, SHUIRoot>();

    public override void OnInitialize()
    {
        SetDontDestroy();
        Single.Scene.AddEventForLoadedScene(OnEventLoadedScene);
    }

    public void AddRoot(Type type, SHUIRoot root)
    {
        if (false == dicRoots.ContainsKey(type))
        {
            dicRoots.Add(type, root);
        }
        else
        {
            dicRoots[type] = root;
        }
    }

    public void RemoveRoot(Type type)
    {
        if (dicRoots.ContainsKey(type))
        {
            dicRoots.Remove(type);
        }
    }

    public void GetRoot<T>(Action<T> pCallback) where T :  SHUIRoot
    {
        if (dicRoots.ContainsKey(typeof(T)))
        {
            pCallback(dicRoots[typeof(T)] as T);
            return;
        }

        Single.Resources.GetGameObject(typeof(T).ToString(), (pObject) => 
        {
            if (null != pObject)
            {
                var tObject = pObject as T;
                AddRoot(typeof(T), tObject);
                pCallback(tObject);
            }
            else
            {
                pCallback(default(T));
            }
        });
    }

    void OnEventLoadedScene(eSceneType eType)
    {
        // 씬이 변경될때 한번 털어줘야한다.
    }
}