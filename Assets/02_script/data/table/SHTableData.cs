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
        //m_dicTables.Add(typeof(JsonAssetBundleInfo),  new JsonAssetBundleInfo());
    }

    public override void OnFinalize()
    {
        m_dicTables.Clear();
    }
    
    public override Dictionary<string, SHLoadData> GetLoadList(eSceneType eType)
    {
        var dicLoadList = new Dictionary<string, SHLoadData>();
        
        SHUtils.ForToDic<Type, SHBaseTable>(m_dicTables, (pKey, pValue) =>
        {
            if (true == pValue.IsLoadTable())
                return;

            dicLoadList.Add(pValue.m_strFileName, CreateLoadInfo(pValue.m_strFileName));
        });

        return dicLoadList; 
    }

    public override IEnumerator Load(SHLoadData pInfo, 
                                     Action<string, SHLoadStartInfo> pStart,
                                     Action<string, SHLoadEndInfo> pDone)
    {
        pStart(pInfo.m_strName, new SHLoadStartInfo());

        var pTable = GetTable(pInfo.m_strName, false);
        if (null == pTable)
        {
            Debug.LogErrorFormat("[LSH] 등록된 테이블이 아닙니다.!!({0})", pInfo.m_strName);
            pDone(pInfo.m_strName, new SHLoadEndInfo(eErrorCode.Table_Not_AddClass));
            yield break;
        }

        SHUtils.ForToList(GetLoadOrder(pTable), (pLoadTable) =>
        {
            pDone(pInfo.m_strName, new SHLoadEndInfo(pLoadTable()));
        });
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

    public T GetTable<T>(bool bIsLoadCheck = true) where T : SHBaseTable
    {
        return GetTable(typeof(T), bIsLoadCheck) as T;
    }

    public SHBaseTable GetTable(Type pType, bool bIsLoadCheck = true)
    {
        if (0 == m_dicTables.Count)
            OnInitialize();

        if (false == m_dicTables.ContainsKey(pType))
            return null;

        var pTable = m_dicTables[pType];
        if (true == bIsLoadCheck)
        {
            if (false == pTable.IsLoadTable())
            {
                switch(pTable.GetTableType())
                {
                    case eTableType.Static: pTable.LoadStatic();                       break;
                    case eTableType.Byte:   pTable.LoadByte(pTable.m_strByteFileName); break;
                    case eTableType.XML:    pTable.LoadXML(pTable.m_strFileName);      break;
                    case eTableType.Json:   pTable.LoadJson(pTable.m_strFileName);     break;
                }
            }
        }

        return pTable;
    }

    public SHBaseTable GetTable(string strFileName, bool bIsLoadCheck = true)
    {
        if (true == string.IsNullOrEmpty(strFileName))
            return null;

        return GetTable(GetTypeToFileName(strFileName), bIsLoadCheck);
    }
    
    public Type GetTypeToFileName(string strFileName)
    {
        strFileName = Path.GetFileNameWithoutExtension(strFileName);
        foreach (var kvp in m_dicTables)
        {
            if (true == kvp.Value.m_strFileName.Equals(strFileName))
                return kvp.Key;
        }

        return null;
    }
    
    List<Func<eErrorCode>> GetLoadOrder(SHBaseTable pTable)
    {
        var pLoadOrder = new List<Func<eErrorCode>>();
        //if (true == Single.AppInfo.IsEditorMode())
        //{
        //    pLoadOrder.Add(() => { return pTable.LoadStatic();                        });
        //    pLoadOrder.Add(() => { return pTable.LoadXML(pTable.m_strFileName);       });
        //    pLoadOrder.Add(() => { return pTable.LoadBytes(pTable.m_strByteFileName); });
        //    pLoadOrder.Add(() => { return pTable.LoadJson(pTable.m_strFileName);      });
        //}
        //else
        {
            pLoadOrder.Add(() => { return pTable.LoadStatic();                        });
            pLoadOrder.Add(() => { return pTable.LoadByte(pTable.m_strByteFileName);  });
            pLoadOrder.Add(() => { return pTable.LoadXML(pTable.m_strFileName);       });
            pLoadOrder.Add(() => { return pTable.LoadJson(pTable.m_strFileName);      });
        }

        return pLoadOrder;
    }
}