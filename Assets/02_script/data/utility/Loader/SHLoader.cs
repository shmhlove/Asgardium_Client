using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHLoader
{
    public void Process(SHLoadData pLoadInfo,
        Action<SHLoadingInfo> pDone = null, Action<SHLoadingInfo> pProgress = null)
    {
        Process(new Dictionary<string, SHLoadData>()
        {
            { pLoadInfo.m_strName, pLoadInfo}
        }, 
        pDone, pProgress);
    }

    public void Process(Dictionary<string, SHLoadData> pLoadList,
        Action<SHLoadingInfo> pDone = null, Action<SHLoadingInfo> pProgress = null)
    {
        Process(new List<Dictionary<string, SHLoadData>>()
        {
            pLoadList
        }, 
        pDone, pProgress);
    }

    public void Process(List<Dictionary<string, SHLoadData>> pLoadList,
        Action<SHLoadingInfo> pDone = null, Action<SHLoadingInfo> pProgress = null)
    {
        Initialize();
        
        AddLoadDatum(pLoadList);
        AddLoadEvent(pDone, pProgress);
        
        if (false == IsRemainLoadFiles())
        {
            CallEventToDone();
            return;
        }

        m_pProgress.LoadStart();
        
        CoroutineToLoadProcess();
        CoroutineToLoadProgressEvent();
    }
}