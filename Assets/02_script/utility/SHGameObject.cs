using UnityEngine;

using System;
using System.Collections;

public static class SHGameObject
{
    public static GameObject GetObject(string strRoot)
    {
        GameObject pRoot = GameObject.Find(strRoot);
        if (null == pRoot)
            pRoot = new GameObject(strRoot);

        return pRoot;
    }
    
    public static GameObject SetParent(GameObject pObject, string strParent)
    {
        return SetParent(pObject, GetObject(strParent));
    }

    public static GameObject SetParent(GameObject pChild, GameObject pParent)
    {
        if (null == pParent)
            return null;

        if (null == pChild)
            return null;

        SetParent(pChild.transform, pParent.transform);
        
        return pParent;
    }

    public static Transform SetParent(Transform pChild, Transform pParent)
    {
        if (null == pChild)
            return null;
        if (null == pParent)
            return null;

        pChild.SetParent(pParent);
        return pParent;
    }
    
    public static T GetComponent<T>(GameObject pObject) where T : Component
    {
        if (null == pObject)
            return default(T);

        T pComponent = pObject.GetComponent<T>();
        if (null == pComponent)
        {
            pComponent = pObject.AddComponent<T>();
        }

        return pComponent;
    }
    
    public static T GetDuplication<T>(T pInstance) where T : UnityEngine.Object
    {
        var pList = GameObject.FindObjectsOfType<T>();
        if (null == pList)
            return null;
        
        for (int iLoop = 0; iLoop < pList.Length; ++iLoop)
        {
            if (pInstance.GetInstanceID() != pList[iLoop].GetInstanceID())
                return pList[iLoop];
        }
        
        return null;
    }
}