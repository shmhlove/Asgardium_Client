using UnityEngine;

using System;
using System.Collections.Generic;

public static class Single
{
    // 데이터
    public static SHDataManager             Data        { get { return SHDataManager.Instance; } }
    public static SHTableData               Table       { get { return Data.Table; } }
    public static SHResourceData            Resources   { get { return Data.Resources; } }
    
    // 씬
    public static SHSceneManager            Scene               { get { return SHSceneManager.Instance; } }

    // UI
    public static SHUIManager               UI                  { get { return SHUIManager.Instance; } }

    // 네트워크
    public static SHNetworkManager          Network            { get { return SHNetworkManager.Instance; } }

    // 사운드
    public static SHSoundManager            Sound               { get { return SHSoundManager.Instance; } }

    // 플랫폼
    public static SHFirebaseManager         Firebase            { get { return SHFirebaseManager.Instance; } }
    public static SHGoogleManager           Google              { get { return SHGoogleManager.Instance; } }
    public static SHAppleManager            Apple               { get { return SHAppleManager.Instance; } }

    // 유틸리티
    public static SHApplicationInfo         AppInfo             { get { return SHApplicationInfo.Instance; } }
    public static SHCoroutine               Coroutine           { get { return SHCoroutine.Instance; } }
    public static SHObjectPool              ObjectPool          { get { return SHObjectPool.Instance; } }
    public static SHRenderTextureManager    RenderTexture       { get { return SHRenderTextureManager.Instance; } }
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