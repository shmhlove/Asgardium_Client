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
    }

    public bool AddRoot(Type type, SHUIRoot root)
    {
        if (dicRoots.ContainsKey(type))
        {
            return false;
        }

        dicRoots.Add(type, root);
        return true;
    }

    public bool RemoveRoot(Type type)
    {
        if (dicRoots.ContainsKey(type))
        {
            dicRoots.Remove(type);
            return true;
        }

        return false;
    }

    public T GetRoot<T>() where T :  SHUIRoot
    {
        if (dicRoots.ContainsKey(typeof(T)))
        {
            return dicRoots[typeof(T)] as T;
        }

        // 리소스 동적로드 처리 
        // (싱크방식,, 비동기방식으로 하는게 쉬운데.. 쓰기가 불편하니..)

        return default(T);
    }
}