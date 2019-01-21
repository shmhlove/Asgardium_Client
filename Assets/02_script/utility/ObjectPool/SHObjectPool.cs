﻿using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public enum eObjectPoolReturnType
{
    None,           // 반환 : 반환안함
    Disable,        // 반환 : 오브젝트의 Active가 꺼질때 반환
    ChangeScene,    // 반환 : 씬이 변경될때 반환
}

public enum eObjectPoolDestroyType
{
    None,           // 제거 : 제거안함
    Return,         // 제거 : 풀에 반환될때 제거
    ChangeScene,    // 제거 : 씬이 변경될때 제거
}

public class SHObjectPool
{
    public eObjectPoolReturnType  m_eReturnType    = eObjectPoolReturnType.None;
    public eObjectPoolDestroyType m_eDestroyType   = eObjectPoolDestroyType.None;
    public GameObject       m_pObject        = null;

    public Vector3          m_vStartPosition = Vector3.zero;
    public Quaternion       m_qStartRotate   = Quaternion.identity;
    public Vector3          m_vStartScale    = Vector3.zero;

    public SHObjectPool() { }
    public SHObjectPool(eObjectPoolReturnType eReturnType, eObjectPoolDestroyType eDestoryType, GameObject pObject)
    {
        m_eReturnType  = eReturnType;
        m_eDestroyType = eDestoryType;
        m_pObject      = pObject;

        if (null != m_pObject)
        {
            m_vStartPosition = m_pObject.transform.localPosition;
            m_qStartRotate   = m_pObject.transform.localRotation;
            m_vStartScale    = m_pObject.transform.localScale;
        }
    }

    public void SetParent(Transform pParent)
    {
        if (null == m_pObject)
            return;

        var pLayer = m_pObject.layer;
        m_pObject.transform.SetParent(pParent);
        m_pObject.layer = pLayer;
    }

    public void SetActive(bool bIsActive)
    {
        if (null == m_pObject)
            return;

        if (bIsActive == m_pObject.activeInHierarchy)
            return;

        m_pObject.SetActive(bIsActive);
    }

    public void ResetStartTransform()
    {
        m_pObject.transform.localPosition = m_vStartPosition;
        m_pObject.transform.localRotation = m_qStartRotate;
        m_pObject.transform.localScale    = m_vStartScale;
    }

    public bool IsActive()
    {
        if (null == m_pObject)
            return false;

        return m_pObject.activeInHierarchy;
    }

    public string GetName()
    {
        if (null == m_pObject)
            return string.Empty;

        return m_pObject.name;
    }

    public bool IsSameObject(GameObject pObject)
    {
        if ((null == pObject) || (null == m_pObject))
            return false;

        return (pObject == m_pObject);
    }

    public void DestroyObject()
    {
        m_eReturnType  = eObjectPoolReturnType.None;
        m_eDestroyType = eObjectPoolDestroyType.None;

        if (null != m_pObject)
        {
            GameObject.DestroyObject(m_pObject);
        }
    }
}