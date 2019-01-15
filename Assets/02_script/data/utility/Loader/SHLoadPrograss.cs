using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

// public class : 로드 프로그래스
/* Summary
 * --------------------------------------------------------------------------------------
 * 로딩 플로우의 세부기능을 담고 있으며 Loader를 통해 제어됩니다.
 * --------------------------------------------------------------------------------------
 */
public class SHLoadProgress
{
    // 로드 대기 중인 데이터 정보
    private Queue<SHLoadDataStateInfo> m_qLoadDatumWaitQueue = new Queue<SHLoadDataStateInfo>();

    // 로드 진행 중인 데이터 정보<파일명, 파일정보>
    private Dictionary<string, SHLoadDataStateInfo> m_dicLoadingDatum = new Dictionary<string, SHLoadDataStateInfo>();
    public List<SHLoadDataStateInfo> LoadingDatum { get { return new List<SHLoadDataStateInfo>(m_dicLoadingDatum.Values); } }

    // 로드 성공한 데이터 정보<파일명, 파일정보>
    private Dictionary<string, SHLoadDataStateInfo> m_dicLoadSucceedDatum = new Dictionary<string, SHLoadDataStateInfo>();

    // 로드 실패한 데이터 정보<파일명, 파일정보>
    private Dictionary<string, SHLoadDataStateInfo> m_dicLoadFailedDatum = new Dictionary<string, SHLoadDataStateInfo>();

    // 전체 데이터 정보 : <데이터 타입, <파일명, 파일정보>>
    private Dictionary<eDataType, Dictionary<string, SHLoadDataStateInfo>> m_dicAllLoadDatum = new Dictionary<eDataType, Dictionary<string, SHLoadDataStateInfo>>();

    // 로드 시작 시간
    public DateTime m_pLoadStartTime;

    // 로드 종료 시간
    public DateTime m_pLoadEndTime;
    
    public void Initialize()
    {
        m_qLoadDatumWaitQueue.Clear();
        m_dicAllLoadDatum.Clear();
        m_dicLoadingDatum.Clear();
        m_dicLoadSucceedDatum.Clear();
        m_dicLoadFailedDatum.Clear();
    }

    public void AddLoadDatum(Dictionary<string, SHLoadData> dicLoadDatum)
    {
        SHUtils.ForToDic(dicLoadDatum, (strName, pData) => 
        {
            if (null == pData)
                return;

            var pLoadData = GetLoadDataInfo(strName);
            if (null != pLoadData)
            {
                Debug.LogErrorFormat("[LSH] 데이터 로드 중 중복파일 발견!!!(FileName : {0})", strName);
                return;
            }
            
            AddLoadData(pData);
        });
    }
    
    public SHLoadDataStateInfo GetLoadDataInfo(string strName)
    {
        foreach (var kvp in m_dicAllLoadDatum)
        {
            if (true == kvp.Value.ContainsKey(strName.ToLower()))
                return kvp.Value[strName.ToLower()];
        }
        return null;
    }
    
    public void EnqueueWaitingDataInfo(SHLoadDataStateInfo pLoadDataInfo)
    {
        if (true == m_qLoadDatumWaitQueue.Contains(pLoadDataInfo))
            return;

        m_qLoadDatumWaitQueue.Enqueue(pLoadDataInfo);
    }

    public SHLoadDataStateInfo DequeueWaitingDataInfo()
    {
        if (0 == m_qLoadDatumWaitQueue.Count)
            return null;

        var pDataInfo = m_qLoadDatumWaitQueue.Dequeue();
        if (null == pDataInfo)
            return null;

        return pDataInfo;
    }

    public SHLoadingInfo GetLoadingInfo()
    {
        var pLoadingInfo = new SHLoadingInfo();

        // 로드 중인 데이터 정보들
        pLoadingInfo.m_pLoadingDatum   = LoadingDatum;

        // 로딩 카운트 정보
        pLoadingInfo.m_iSucceedCount   = GetLoadSucceedCount();
        pLoadingInfo.m_iFailedCount    = GetLoadFailedCount();
        pLoadingInfo.m_iLoadDoneCount  = GetLoadDoneCount();
        pLoadingInfo.m_iTotalDataCount = GetTotalCount();
        pLoadingInfo.m_fElapsedTime    = GetLoadTime();
        pLoadingInfo.m_fLoadPercent    = GetProgressPercent();

        return pLoadingInfo;
    }

    public int GetLoadSucceedCount()
    {
        return m_dicLoadSucceedDatum.Count;
    }

    public int GetLoadFailedCount()
    {
        return m_dicLoadFailedDatum.Count;
    }
    
    public int GetLoadDoneCount()
    {
        return (m_dicLoadSucceedDatum.Count + m_dicLoadFailedDatum.Count);
    }

    public int GetTotalCount()
    {
        int iTotalCount = 0;
        foreach(var kvp in m_dicAllLoadDatum)
        {
            iTotalCount += kvp.Value.Count;
        }
        return iTotalCount;
    }

    public float GetProgressPercent()
    {
        if (false == IsDone())
            return 100.0f;

        float fProgress = 0.0f;

        // 로드 중인 파일의 진행률 반영
        SHUtils.ForToList<SHLoadDataStateInfo>(LoadingDatum, (pDataInfo) =>
        {
            fProgress += pDataInfo.GetProgress();
        });
        fProgress /= LoadingDatum.Count;
        
        // 로드 완료된 파일의 진행률 반영
        fProgress += (GetLoadDoneCount() / GetTotalCount());
        
        // 100분률로 변경 후 반환
        return (fProgress * 100.0f);
    }

    public void SetLoadStartInfo(string strName, SHLoadStartInfo pLoadStartInfo)
    {
        if (null == pLoadStartInfo)
            return;

        var pDataInfo = GetLoadDataInfo(strName);
        if (null == pDataInfo)
            return;

        if (true == pDataInfo.m_bIsDone)
            return;

        m_dicLoadingDatum[strName] = pDataInfo;
    }

    public void SetLoadDoneInfo(string strName, SHLoadEndInfo pLoadEndInfo)
    {
        var pDataInfo = GetLoadDataInfo(strName);
        if (null == pDataInfo)
        {
            Debug.LogError(string.Format("[LSH] 추가되지 않은 파일이 로드 되었다고 합니다~~({0})", strName));
            return;
        }

        pDataInfo.LoadDone(pLoadEndInfo);

        if (false == pLoadEndInfo.m_bIsSuccess)
        {
            if (false == m_dicLoadFailedDatum.ContainsKey(strName.ToLower()))
                m_dicLoadFailedDatum.Add(strName.ToLower(), pDataInfo);
        }
        else
        {
            if (false == m_dicLoadSucceedDatum.ContainsKey(strName.ToLower()))
                m_dicLoadSucceedDatum.Add(strName.ToLower(), pDataInfo);
        }

        if (true == m_dicLoadingDatum.ContainsKey(strName.ToLower()))
        {
            m_dicLoadingDatum.Remove(strName.ToLower());
        }

        if (true == IsDone())
        {
            LoadEnd();
        }
    }
    
    public void LoadStart()
    {
        m_pLoadStartTime = DateTime.Now;
    }

    public void LoadEnd()
    {
        m_pLoadEndTime = DateTime.Now;
    }

    public float GetLoadTime()
    {
        if (DateTimeKind.Unspecified == m_pLoadEndTime.Kind)
            return (float)((DateTime.Now - m_pLoadStartTime).TotalMilliseconds / 1000.0f);
        else
            return (float)((m_pLoadEndTime - m_pLoadStartTime).TotalMilliseconds / 1000.0f);
    }

    public bool IsFailed()
    {
        return (0 != m_dicLoadFailedDatum.Count);
    }

    public bool IsDone()
    {
        return ((0 == m_dicLoadingDatum.Count) && (0 == m_qLoadDatumWaitQueue.Count));
    }

    public bool IsDone(string strFileName)
    {
        var pData = GetLoadDataInfo(strFileName);
        if (null == pData)
            return true;

        return pData.IsDone();
    }

    public bool IsDone(eDataType eType)
    {
        if (false == m_dicAllLoadDatum.ContainsKey(eType))
            return true;
        
        foreach (var kvp in m_dicAllLoadDatum[eType])
        {
            if (false == kvp.Value.IsDone())
                return false;
        }

        return true;
    }

    public bool IsRemainLoadWaitQueue()
    {
        return (0 != m_qLoadDatumWaitQueue.Count);
    }
    
    private void AddLoadData(SHLoadData pData)
    {
        if (null == pData)
            return;

        if (false == m_dicAllLoadDatum.ContainsKey(pData.m_eDataType))
            m_dicAllLoadDatum.Add(pData.m_eDataType, new Dictionary<string, SHLoadDataStateInfo>());

        var pDataStateInfo = new SHLoadDataStateInfo(pData);
        m_qLoadDatumWaitQueue.Enqueue(pDataStateInfo);
        m_dicAllLoadDatum[pData.m_eDataType][pData.m_strName.ToLower()] = pDataStateInfo;
    }
}
