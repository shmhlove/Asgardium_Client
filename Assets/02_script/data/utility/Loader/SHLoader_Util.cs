using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHLoader
{
    void CoroutineToRun()
    {
        Load();

        if (false == IsRemainLoadFiles())
            return;

        Single.Coroutine.NextUpdate(CoroutineToRun);
    }
    
    void CoroutineToProgressEvent()
    {
        SendEventForProgress();

        if (true == IsLoaded())
            return;

        Single.Coroutine.NextUpdate(CoroutineToProgressEvent);
    }

    void Load()
    {
        var pDataInfo = m_pProgress.DequeueWaitingDataInfo();
        if (null == pDataInfo)
            return;

        if (false == pDataInfo.IsLoadOkayByTrigger())
        {
            m_pProgress.EnqueueWaitingDataInfo(pDataInfo);
            return;
        }
        
        pDataInfo.Load(OnEventForLoadStart, OnEventForLoadDone);
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
        EventForDone = pDone;
        EventForProgress = pProgress;
    }

    void OnEventForLoadStart(string strFileName, SHLoadStartInfo pData)
    {
        m_pProgress.SetLoadStartInfo(strFileName, pData);
    }

    void OnEventForLoadDone(string strFileName, SHLoadEndInfo pData)
    {
        m_pProgress.SetLoadDoneInfo(strFileName, pData);

        if (false == m_pProgress.IsDone())
        {
            return;
        }

        SendEventForProgress();
        SendEventForDone();
    }
    
    void SendEventForProgress()
    {
        EventForProgress?.Invoke(m_pProgress.GetLoadingInfo());
    }

    void SendEventForDone()
    {
        EventForDone?.Invoke(m_pProgress.GetLoadingInfo());
    }
    
    // 로드가 완료되었는가?(성공/실패유무가 아님)
    public bool IsLoaded()
    {
        return m_pProgress.IsDone();
    }

    // 특정 파일이 로드완료되었는가?(성공/실패유무가 아님)
    public bool IsLoaded(string strFileName)
    {
        return m_pProgress.IsDone(strFileName);
    }

    // 특정 타입이 로드완료되었는가?(성공/실패유무가 아님)
    public bool IsLoaded(eDataType eType)
    {
        return m_pProgress.IsDone(eType);
    }

    // 로드할 파일이 있는가?
    public bool IsRemainLoadFiles()
    {
        return m_pProgress.IsRemainLoadWaitQueue();
    }
}