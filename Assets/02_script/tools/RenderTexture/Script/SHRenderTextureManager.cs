using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

// 클래스 : 렌더 텍스쳐 매니져
public partial class SHRenderTextureManager : SHSingleton<SHRenderTextureManager>
{
    // 다양화 : 싱글턴이 생성 될때
    public override void OnInitialize()
    {
        SetDontDestroy();
        CreateRenderInfo(m_iPoolCount);
    }

    // 다양화 : 싱글턴이 제거될때
    public override void OnFinalize()
    {
        m_pRenderInfo.Clear();
        m_dicTargetObject.Clear();
    }

    // 시스템 : 프레임 업데이트
    public void Update()
    {
        foreach (SHRenderInfo pInfo in m_pRenderInfo)
        {
            // 참조카운트 체크 후 초기화 처리
            if (0 == pInfo.m_iReferenceCount)
            {
                pInfo.Initialize();
                continue;
            }

            // 원샷 렌더링 체크
            if (true == pInfo.m_bIsOneShot)
                continue;

            // 계속 렌더링
            RenderTexture(pInfo);
        }
    }
    
    // 인터페이스 : 렌더 텍스쳐 얻기
    public Texture RenderTextureOfOneShot(string strFileName)
    {
        return RenderTextureOfOneShot(CreateTargetObject(strFileName));
    }
    public Texture RenderTextureOfOneShot(GameObject pTarget)
    {
        return RenderTextureOfOneShot(pTarget, m_vBaseTargetPos);
    }
    public Texture RenderTextureOfOneShot(GameObject pTarget, Vector3 vTargetPos)
    {
        SHRenderInfo pInfo = GetRenderInfo(pTarget);
        if (null == pInfo)
            return null;

        pInfo.SetOneShot(true);
        pInfo.SetTarget(pTarget);
        pInfo.SetTargetPos(vTargetPos);
        pInfo.AddReferenceCount(1);

        return RenderTexture(pInfo);
    }
    public Texture RenderTextureOfContinue(string strFileName)
    {
        return RenderTextureOfContinue(CreateTargetObject(strFileName));
    }
    public Texture RenderTextureOfContinue(GameObject pTarget)
    {
        return RenderTextureOfContinue(pTarget, m_vBaseTargetPos);
    }
    public Texture RenderTextureOfContinue(GameObject pTarget, Vector3 vTargetPos)
    {
        SHRenderInfo pInfo = GetRenderInfo(pTarget);
        if (null == pInfo)
            return null;

        pInfo.SetOneShot(false);
        pInfo.SetTarget(pTarget);
        pInfo.SetTargetPos(vTargetPos);
        pInfo.AddReferenceCount(1);

        return RenderTexture(pInfo);
    }
    
    // 인터페이스 : 렌더 텍스쳐 사용 종료
    public void EndUseTexture(GameObject pTarget)
    {
        SHRenderInfo pInfo = FindRenderInfo(pTarget);
        if (null == pInfo)
            return;

        pInfo.AddReferenceCount(-1);
    }
    public void EndUseTexture(string strFileName)
    {
        SHRenderInfo pInfo = FindRenderInfo(strFileName);
        if (null == pInfo)
            return;

        pInfo.AddReferenceCount(-1);
    }

    // 테스트 : 렌더 텍스쳐 찍기
    [FuncButton]
    void Test_ContinueShotting_Texture()
    {
        if (null == m_pTestTarget)
            return;

        if (null == m_pTestRawImage)
            return;

        m_pTestRawImage.texture = 
            RenderTextureOfContinue(m_pTestTarget);
    }
    [FuncButton]
    void Test_OneShotting_Texture()
    {
        if (null == m_pTestTarget)
            return;

        if (null == m_pTestRawImage)
            return;

        m_pTestRawImage.texture =
            RenderTextureOfOneShot(m_pTestTarget);
    }
    // 테스트 : 사용 종료
    [FuncButton]
    void Test_EndUse_Texture()
    {
        if (null == m_pTestTarget)
            return;
        
        EndUseTexture(m_pTestTarget);
    }
}