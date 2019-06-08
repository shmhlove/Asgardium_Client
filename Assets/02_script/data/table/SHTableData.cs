using UnityEngine;

using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public partial class SHTableData : SHBaseData
{
    private Dictionary<Type, SHBaseTable> m_dicTables = new Dictionary<Type, SHBaseTable>();
    public Dictionary<Type, SHBaseTable> Tables { get { return m_dicTables; } }

    public override void OnInitialize()
    {
        m_dicTables.Clear();
        
        // 인스턴스 테이블
        m_dicTables.Add(typeof(SHTableUserInfo), new SHTableUserInfo());
        m_dicTables.Add(typeof(SHTableServerInstanceMiningActiveCompany), new SHTableServerInstanceMiningActiveCompany());

        // 클라 테이블
        m_dicTables.Add(typeof(SHTableClientConfig), new SHTableClientConfig());
        m_dicTables.Add(typeof(SHTableClientPreloadResources), new SHTableClientPreloadResources());
        m_dicTables.Add(typeof(SHTableClientResources), new SHTableClientResources());
        m_dicTables.Add(typeof(SHTableClientString), new SHTableClientString());

        // 서버 테이블
        m_dicTables.Add(typeof(SHTableServerGlobalConfig), new SHTableServerGlobalConfig());
        m_dicTables.Add(typeof(SHTableServerGlobalUnitData), new SHTableServerGlobalUnitData());
        m_dicTables.Add(typeof(SHTableServerMiningActiveCompanyNPC), new SHTableServerMiningActiveCompanyNPC());
        m_dicTables.Add(typeof(SHTableServerMiningActiveQuantity), new SHTableServerMiningActiveQuantity());
        m_dicTables.Add(typeof(SHTableServerMiningActiveSupply), new SHTableServerMiningActiveSupply());
        m_dicTables.Add(typeof(SHTableServerInventoryInfo), new SHTableServerInventoryInfo());
    }
    
    public override void OnFinalize()
    {
        m_dicTables.Clear();
    }
    
    public async override Task<Dictionary<string, SHLoadData>> GetLoadList(eSceneType eType)
    {
        return await Task.Run(() => 
        {
            var dicLoadList = new Dictionary<string, SHLoadData>();
            
            foreach (var kvp in m_dicTables)
            {
                if (true == kvp.Value.IsLoadTable())
                    continue;

                dicLoadList.Add(kvp.Value.m_strIdentity, CreateLoadInfo(kvp.Value));
            }

            return dicLoadList;
        });
    }

    public async override void Load
    (
        SHLoadData pInfo, 
        Action<string, SHLoadStartInfo> pStart,
        Action<string, SHLoadEndInfo> pDone
    )
    {
        pStart(pInfo.m_strName, new SHLoadStartInfo());
        
        var pTable = await GetTable(pInfo.m_strName);
        if (null == pTable)
        {
            Debug.LogErrorFormat("[LSH] 등록된 테이블이 아닙니다.!!({0})", pInfo.m_strName);
            pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Table_Not_AddedClass));
            return;
        }
        
        if (true == pTable.IsLoadTable())
        {
            pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Succeed));
        }
        else
        {
            pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Table_LoadFailed));
        }
    }
    
    public SHLoadData CreateLoadInfo(SHBaseTable pTable)
    {
        return new SHLoadData()
        {
            m_eDataType = pTable.m_eDataType,
            m_strName   = pTable.m_strIdentity,
            m_pLoadFunc = Load
        };
    }
    
    public bool IsLoaded<T>() where T : SHBaseTable
    {
        return m_dicTables.ContainsKey(typeof(T));
    }

    public T GetTableSync<T>() where T : SHBaseTable
    {
        // 이 함수는 이미 로드가 되었다는 가정하에 사용하는 함수이다.
        // 이 함수를 사용하는 곳에서는 항상 null 체크를 해야한다.

        if (false == m_dicTables.ContainsKey(typeof(T)))
        {
            return default;
        }

        return m_dicTables[typeof(T)] as T;
    }

    public async Task<T> GetTable<T>() where T : SHBaseTable
    {
        return await GetTable(typeof(T)) as T;
    }

    public async Task<SHBaseTable> GetTable(string strIdentity)
    {
        return await GetTable(GetTypeByIdentity(strIdentity));
    }

    public async Task<SHBaseTable> GetTable(Type pType)
    {
        if (0 == m_dicTables.Count)
        {
            OnInitialize();
        }

        var pPromise = new TaskCompletionSource<SHBaseTable>();

        if (false == m_dicTables.ContainsKey(pType))
        {
            pPromise.TrySetResult(null);
        }
        else
        {
            var pTable = m_dicTables[pType];

            if (true == pTable.IsLoadTable())
            {
                pPromise.TrySetResult(pTable);
            }
            else
            {
                void pAction(eErrorCode errorCode) { pPromise.TrySetResult(pTable); }
                switch (pTable.GetTableType())
                {
                    case eTableLoadType.Static: pTable.LoadStatic(pAction); break;
                    case eTableLoadType.Server: pTable.LoadServer(pAction); break;
                    case eTableLoadType.Byte:   pTable.LoadByte(pAction);   break;
                    case eTableLoadType.XML:    pTable.LoadXML(pAction);    break;
                    case eTableLoadType.Json:   pTable.LoadJson(pAction);   break;
                }
            }
        }

        return await pPromise.Task;
    }
    
    public Type GetTypeByIdentity(string strIdentity)
    {
        strIdentity = Path.GetFileNameWithoutExtension(strIdentity);
        foreach (var kvp in m_dicTables)
        {
            if (true == kvp.Value.m_strIdentity.Equals(strIdentity))
                return kvp.Key;
        }

        return null;
    }
}