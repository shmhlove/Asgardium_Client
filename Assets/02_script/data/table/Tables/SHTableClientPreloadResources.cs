using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableClientPreloadResources : SHBaseTable
{
    Dictionary<eSceneType, List<string>> m_pData = new Dictionary<eSceneType, List<string>>();
    
    public SHTableClientPreloadResources()
    {
        m_strIdentity = "PreloadResources";
    }
    
    public override void Initialize()
    {
        m_pData.Clear();
    }

    public override eErrorCode LoadJsonTable(JsonData pJson)
    {
        if (null == pJson)
            return eErrorCode.Table_LoadFailed;

        int iMaxTable = pJson.Count;
        for (int iLoop = 0; iLoop < iMaxTable; ++iLoop)
        {
            var pDataNode = pJson[iLoop];
            SHUtils.ForToEnum<eSceneType>((eType) => 
            {
                if (false == pDataNode.Keys.Contains(eType.ToString()))
                    return;
                    
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
            LoadJson((errorCode) => {});

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