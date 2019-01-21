using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using DicRoots  = System.Collections.Generic.Dictionary<int, UnityEngine.Transform>;
using DicObject = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<SHObjectPool>>;

public class SHObjectPoolManager : SHSingleton<SHObjectPoolManager>
{
    DicRoots m_dicRoots      = new DicRoots();

    DicObject m_dicActives   = new DicObject();
    DicObject m_dicInactives = new DicObject();

    private readonly int DELAY_CHECK_FOR_RECOVERY = 5;

    public override void OnInitialize()
    {
        SetDontDestroy();

        ClearAll();
        Single.Scene.AddEventForLoadedScene(OnEventOfLoadedScene);

        StartCoroutine(CoroutineCheckAutoRecovery());
    }

    public override void OnFinalize()
    {
        StopAllCoroutines();
    }

    public T Get<T>(
        string                 strName, 
        eObjectPoolReturnType  eReturnType  = eObjectPoolReturnType.Disable, 
        eObjectPoolDestroyType eDestroyType = eObjectPoolDestroyType.ChangeScene) where T : Component
    {
        return SHGameObject.GetComponent<T>(Get(strName, eReturnType, eDestroyType));
    }

    public GameObject Get(
        string                 strName, 
        eObjectPoolReturnType  eReturnType  = eObjectPoolReturnType.Disable, 
        eObjectPoolDestroyType eDestroyType = eObjectPoolDestroyType.ChangeScene)
    {
        var pObject = GetInactiveObject(eReturnType, eDestroyType, strName);
        SetActiveObject(strName, pObject);
        return pObject.m_pObject;
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

        if (eObjectPoolDestroyType.Return == pObjectInfo.m_eDestroyType)
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

    private SHObjectPool GetInactiveObject(eObjectPoolReturnType eReturnType, eObjectPoolDestroyType eDestroyType, string strName)
    {
        var pObjects = GetInactiveObjects(strName);
        if (0 == pObjects.Count)
        {
            return new SHObjectPool(
                eReturnType,
                eDestroyType, 
                Single.Resources.GetGameObject(strName));
        }
        else
        {
            pObjects[0].m_eReturnType  = eReturnType;
            pObjects[0].m_eDestroyType = eDestroyType;
            return pObjects[0];
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
                case eObjectPoolReturnType.Disable:
                    if (false == pItem.IsActive())
                    {
                        pReturns.Add(pItem);
                    }
                    break;
                case eObjectPoolReturnType.ChangeScene:
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
                case eObjectPoolDestroyType.ChangeScene:
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
                case eObjectPoolDestroyType.ChangeScene:
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
            var pRoot = SHGameObject.CreateEmptyObject(string.Format("SHObjectPool_{0}", iLayer));
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
