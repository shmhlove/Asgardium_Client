using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHResourcesInfo
{
    public string           m_strName;             // 확장자가 없는 이름
    public string           m_strFileName;         // 확장자가 있는 이름
    public string           m_strExtension;        // 확장자
    public string           m_strSize;             // 파일크기
    public string           m_strHash;             // 해시
    public string           m_strPath;             // Resources폴더 이하 경로
    public eResourceType    m_eResourceType;       // 리소스 타입
}

public class SHTableClientResources : SHBaseTable
{
    Dictionary<string, SHResourcesInfo> m_pData = new Dictionary<string, SHResourcesInfo>();

    public SHTableClientResources()
    {
        m_strIdentity = "ResourcesInfo";
    }
    
    public override void Initialize()
    {
        m_pData.Clear();
    }
    
    public override void LoadJson(Action<eErrorCode> pCallback)
    {
        var pTextAsset = Resources.Load<TextAsset>(string.Format("Table/Json/{0}", GetFileName(eTableLoadType.Json)));
        if (null == pTextAsset)
        {
            pCallback(eErrorCode.Table_Not_ExsitFile);
        }
        else
        {
            Initialize();
            var errorCode = LoadJsonTable(JsonMapper.ToObject(pTextAsset.text));
            m_bIsLoaded = errorCode == eErrorCode.Succeed;
            pCallback(errorCode);
        }
    }

    public override eErrorCode LoadJsonTable(JsonData pJson)
    {
        if (null == pJson)
        {
            return eErrorCode.Table_LoadFailed;
        }
        
        int iMaxTable = pJson.Count;
        for (int iLoop = 0; iLoop < iMaxTable; ++iLoop)
        {
            var pDataNode               = pJson[iLoop];
            SHResourcesInfo pData       = new SHResourcesInfo();
            pData.m_strName             = GetStrToJson(pDataNode, "s_Name");
            pData.m_strFileName         = GetStrToJson(pDataNode, "s_FileName");
            pData.m_strExtension        = GetStrToJson(pDataNode, "s_Extension");
            pData.m_strSize             = GetStrToJson(pDataNode, "s_Size");
            pData.m_strHash             = GetStrToJson(pDataNode, "s_Hash");
            pData.m_strPath             = GetStrToJson(pDataNode, "s_Path");
            pData.m_eResourceType       = SHUtils.GetResourceTypeByExtension(pData.m_strExtension);

            AddResources(pData.m_strName, pData);
        }

        return eErrorCode.Succeed;
    }
    
    public SHResourcesInfo GetResouceInfo(string strName)
    {
        if (false == IsLoadTable())
        {
            LoadJson((errorCode) => { });
        }

        strName = strName.ToLower().Trim();
        if (false == m_pData.ContainsKey(strName))
            return null;

        return m_pData[strName];
    }
    
    void AddResources(string strKey, SHResourcesInfo pData)
    {
        m_pData[strKey.ToLower().Trim()] = pData;
    }
}