using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHLoader
{
    void CoroutineToLoadProcess()
    {
        LoadCall();

        if (false == IsRemainLoadFiles())
            return;

        Single.Coroutine.NextUpdate(CoroutineToLoadProcess);
    }
    
    void CoroutineToLoadProgressEvent()
    {
        CallEventToProgress();

        if (true == IsLoadDone())
            return;

        Single.Coroutine.NextUpdate(CoroutineToLoadProgressEvent);
    }

    void LoadCall()
    {
        var pDataInfo = m_pProgress.DequeueWaitingDataInfo();
        if (null == pDataInfo)
            return;

        if (false == pDataInfo.IsLoadOkayByTrigger())
        {
            m_pProgress.EnqueueWaitingDataInfo(pDataInfo);
            return;
        }
        
        pDataInfo.LoadCall(OnEventToLoadStart, OnEventToLoadDone);
    }

    void AddLoadDatas(List<Dictionary<string, SHLoadData>> pLoadData)
    {
        foreach (var pData in pLoadData)
        {
            m_pProgress.AddLoadDatas(pData);
        }
    }

    void AddLoadEvent(Action<SHLoadingInfo> pDone, Action<SHLoadingInfo> pProgress)
    {
        EventToDone = pDone;
        EventToProgress = pProgress;
    }

    void OnEventToLoadStart(string strFileName, SHLoadStartInfo pData)
    {
        m_pProgress.SetLoadStartInfo(strFileName, pData);
    }

    void OnEventToLoadDone(string strFileName, SHLoadEndInfo pData)
    {
        m_pProgress.SetLoadDoneInfo(strFileName, pData);

        if (false == m_pProgress.IsDone())
        {
            return;
        }

        CallEventToProgress();
        CallEventToDone();
    }
    
    void CallEventToProgress()
    {
        if (null == EventToProgress)
            return;

        EventToProgress(m_pProgress.GetLoadingInfo());
    }

    void CallEventToDone()
    {
        if (null == EventToDone)
            return;

        EventToDone(m_pProgress.GetLoadingInfo());
    }
    
    // 로드가 완료되었는가?(성공/실패유무가 아님)
    public bool IsLoadDone()
    {
        return m_pProgress.IsDone();
    }

    // 특정 파일이 로드완료되었는가?(성공/실패유무가 아님)
    public bool IsLoadDone(string strFileName)
    {
        return m_pProgress.IsDone(strFileName);
    }

    // 특정 타입이 로드완료되었는가?(성공/실패유무가 아님)
    public bool IsLoadDone(eDataType eType)
    {
        return m_pProgress.IsDone(eType);
    }

    // 로드할 파일이 있는가?
    public bool IsRemainLoadFiles()
    {
        return m_pProgress.IsRemainLoadWaitQueue();
    }
}