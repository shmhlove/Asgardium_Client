﻿using UnityEngine;

using System;
using System.Collections.Generic;

public static class Single
{
    public static SHUIManager UI { get { return SHUIManager.Instance; } }    
    public static SHNetwork network { get { return SHNetwork.Instance; } }
}

public abstract class SHSingleton<T> : MonoBehaviour where T : SHSingleton<T>
{
    private static T m_pInstance = null;
    public static T Instance { get { return GetInstance(); } }
    public static bool IsExists { get { return (null != m_pInstance); } }

    public virtual void OnInitialize() { }

    public virtual void OnFinalize() { }

    public void Awake()
    {
        Initialize(this as T);
    }

    public void OnDestroy()
    {
        if (null == m_pInstance)
            return;

        m_pInstance.OnFinalize();
        m_pInstance = null;
    }

    private static object m_pLocker = new object();
    public static T GetInstance()
    {
        lock (m_pLocker)
        {
            if (null == m_pInstance)
            {
                if (null == (m_pInstance = SHGameObject.FindObjectOfType<T>()))
                    Initialize(SHGameObject.CreateEmptyObject(typeof(T).ToString()).AddComponent<T>());
            }

            return m_pInstance;
        }
    }

    static void Initialize(T pInstance)
    {
        if (null == pInstance)
            return;

        // 초기화 무시처리 : 싱글턴 생성시 Awake에서 호출되고, Instance Property에 접근하면서 호출될 수 있므로 인스턴스가 같으면 무시
        if (m_pInstance == pInstance)
            return;

        // 인스턴스 중복체크 : 이미 생성된 게임오브젝트가 존재할 수 있으므로 중복체크 후 인스턴스 업데이트 처리
        T pDuplication = SHGameObject.GetDuplication(pInstance);
        if (null != pDuplication)
        {
            SHGameObject.DestoryObject(pInstance.gameObject);
            m_pInstance = pDuplication;
            return;
        }

        m_pInstance = pInstance;
        m_pInstance.SetParent("SHSingletons(Destroy)");
        m_pInstance.OnInitialize();
    }

    public void CreateSingleton() { }

    public void DoDestroy()
    {
        SHGameObject.DestoryObject(gameObject);
    }

    // 인터페이스 : 씬이 제거되어도 싱글턴을 제거하지 않습니다.
    public void SetDontDestroy()
    {
        if (null == m_pInstance)
            return;

#if UNITY_EDITOR
        if (false == Application.isPlaying)
            return;
#endif

        DontDestroyOnLoad(m_pInstance.SetParent("SHSingletons(DontDestroy)"));
    }

    GameObject SetParent(string strRootName)
    {
        return SHGameObject.SetParent(gameObject, strRootName);
    }
}