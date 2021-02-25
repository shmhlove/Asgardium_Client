using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHLoader
{
    public void Run(SHLoadData pLoadInfo,
        Action<SHLoadingInfo> pDoneCallback = null, Action<SHLoadingInfo> pProgressCallback = null)
    {
        Run(new Dictionary<string, SHLoadData>()
        {
            { pLoadInfo.m_strName, pLoadInfo}
        }, 
        pDoneCallback, pProgressCallback);
    }

    public void Run(Dictionary<string, SHLoadData> pLoadList,
        Action<SHLoadingInfo> pDoneCallback = null, Action<SHLoadingInfo> pProgressCallback = null)
    {
        Run(new List<Dictionary<string, SHLoadData>>()
        {
            pLoadList
        }, 
        pDoneCallback, pProgressCallback);
    }

    public void Run(List<Dictionary<string, SHLoadData>> pLoadList,
        Action<SHLoadingInfo> pDoneCallback = null, Action<SHLoadingInfo> pProgressCallback = null)
    {
        Initialize();
        
        AddLoadDatas(pLoadList);
        AddLoadEvent(pDoneCallback, pProgressCallback);
        
        if (false == IsRemainLoadFiles())
        {
            SendEventForDone();
            return;
        }

        m_pProgress.LoadStart();

        CoroutineToProgressEvent();
        CoroutineToRun();
    }
}