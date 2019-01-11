using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class SHUIManager : SHSingleton<SHUIManager>
{
    Dictionary<Type, SHUIRoot> dicRoots = new Dictionary<Type, SHUIRoot>();

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
        else
        {
            return default(T);
        }
    }
}