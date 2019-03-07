using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerCompanyForMiningData
{
    public string m_strInstanceId;
    public int m_iResourceId;
    public int m_iNameStrid;
    public string m_strEmblemImage;
    public int m_iEfficiencyLV;
    public int m_iSupplyCount;
    public bool m_bIsBasicCompany;
}

public class SHTableServerCompanyForMining : SHBaseTable
{
    public Dictionary<string, SHTableServerCompanyForMiningData> m_dicDatas = new Dictionary<string, SHTableServerCompanyForMiningData>();
	
    public SHTableServerCompanyForMining()
    {
        m_strIdentity = "ServerCompanyForMining";
    }
    
    public override eErrorCode LoadServerTable(Action<eErrorCode> pCallback)
    {
        if (null == pCallback)
            return eErrorCode.Table_LoadFailed;

        Single.Network.GET(SHAPIs.SH_API_GET_COMPANY_FOR_MINING, null, (reply) => 
        {
            if (reply.isSucceed)
            {
                pCallback(LoadJsonTable(reply.data));
            }
            else
            {
                pCallback(eErrorCode.Table_LoadFailed);
            }
        });

        return eErrorCode.Succeed;
    }
    
    public override eErrorCode LoadJsonTable(JsonData pJson)
    {
        if (null == pJson)
            return eErrorCode.Table_LoadFailed;

        m_dicDatas.Clear();

        for (int iLoop = 0; iLoop < pJson.Count; ++iLoop)
        {
            var pData = new SHTableServerCompanyForMiningData();
            pData.m_strInstanceId = GetStrToJson(pJson[iLoop], "instance_id");
            pData.m_iResourceId = GetIntToJson(pJson[iLoop], "resource_id");
            pData.m_iNameStrid = GetIntToJson(pJson[iLoop], "name_strid");
            pData.m_strEmblemImage = GetStrToJson(pJson[iLoop], "emblem_image");
            pData.m_iEfficiencyLV = GetIntToJson(pJson[iLoop], "efficiency_lv");
            pData.m_iSupplyCount = GetIntToJson(pJson[iLoop], "supply_count");
            pData.m_bIsBasicCompany = GetBoolToJson(pJson[iLoop], "is_basic_company");
            
            m_dicDatas.Add(pData.m_strInstanceId, pData);
        }
        
        return eErrorCode.Succeed;
    }
}
