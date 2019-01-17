using UnityEngine;

using System;
using System.IO;
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
    
    public void Load(eSceneType eType, Action<SHLoadingInfo> pDone, Action<SHLoadingInfo> pProgress)
    {
        OnEventToLoadStart();
        
        m_pLoader.Process(GetLoadList(eType), (pLoadInfo) => 
        {
            if (null != pDone)
            {
                pDone(pLoadInfo);
            }
            
            OnEventToLoadDone();
        }, pProgress);
    }
    
    public void Patch(Action<SHLoadingInfo> pDone, Action<SHLoadingInfo> pProgress)
    {
        m_pLoader.Process(GetPatchList(), pDone, pProgress);
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
    
    List<Dictionary<string, SHLoadData>> GetLoadList(eSceneType eType)
    {
        return new List<Dictionary<string, SHLoadData>>()
        {
            Table.GetLoadList(eType),
            Resources.GetLoadList(eType)
        };
    }
    
    List<Dictionary<string, SHLoadData>> GetPatchList()
    {
        return new List<Dictionary<string, SHLoadData>>()
        {
            Table.GetPatchList(),
            Resources.GetPatchList()
        };
    }
    
    public void OnEventToLoadStart()
    {
        UnityEngine.Resources.UnloadUnusedAssets();
        
        for (int iLoop = 0; iLoop < System.GC.MaxGeneration; ++iLoop)
        {
            System.GC.Collect(iLoop, GCCollectionMode.Forced);
        }
    }
    
    public void OnEventToLoadDone()
    {
        UnityEngine.Resources.UnloadUnusedAssets();
        
        for (int iLoop = 0; iLoop < System.GC.MaxGeneration; ++iLoop)
        {
            System.GC.Collect(iLoop, GCCollectionMode.Forced);
        }
    }
}