using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using DicRoots  = System.Collections.Generic.Dictionary<int, UnityEngine.Transform>;
using DicObject = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<SHObjectPool>>;

public class SHObjectPoolManager : SHSingleton<SHObjectPoolManager>
{
    // 오브젝트 풀의 관리 포인트로 루트오브젝트를 두고 있다.
    // Dictionary로 관리하는 이유는 오브젝트 그룹핑을 위해 레이어 구분을 두기 위해서이다.
    // 즉, 오브젝트 풀의 Root 오브젝트의 이름은 SHObjectPool_LayerNumber로 관리하고 있다.
    DicRoots m_dicRoots      = new DicRoots();

    // 활성화 되어 있는 오브젝트들
    DicObject m_dicActives   = new DicObject();
    // 비 활성화 되어 있는 오브젝트들
    DicObject m_dicInactives = new DicObject();

    private readonly int DELAY_CHECK_FOR_RECOVERY = 5;

    public override void OnInitialize()
    {
        SetDontDestroy();

        ClearAll();
        Single.Scene.AddEventForBeforeLoadScene(OnEventOfLoadedScene);

        StartCoroutine(CoroutineCheckAutoRecovery());
    }
    
    public override void OnFinalize()
    {
        StopAllCoroutines();
        Single.Scene.DelEventForBeforeLoadScene(OnEventOfLoadedScene);
    }

    public void Get<T>(string  strName, 
        eObjectPoolReturnType  eReturnType, 
        eObjectPoolDestroyType eDestroyType,
        Action<T>              pCallback) where T : Component
    {
        Get(strName, eReturnType, eDestroyType, (pObject) => 
        {
            pCallback(SHGameObject.GetComponent<T>(pObject));
        });
    }

    public void Get(string     strName, 
        eObjectPoolReturnType  eReturnType, 
        eObjectPoolDestroyType eDestroyType,
        Action<GameObject>     pCallback)
    {
        GetInactiveObject(strName, eReturnType, eDestroyType, (pObject) => 
        {
            SetActiveObject(strName, pObject);
            pCallback(pObject.m_pObject);
        });
    }

    public void Return(GameObject pObject)
    {
        var pObjectInfo = GetObjectInfo(pObject);
        if (null == pObjectInfo)
            return;

        SetReturnObject(pObjectInfo.GetName(), pObjectInfo);
    }

    public void ResetStartTransform(GameObject pObject)
    {
        var pObjectInfo = GetObjectInfo(pObject);
        if (null == pObjectInfo)
            return;

        pObjectInfo.ResetStartTransform();
    }

    private void SetActiveObject(string strName, SHObjectPool pObjectInfo)
    {
        CheckDictionary(m_dicActives,   strName);
        CheckDictionary(m_dicInactives, strName);

        m_dicActives[strName].Add(pObjectInfo);
        m_dicInactives[strName].Remove(pObjectInfo);
        
        pObjectInfo.SetParent(GetRoot(pObjectInfo.m_pObject.layer));
        pObjectInfo.ResetStartTransform();
        pObjectInfo.SetActive(false);
    }

    private void SetReturnObject(string strName, SHObjectPool pObjectInfo)
    {
        CheckDictionary(m_dicActives,   strName);
        CheckDictionary(m_dicInactives, strName);

        if (eObjectPoolDestroyType.WhenReturn == pObjectInfo.m_eDestroyType)
        {
            SetDestroyObject(strName, pObjectInfo);
        }
        else
        {
            m_dicActives[strName].Remove(pObjectInfo);
            m_dicInactives[strName].Add(pObjectInfo);

            pObjectInfo.SetParent(GetRoot(pObjectInfo.m_pObject.layer));
            pObjectInfo.SetActive(false);
        }
    }

    private void SetDestroyObject(string strName, SHObjectPool pObjectInfo)
    {
        CheckDictionary(m_dicActives,   strName);
        CheckDictionary(m_dicInactives, strName);

        m_dicActives[strName].Remove(pObjectInfo);
        m_dicInactives[strName].Remove(pObjectInfo);

        pObjectInfo.DestroyObject();
    }

    private List<SHObjectPool> GetInactiveObjects(string strName)
    {
        if (false == m_dicInactives.ContainsKey(strName))
            return new List<SHObjectPool>();
        else
            return m_dicInactives[strName];
    }

    private async void GetInactiveObject(string                  strName, 
                                         eObjectPoolReturnType   eReturnType, 
                                         eObjectPoolDestroyType  eDestroyType,
                                         Action<SHObjectPool>    pCallback)
    {                                    
        var pObjects = GetInactiveObjects(strName);
        if (0 == pObjects.Count)
        {
            var pObject = await Single.Resources.GetGameObject(strName);
            pCallback(new SHObjectPool(eReturnType, eDestroyType, pObject));
        }
        else
        {
            pObjects[0].m_eReturnType  = eReturnType;
            pObjects[0].m_eDestroyType = eDestroyType;
            pCallback(pObjects[0]);
        }
    }

    private SHObjectPool GetObjectInfo(GameObject pObject)
    {
        if (null == pObject)
            return null;
        
        var strName = pObject.name;
        if (true == m_dicActives.ContainsKey(strName))
        {
            var pObjectInfo = m_dicActives[strName].Find((pItem) =>
                              { return pItem.IsSameObject(pObject); });
            if (null != pObjectInfo)
                return pObjectInfo;
        }

        if (true == m_dicInactives.ContainsKey(strName))
        {
            var pObjectInfo = m_dicInactives[strName].Find((pItem) =>
                              { return pItem.IsSameObject(pObject); });
            if (null != pObjectInfo)
                return pObjectInfo;
        }

        return null;
    }

    private void ClearAll()
    {
        foreach(var kvp in m_dicActives)
        {
            foreach (var pObject in kvp.Value)
            {
                pObject.DestroyObject();
            }
        }

        foreach(var kvp in m_dicInactives)
        {
            foreach (var pObject in kvp.Value)
            {
                pObject.DestroyObject();
            }
        }
        
        m_dicActives.Clear();
        m_dicInactives.Clear();
    }

    private void CheckDictionary(DicObject dicObjects, string strName)
    {
        if (true == dicObjects.ContainsKey(strName))
            return;

        dicObjects.Add(strName, new List<SHObjectPool>());
    }

    private void ForLoopActives(Action<SHObjectPool> pCallback)
    {
        if (null == pCallback)
            return;
        
        foreach (var kvp in m_dicActives)
        {
            foreach (var pItem in kvp.Value)
            {
                pCallback(pItem);
            }
        }

        foreach (var kvp in m_dicActives)
        {
            foreach (var pItem in kvp.Value)
            {
                pCallback(pItem);
            }
        }
    }

    private void ForLoopInactives(Action<SHObjectPool> pCallback)
    {
        if (null == pCallback)
            return;
        
        foreach (var kvp in m_dicInactives)
        {
            foreach (var pItem in kvp.Value)
            {
                pCallback(pItem);
            }
        }
    }

    private void CheckAutoReturnObject(bool bIsChangeScene)
    {
        var pReturns = new List<SHObjectPool>();
        ForLoopActives((pItem) =>
        {
            switch (pItem.m_eReturnType)
            {
                case eObjectPoolReturnType.WhenDisable:
                    if (false == pItem.IsActive())
                    {
                        pReturns.Add(pItem);
                    }
                    break;
                case eObjectPoolReturnType.WhenChangeScene:
                    if (true == bIsChangeScene)
                    {
                        pReturns.Add(pItem);
                    }
                    break;
            }
        });

        foreach (var pItem in pReturns)
        {
            SetReturnObject(pItem.GetName(), pItem);
        }
    }

    private void CheckAutoDestroyObject(bool bIsChangeScene)
    {
        var pDestroys = new List<SHObjectPool>();
        ForLoopActives((pItem) =>
        {
            switch (pItem.m_eDestroyType)
            {
                case eObjectPoolDestroyType.WhenChangeScene:
                    if (true == bIsChangeScene)
                    {
                        pDestroys.Add(pItem);
                    }
                    break;
            }
        });

        ForLoopInactives((pItem) =>
        {
            switch (pItem.m_eDestroyType)
            {
                case eObjectPoolDestroyType.WhenChangeScene:
                    if (true == bIsChangeScene)
                    {
                        pDestroys.Add(pItem);
                    }
                    break;
            }
        });

        foreach (var pItem in pDestroys)
        {
            SetDestroyObject(pItem.GetName(), pItem);
        }
    }

    Transform GetRoot(int iLayer)
    {
        if (false == m_dicRoots.ContainsKey(iLayer))
        {
            var pRoot = new GameObject(string.Format("SHObjectPool_{0}", iLayer));
            pRoot.layer = iLayer;
            DontDestroyOnLoad(pRoot);
            m_dicRoots.Add(iLayer, pRoot.transform);
        }

        return m_dicRoots[iLayer];
    }

    IEnumerator CoroutineCheckAutoRecovery()
    {
        while (true)
        {
            yield return new WaitForSeconds(DELAY_CHECK_FOR_RECOVERY);

            CheckAutoReturnObject(false);
            CheckAutoDestroyObject(false);
        }
    }
    
    public void OnEventOfLoadedScene(eSceneType eType)
    {
        CheckAutoReturnObject(true);
        CheckAutoDestroyObject(true);
    }
}
