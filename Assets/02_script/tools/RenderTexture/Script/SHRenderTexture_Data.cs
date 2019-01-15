using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

// 클래스 : 렌더 텍스쳐 매니져
public partial class SHRenderTextureManager : SHSingleton<SHRenderTextureManager>
{
    // 멤버 : 기본정보
    public Camera                    m_pCamera            = null;
    public RenderTexture             m_pBaseTexture       = null;
    public Vector3                   m_vBaseTargetPos     = Vector3.zero;

    // 멤버 : 풀 정보
    public int                       m_iPoolCount         = 10;
    public List<SHRenderInfo>        m_pRenderInfo        = new List<SHRenderInfo>();
    public Dictionary<string, GameObject> m_dicTargetObject = new Dictionary<string, GameObject>();

    // 테스트 샘플
    public GameObject                m_pTestTarget        = null;
    public RawImage                  m_pTestRawImage      = null;    
}

// 클래스 : 렌더 텍스쳐 정보
[Serializable]
public class SHRenderInfo
{
    public int           m_iReferenceCount;         // 참조 카운트
    public bool          m_bIsOneShot;              // 한 번만 렌더링 할 것인가? 매 프레임 렌더링 할 것인가?
    public GameObject    m_pTarget;                 // 타켓 오브젝트
    public RenderTexture m_pRenderTex;              // 렌더 텍스쳐

    // 시스템 : 생성자
    public SHRenderInfo() { }
    public SHRenderInfo(RenderTexture pTex, GameObject pTarget)
    {
        m_pRenderTex = pTex;
        SetTarget(pTarget);
    }

    // 인터페이스 : 초기화
    public void Initialize()
    {
        m_iReferenceCount = 0;
        SetOneShot(false);
        SetTarget(null);
        SetTargetPos(Vector3.zero);
    }

    // 인터페이스 : 참조 카운트
    public void AddReferenceCount(int iCount)
    {
        m_iReferenceCount += iCount;
        m_iReferenceCount = Mathf.Clamp(m_iReferenceCount, 0, m_iReferenceCount);
    }

    // 인터페이스 : 원샷 렌더링 설정
    public void SetOneShot(bool bIsOneShot)
    {
        m_bIsOneShot = bIsOneShot;
    }

    // 인터페이스 : 타켓 오브젝트 설정
    public void SetTarget(GameObject pTarget)
    {
        m_pTarget = pTarget;
    }

    // 인터페이스 : 타켓 위치 설정
    public void SetTargetPos(Vector3 vTargetPos)
    {
        if (null == m_pTarget)
            return;

        m_pTarget.transform.position = vTargetPos;
    }

    // 인터페이스 : 타켓 레이어 설정
    public void SetTargetLayer(int iLayer)
    {
        if (null == m_pTarget)
            return;

        m_pTarget.layer = iLayer;
    }

    // 인터페이스 : 오브젝트 비교
    public bool IsSameObject(string strFileName)
    {
        if (null == m_pTarget)
            return false;

        return m_pTarget.name.Equals(strFileName);
    }
    public bool IsSameObject(GameObject pTarget)
    {
        return (m_pTarget == pTarget);
    }
}
