using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public partial class SHTableData : SHBaseData
{
    private Dictionary<Type, SHBaseTable> m_dicTables = new Dictionary<Type, SHBaseTable>();
    public Dictionary<Type, SHBaseTable> Tables { get { return m_dicTables; } }

    public override void OnInitialize()
    {
        m_dicTables.Clear();

        m_dicTables.Add(typeof(JsonClientConfig),     new JsonClientConfig());
        m_dicTables.Add(typeof(JsonPreloadResources), new JsonPreloadResources());
        m_dicTables.Add(typeof(JsonResources),        new JsonResources());
    }

    public override void OnFinalize()
    {
        m_dicTables.Clear();
    }
    
    public override void GetLoadList(eSceneType eType, Action<Dictionary<string, SHLoadData>> pCallback)
    {
        var dicLoadList = new Dictionary<string, SHLoadData>();
        
        foreach (var kvp in m_dicTables)
        {
            if (true == kvp.Value.IsLoadTable())
                continue;

            dicLoadList.Add(kvp.Value.m_strFileName, CreateLoadInfo(kvp.Value.m_strFileName));
        }

        pCallback(dicLoadList);
    }

    public override IEnumerator Load(SHLoadData pInfo, 
                                     Action<string, SHLoadStartInfo> pStart,
                                     Action<string, SHLoadEndInfo> pDone)
    {
        pStart(pInfo.m_strName, new SHLoadStartInfo());

        GetTable(pInfo.m_strName, (pTable) => 
        {
            if (null == pTable)
            {
                Debug.LogErrorFormat("[LSH] 등록된 테이블이 아닙니다.!!({0})", pInfo.m_strName);
                pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Table_Not_AddClass));
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
        });

        yield return null;
    }
    
    public SHLoadData CreateLoadInfo(string strName)
    {
        return new SHLoadData()
        {
            m_eDataType = eDataType.LocalTable,
            m_strName   = strName,
            m_pLoadFunc = Load
        };
    }

    public void GetTable(string strFileName, Action<SHBaseTable> pCallback)
    {
        if (true == string.IsNullOrEmpty(strFileName))
        {
            pCallback(null);
        }

        GetTable(GetTypeByFileName(strFileName), pCallback);
    }
    
    public void GetTable<T>(Action<T> pCallback) where T : SHBaseTable
    {
        GetTable(typeof(T), (pTable) => 
        {
            if (null == pTable)
            {
                pCallback(default(T));
            }
            else
            {
                pCallback(pTable as T);
            }
        });
    }

    public void GetTable(Type pType, Action<SHBaseTable> pCallback)
    {
        if (0 == m_dicTables.Count)
        {
            OnInitialize();
        }

        if (false == m_dicTables.ContainsKey(pType))
        {
            pCallback(null);
        }
        else
        {
            var pTable = m_dicTables[pType];

            if (true == pTable.IsLoadTable())
            {
                pCallback(pTable);
            }
            else
            {
                Action<eErrorCode> pAction = (errorCode) => { pCallback(pTable); };
                switch(pTable.GetTableType())
                {
                    case eTableType.Static: pTable.LoadStatic(pAction);                         break;
                    case eTableType.Byte:   pTable.LoadByte(pTable.m_strByteFileName, pAction); break;
                    case eTableType.XML:    pTable.LoadXML(pTable.m_strFileName, pAction);      break;
                    case eTableType.Json:   pTable.LoadJson(pTable.m_strFileName, pAction);     break;
                }
            }            
        }
    }
    
    public Type GetTypeByFileName(string strFileName)
    {
        strFileName = Path.GetFileNameWithoutExtension(strFileName);
        foreach (var kvp in m_dicTables)
        {
            if (true == kvp.Value.m_strFileName.Equals(strFileName))
                return kvp.Key;
        }

        return null;
    }
}