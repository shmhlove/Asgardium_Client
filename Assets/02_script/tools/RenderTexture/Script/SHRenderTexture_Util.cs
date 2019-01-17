using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHRenderTextureManager : SHSingleton<SHRenderTextureManager>
{
    // 유틸 : 텍스쳐에 렌더링
    Texture RenderTexture(SHRenderInfo pInfo)
    {
        if (null == m_pCamera)
            return null;

        if (null == pInfo)
            return null;

        if (null == pInfo.m_pTarget)
            return null;

        // 타켓 설정
        pInfo.m_pTarget.SetActive(true);
        pInfo.SetTargetLayer(gameObject.layer);

        // 렌더링
        m_pCamera.targetTexture = pInfo.m_pRenderTex;
        m_pCamera.Render();
        m_pCamera.targetTexture = null;
        return pInfo.m_pRenderTex;
    }

    // 유틸 : 렌더정보 얻기
    SHRenderInfo GetRenderInfo(GameObject pTarget)
    {
        if (null == pTarget)
        {
            Debug.LogError("타켓 오브젝트가 없습니다.");
            return null;
        }

        SHRenderInfo pInfo = FindRenderInfo(pTarget);
        if (null == pInfo)
        {
            pInfo = GetEmptyRenderInfo();
        }

        if (null == pInfo)
        {
            Debug.LogError("렌더 텍스쳐 정보를 얻지 못하였습니다.");
            return null;
        }

        return pInfo;
    }

    // 유틸 : 렌더정보 찾기
    SHRenderInfo FindRenderInfo(GameObject pObject)
    {
        if (null == pObject)
            return null;

        return FindRenderInfo(pObject.name);
    }
    SHRenderInfo FindRenderInfo(string strFileName)
    {
        foreach (SHRenderInfo pInfo in m_pRenderInfo)
        {
            if (true == pInfo.IsSameObject(strFileName))
                return pInfo;
        }
        return null;
    }

    // 유틸 : 빈 렌더정보 찾기 ( 없으면 새로 생성 )
    SHRenderInfo GetEmptyRenderInfo()
    {
        foreach (SHRenderInfo pInfo in m_pRenderInfo)
        {
            if (0 == pInfo.m_iReferenceCount)
                return pInfo;
        }
        return AddRenderInfo();
    }

    // 유틸 : 렌더 텍스쳐 정보 추가
    SHRenderInfo AddRenderInfo()
    {
        return AddRenderInfo(CreateRenderTexture(), null);
    }
    SHRenderInfo AddRenderInfo(GameObject pTarget)
    {
        return AddRenderInfo(CreateRenderTexture(), pTarget);
    }
    SHRenderInfo AddRenderInfo(RenderTexture pTexture, GameObject pTarget)
    {
        if (null == pTexture)
            return null;

        SHRenderInfo pInfo = new SHRenderInfo(pTexture, pTarget);
        m_pRenderInfo.Add(pInfo);
        return pInfo;
    }

    // 유틸 : 렌더 정보 iCount만큼 생성
    void CreateRenderInfo(int iCount)
    {
        for (int iLoop = 0; iLoop < iCount; ++iLoop)
            AddRenderInfo();
    }

    // 유틸 : 렌더 텍스쳐 생성
    RenderTexture CreateRenderTexture()
    {
        if (null == m_pBaseTexture)
            return null;
        
        return Single.Resources.Instantiate<RenderTexture>(m_pBaseTexture);
    }

    // 유틸 : 타켓 오브젝트 생성
    GameObject CreateTargetObject(string strFileName)
    {
        if (false == m_dicTargetObject.ContainsKey(strFileName))
        {
            GameObject pObject = Single.Resources.GetGameObject(strFileName);
            if (null == pObject)
                return null;

            SHGameObject.SetParent(pObject, gameObject);
            m_dicTargetObject.Add(strFileName, pObject);
        }

        return m_dicTargetObject[strFileName];
    }
}