using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerInstanceMiningActiveCompanyData
{
    public string m_strInstanceId;
    public int m_iUnitId;
    public int m_iNameStrid;
    public string m_strEmblemImage;
    public int m_iEfficiencyLV;
    public int m_iSupplyCount;
    public bool m_bIsBasicCompany;
}

public class SHTableServerInstanceMiningActiveCompany : SHBaseTable
{
    public Dictionary<string, SHTableServerInstanceMiningActiveCompanyData> m_dicDatas = new Dictionary<string, SHTableServerInstanceMiningActiveCompanyData>();
	
    public SHTableServerInstanceMiningActiveCompany()
    {
        m_strIdentity = "ServerInstanceMiningActiveCompany";
    }
    
    public override eErrorCode LoadServerTable(Action<eErrorCode> pCallback)
    {
        if (null == pCallback)
            return eErrorCode.Table_LoadFailed;

        Single.Network.GET(SHAPIs.SH_API_GET_MINING_ACTIVE_COMPANY_TABLE, null, (reply) => 
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
            var pData = new SHTableServerInstanceMiningActiveCompanyData();
            pData.m_strInstanceId = GetStrToJson(pJson[iLoop], "instance_id");
            pData.m_iUnitId = GetIntToJson(pJson[iLoop], "unit_id");
            pData.m_iNameStrid = GetIntToJson(pJson[iLoop], "name_str_id");
            pData.m_strEmblemImage = GetStrToJson(pJson[iLoop], "emblem_image");
            pData.m_iEfficiencyLV = GetIntToJson(pJson[iLoop], "efficiency_lv");
            pData.m_iSupplyCount = GetIntToJson(pJson[iLoop], "supply_count");
            pData.m_bIsBasicCompany = GetBoolToJson(pJson[iLoop], "is_basic_company");
            
            m_dicDatas.Add(pData.m_strInstanceId, pData);
        }
        
        return eErrorCode.Succeed;
    }
}
