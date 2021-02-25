using UnityEngine;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class SHLoadData
{
    public eDataType            m_eDataType;          // 로드할 데이터 타입
    public string               m_strName;            // 로드할 데이터 이름
    public Func<bool>           m_pLoadOkayTrigger;   // 트리거 람다 : 로드 타이밍을 SHLoader를 사용하는 곳에서 결정할 수 있도록 트리거 람다를 등록할 수 있다.
    public Action                                     // 로드 람다 : 실제 로드해야 할 타이밍에 호출될 람다.
    <   SHLoadData, 
        Action<string, SHLoadStartInfo>,
        Action<string, SHLoadEndInfo>
    > m_pLoadFunc;
    
    public SHLoadData()
    {
        m_pLoadOkayTrigger  = () => { return true; };
        m_pLoadFunc         = null;
    }
}

// public class : 로드 시작 정보
/* Summary
 * --------------------------------------------------------------------------------------
 * 데이터 로드가 시작될때 로더에 알려줘야 할 정보.
 * --------------------------------------------------------------------------------------
 * 만약 로드 방식이 Async 라면 
 * 데이터 로드부에서 AsyncOperation를 로더에 전달하여
 * 로더의 프로그래스 정보를 갱신시켜줄 수 있다.
 * --------------------------------------------------------------------------------------
 */
public class SHLoadStartInfo
{
    public AsyncOperation   m_pAsync = null;

    public SHLoadStartInfo() { }
    public SHLoadStartInfo(AsyncOperation pAsync)
    {
        m_pAsync = pAsync;
    }

    public float GetProgress()
    {
        if (null != m_pAsync)
            return m_pAsync.progress;
        
        return 0.0f;
    }
}

// public class : 로드 종료 정보
/* Summary
 * --------------------------------------------------------------------------------------
 * 데이터 로드가 종료 되었을때 로더에 알려줘야 할 정보.
 * --------------------------------------------------------------------------------------
 * 데이터를 성공적으로 로드하였는지에 대한 정보를 로더에게 알려주면
 * 로더는 모든 데이터의 결과를 종합하여 최종적으로 데이터 로드가 성공했는지를 기록한다.
 * 이 기록으로 데이터 로드를 재시작 할 것인지 게임을 진행 시킬 것인지 결정할 수 있다.
 * --------------------------------------------------------------------------------------
 */
public class SHLoadEndInfo
{
    public bool         m_bIsSuccess;
    public eErrorCode   m_eErrorCode;

    public SHLoadEndInfo(eErrorCode errorCode)
    {
        m_bIsSuccess = (eErrorCode.Succeed == errorCode);
        m_eErrorCode = errorCode;
    }
}

// public class : 로드 데이터 상태
/* Summary
 * --------------------------------------------------------------------------------------
 * 로드할 데이터에 대한 현재 상태 정보
 * --------------------------------------------------------------------------------------
 */
public class SHLoadDataStateInfo
{
    public SHLoadData      m_pLoadDataInfo;
    public SHLoadStartInfo m_pStartInfo;
    public SHLoadEndInfo   m_pEndInfo;

    public bool            m_bIsDone;
    public DateTime        m_pLoadStartTime;
    public DateTime        m_pLoadEndTime;

    public SHLoadDataStateInfo()
    {
        m_pLoadDataInfo = null;
        m_pStartInfo    = null;
        m_pEndInfo      = null;
        m_bIsDone       = false;
    }
    public SHLoadDataStateInfo(SHLoadData pData) : this()
    {
        m_pLoadDataInfo = pData;
    }
    
    public bool IsLoadOkayByTrigger()
    {
        if (null == m_pLoadDataInfo)
            return true;

        return m_pLoadDataInfo.m_pLoadOkayTrigger();
    }

    public void Load(Action<string, SHLoadStartInfo> OnEventStart, Action<string, SHLoadEndInfo> OnEventDone)
    {
        if (null == m_pLoadDataInfo)
            return;

        m_pLoadStartTime = DateTime.Now;

        if (null == m_pLoadDataInfo.m_pLoadFunc)
        {
            OnEventStart(m_pLoadDataInfo.m_strName, new SHLoadStartInfo());
            OnEventDone(m_pLoadDataInfo.m_strName, new SHLoadEndInfo(eErrorCode.Failed));
            return;
        }

        m_pLoadDataInfo.m_pLoadFunc(m_pLoadDataInfo, OnEventStart, OnEventDone);
    }

    public void LoadDone(SHLoadEndInfo pEndInfo)
    {
        m_pLoadEndTime = DateTime.Now;
        m_pEndInfo     = pEndInfo;
        m_bIsDone      = true;
    }

    public bool IsDone()
    {
        return m_bIsDone;
    }

    public float GetProgress()
    {
        if (null == m_pStartInfo)
            return 0.0f;

        return m_pStartInfo.GetProgress();
    }
}

// public class : 데이터 로딩 중 정보
/* Summary
 * --------------------------------------------------------------------------------------
 * 로딩 전체 진행상태 정보
 * --------------------------------------------------------------------------------------
 * 유저 클래스에서는 이 정보를 ProgressCallback 혹은 DoneCallback 으로 프레임마다 받을 수 있다.
 * --------------------------------------------------------------------------------------
 */
public class SHLoadingInfo
{
    // 로드 중인 데이터 정보들
    public List<SHLoadDataStateInfo> m_pLoadingDatas = new List<SHLoadDataStateInfo>();

    // 로딩 카운트 정보
    public int   m_iSucceedCount;   // 로드 성공한 데이터 카운트
    public int   m_iFailedCount;    // 로드 실패한 데이터 카운트
    public int   m_iLoadDoneCount;  // 로드 완료된 데이터 카운트
    public int   m_iTotalDataCount; // 전체 데이터 카운트
    public float m_fElapsedTime;    // 현재 경과된 시간(초단위)
    public float m_fLoadPercent;    // 로드 진행도(0 ~ 100%)

    public bool IsSucceed()
    {
        if (0 == m_iTotalDataCount)
            return false;

        if (0 == m_iLoadDoneCount)
            return false;

        return (m_iTotalDataCount == m_iSucceedCount);
    }

    public double GetPercent()
    {
        if (0 == m_iTotalDataCount)
            return 0;

        if (0 == m_iLoadDoneCount)
            return 0;

        return Math.Round(((double)m_iLoadDoneCount / (double)m_iTotalDataCount), 2) * 100;
    }
}

// public class : 로더
/* Summary
 * --------------------------------------------------------------------------------------
 * 로딩 Public 클래스
 * --------------------------------------------------------------------------------------
 * Loader를 통해 SHLoadData 리스트를 등록하면 
 * 순차적으로 로드 콜을 받을 수 있고, 프로그래스 정보를 이벤트로 받을 수 있다.
 * --------------------------------------------------------------------------------------
 */
public partial class SHLoader
{
    // 로드 진행 관리
    public SHLoadProgress m_pProgress = new SHLoadProgress();

    // 이벤트
    public Action<SHLoadingInfo> EventForDone;
    public Action<SHLoadingInfo> EventForProgress;

    public void Initialize()
    {
        m_pProgress.Initialize();
        EventForDone = null;
        EventForProgress = null;
    }
}