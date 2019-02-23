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
    private Queue<SHLoadDataStateInfo> m_qLoadDataWaitQueue = new Queue<SHLoadDataStateInfo>();

    // 로드 진행 중인 데이터 정보<파일명, 파일정보>
    private Dictionary<string, SHLoadDataStateInfo> m_dicLoadingData = new Dictionary<string, SHLoadDataStateInfo>();

    // 로드 성공한 데이터 정보<파일명, 파일정보>
    private Dictionary<string, SHLoadDataStateInfo> m_dicLoadSucceedData = new Dictionary<string, SHLoadDataStateInfo>();

    // 로드 실패한 데이터 정보<파일명, 파일정보>
    private Dictionary<string, SHLoadDataStateInfo> m_dicLoadFailedData = new Dictionary<string, SHLoadDataStateInfo>();

    // 전체 데이터 정보 : <데이터 타입, <파일명, 파일정보>>
    private Dictionary<eDataType, Dictionary<string, SHLoadDataStateInfo>> m_dicAllLoadData = new Dictionary<eDataType, Dictionary<string, SHLoadDataStateInfo>>();

    // 로드 시작 시간
    public DateTime m_pLoadStartTime;

    // 로드 종료 시간
    public DateTime m_pLoadEndTime;
    
    public void Initialize()
    {
        m_qLoadDataWaitQueue.Clear();
        m_dicAllLoadData.Clear();
        m_dicLoadingData.Clear();
        m_dicLoadSucceedData.Clear();
        m_dicLoadFailedData.Clear();
    }

    public void AddLoadDatas(Dictionary<string, SHLoadData> dicLoadData)
    {
        foreach (var kvp in dicLoadData)
        {
            if (null == kvp.Value)
                return;

            var pLoadData = GetLoadDataInfo(kvp.Key);
            if (null != pLoadData)
            {
                Debug.LogErrorFormat("[LSH] 데이터 로드 중 중복파일 발견!!!(FileName : {0})", kvp.Key);
            }
            else
            {
                AddLoadData(kvp.Value);
            }
        }
    }
    
    public SHLoadDataStateInfo GetLoadDataInfo(string strName)
    {
        foreach (var kvp in m_dicAllLoadData)
        {
            if (true == kvp.Value.ContainsKey(strName.ToLower()))
                return kvp.Value[strName.ToLower()];
        }
        return null;
    }
    
    public void EnqueueWaitingDataInfo(SHLoadDataStateInfo pLoadDataInfo)
    {
        if (true == m_qLoadDataWaitQueue.Contains(pLoadDataInfo))
            return;

        m_qLoadDataWaitQueue.Enqueue(pLoadDataInfo);
    }

    public SHLoadDataStateInfo DequeueWaitingDataInfo()
    {
        if (0 == m_qLoadDataWaitQueue.Count)
            return null;

        var pDataInfo = m_qLoadDataWaitQueue.Dequeue();
        if (null == pDataInfo)
            return null;

        return pDataInfo;
    }

    public SHLoadingInfo GetLoadingInfo()
    {
        var pLoadingInfo = new SHLoadingInfo();

        // 로드 중인 데이터 정보들
        pLoadingInfo.m_pLoadingDatas   = new List<SHLoadDataStateInfo>(m_dicLoadingData.Values);

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
        return m_dicLoadSucceedData.Count;
    }

    public int GetLoadFailedCount()
    {
        return m_dicLoadFailedData.Count;
    }
    
    public int GetLoadDoneCount()
    {
        return (m_dicLoadSucceedData.Count + m_dicLoadFailedData.Count);
    }

    public int GetTotalCount()
    {
        int iTotalCount = 0;
        foreach(var kvp in m_dicAllLoadData)
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
        if (0 < m_dicLoadingData.Count)
        {
            foreach (var pData in m_dicLoadingData.Values)
            {
                fProgress += pData.GetProgress();
            }
            fProgress /= m_dicLoadingData.Count;
        }
        
        // 로드 완료된 파일의 진행률 반영
        if (0 < GetTotalCount())
        {
            fProgress += (GetLoadDoneCount() / GetTotalCount());
        }
        
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

        m_dicLoadingData[strName] = pDataInfo;
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
            if (false == m_dicLoadFailedData.ContainsKey(strName.ToLower()))
                m_dicLoadFailedData.Add(strName.ToLower(), pDataInfo);
        }
        else
        {
            if (false == m_dicLoadSucceedData.ContainsKey(strName.ToLower()))
                m_dicLoadSucceedData.Add(strName.ToLower(), pDataInfo);
        }

        if (true == m_dicLoadingData.ContainsKey(strName.ToLower()))
        {
            m_dicLoadingData.Remove(strName.ToLower());
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
        return (0 != m_dicLoadFailedData.Count);
    }

    public bool IsDone()
    {
        return ((0 == m_dicLoadingData.Count) && (0 == m_qLoadDataWaitQueue.Count));
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
        if (false == m_dicAllLoadData.ContainsKey(eType))
            return true;
        
        foreach (var kvp in m_dicAllLoadData[eType])
        {
            if (false == kvp.Value.IsDone())
                return false;
        }

        return true;
    }

    public bool IsRemainLoadWaitQueue()
    {
        return (0 != m_qLoadDataWaitQueue.Count);
    }
    
    private void AddLoadData(SHLoadData pData)
    {
        if (null == pData)
            return;

        if (false == m_dicAllLoadData.ContainsKey(pData.m_eDataType))
            m_dicAllLoadData.Add(pData.m_eDataType, new Dictionary<string, SHLoadDataStateInfo>());

        var pDataStateInfo = new SHLoadDataStateInfo(pData);
        m_qLoadDataWaitQueue.Enqueue(pDataStateInfo);
        m_dicAllLoadData[pData.m_eDataType][pData.m_strName.ToLower()] = pDataStateInfo;
    }
}
