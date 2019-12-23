using UnityEngine;

using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class SHDataManager : SHSingleton<SHDataManager>
{
    private SHTableData     m_pTable = new SHTableData();
    public SHTableData      Table { get { return m_pTable; } }

    private SHResourceData  m_pResources = new SHResourceData();
    public SHResourceData   Resources { get { return m_pResources; } }

    private SHLoader        m_pLoader = new SHLoader();
    
    public override void OnInitialize()
    {
        Table.OnInitialize();
        Resources.OnInitialize();

        SetDontDestroy();
    }
    
    public override void OnFinalize() 
    {
        Table.OnFinalize();
        Resources.OnFinalize();
    }
    
    public void FixedUpdate()
    {
        Table.FrameMove();
        Resources.FrameMove();
    }
    
    public async Task Load(eSceneType eType, Action<SHLoadingInfo> pDone, Action<SHLoadingInfo> pProgress)
    {
        OnEventForLoadStart();

        await GetLoadList(eType, (pLoadList) => 
        {
            m_pLoader.Process(pLoadList, (pLoadInfo) => 
            {
                if (null != pDone)
                {
                    pDone(pLoadInfo);
                }
                
                OnEventForLoadDone();
            }, pProgress);
        });
    }
    
    public async Task Patch(Action<SHLoadingInfo> pDone, Action<SHLoadingInfo> pProgress)
    {
        await GetPatchList((pPatchList) => 
        {
            m_pLoader.Process(pPatchList, pDone, pProgress);
        });
    }
    
    public bool IsLoadDone()
    {
        return m_pLoader.IsLoadDone();
    }
    
    public bool IsLoadDone(string strFileName)
    {
        return m_pLoader.IsLoadDone(strFileName);
    }
    
    public bool IsLoadDone(eDataType eType)
    {
        return m_pLoader.IsLoadDone(eType);
    }
    
    private async Task GetLoadList(eSceneType eType, Action<List<Dictionary<string, SHLoadData>>> pCallback)
    {
        pCallback(new List<Dictionary<string, SHLoadData>>()
        {
            await Table.GetLoadList(eType),
            await Resources.GetLoadList(eType)
        });
    }
    
    private async Task GetPatchList(Action<List<Dictionary<string, SHLoadData>>> pCallback)
    {
        pCallback(new List<Dictionary<string, SHLoadData>>()
        {
            await Table.GetPatchList(),
            await Resources.GetPatchList()
        });
    }
    
    public void OnEventForLoadStart()
    {
        UnityEngine.Resources.UnloadUnusedAssets();
        
        for (int iLoop = 0; iLoop < System.GC.MaxGeneration; ++iLoop)
        {
            System.GC.Collect(iLoop, GCCollectionMode.Forced);
        }
    }
    
    public void OnEventForLoadDone()
    {
        UnityEngine.Resources.UnloadUnusedAssets();
        
        for (int iLoop = 0; iLoop < System.GC.MaxGeneration; ++iLoop)
        {
            System.GC.Collect(iLoop, GCCollectionMode.Forced);
        }
    }
}