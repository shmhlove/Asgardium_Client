using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class JsonPreloadResources : SHBaseTable
{
    Dictionary<eSceneType, List<string>> m_pData = new Dictionary<eSceneType, List<string>>();
    
    public JsonPreloadResources()
    {
        m_strFileName = "JsonPreloadResources";
    }
    
    public override void Initialize()
    {
        m_pData.Clear();
    }

    public override bool IsLoadTable()
    {
        return (0 != m_pData.Count);
    }

    public override eErrorCode LoadJsonTable(JsonData pJson, string strFileName)
    {
        if (null == pJson)
            return eErrorCode.Table_Load_Fail;

        int iMaxTable = pJson.Count;
        for (int iLoop = 0; iLoop < iMaxTable; ++iLoop)
        {
            var pDataNode = pJson[iLoop];
            SHUtils.ForToEnum<eSceneType>((eType) => 
            {
                for (int iDataIndex = 0; iDataIndex < pDataNode[eType.ToString()].Count; ++iDataIndex)
                {
                    AddData(eType, (string)pDataNode[eType.ToString()][iDataIndex]);
                }
            });
        }

        return eErrorCode.Succeed;
    }
    
    public List<string> GetData(eSceneType eType)
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        if (false == m_pData.ContainsKey(eType))
            return new List<string>();

        return m_pData[eType];
    }
    
    void AddData(eSceneType eType, string strData)
    {
        if (false == m_pData.ContainsKey(eType))
            m_pData.Add(eType, new List<string>());

        strData = strData.ToLower();
        m_pData[eType].Add(strData);
    }
}