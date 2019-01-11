using UnityEngine;

using System;
using System.Collections;

public static class SHGameObject
{
    // 빈 오브젝트 생성
    public static GameObject CreateEmptyObject(string strName)
    {
        return new GameObject(strName);
    }

    // 오브젝트 즉시 제거
    public static void DestoryObject(UnityEngine.Object pObject)
    {
        if (null == pObject)
            return;

        UnityEngine.Object.DestroyImmediate(pObject);
    }

    // 오브젝트 찾기 및 생성
    public static GameObject GetObject(string strRoot)
    {
        GameObject pRoot = Find(strRoot);
        if (null == pRoot)
            pRoot = CreateEmptyObject(strRoot);

        return pRoot;
    }

    // 전체 오브젝트에서 찾기(이름으로)
    public static GameObject Find(string strName)
    {
        return GameObject.Find(strName);
    }

    // 전체 오브젝트에서 찾기(타입으로)
    public static T FindObjectOfType<T>() where T : UnityEngine.Object
    {
        return GameObject.FindObjectOfType<T>();
    }

    // 전체 오브젝트에서 찾기(타입으로)
    public static T[] FindObjectsOfType<T>() where T : UnityEngine.Object
    {
        return GameObject.FindObjectsOfType<T>();
    }
    
    // 자식 오브젝트에서 찾기
    public static GameObject FindChild(GameObject pRoot, string strName)
    {
        if (null == pRoot)
            return null;

        Transform pChildren = FindChild(pRoot.transform, strName);
        if (null == pChildren)
            return null;

        return pChildren.gameObject;
    }
    public static Transform FindChild(Transform pRoot, string strName)
    {
        if (null == pRoot)
            return null;

        return pRoot.FindChild(strName);
    }
    
    // 하이어라키 설정
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

    // 컴포넌트 얻기
    public static T GetComponent<T>(GameObject pObject) where T : Component
    {
        if (null == pObject)
            return default(T);

        T pComponent = pObject.GetComponent<T>();
        if (null == pComponent)
            pComponent = pObject.AddComponent<T>();

        return pComponent;
    }

    // 객체 중복체크
    public static T GetDuplication<T>(T pInstance) where T : UnityEngine.Object
    {
        var pList = SHGameObject.FindObjectsOfType<T>();
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