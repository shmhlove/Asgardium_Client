using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerMiningActiveCompanyNPCData
{
    public int m_iUnitId;
    public int m_iNameStrid;
    public int m_iEmblemId;
    public int m_iEfficiencyLV;
    public int m_iSupplyLV;
}

public class SHTableServerMiningActiveCompanyNPC : SHBaseTable
{
    public List<SHTableServerMiningActiveCompanyNPCData> m_pDatas = new List<SHTableServerMiningActiveCompanyNPCData>();
	
    public SHTableServerMiningActiveCompanyNPC()
    {
        m_strIdentity = "ServerMiningActiveCompanyNPC";
    }
    
    public override eErrorCode LoadServerTable(Action<eErrorCode> pCallback)
    {
        if (null == pCallback)
            return eErrorCode.Table_LoadFailed;

        Single.Network.GET(SHAPIs.SH_API_GET_MINING_ACTIVE_COMPANY_NPC_TABLE, null, (reply) => 
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

        m_pDatas.Clear();

        for (int iLoop = 0; iLoop < pJson.Count; ++iLoop)
        {
            var pData = new SHTableServerMiningActiveCompanyNPCData();
            pData.m_iUnitId = GetIntToJson(pJson[iLoop], "unit_id");
            pData.m_iNameStrid = GetIntToJson(pJson[iLoop], "name_str_id");
            pData.m_iEmblemId = GetIntToJson(pJson[iLoop], "emblem_id");
            pData.m_iEfficiencyLV = GetIntToJson(pJson[iLoop], "efficiency_lv");
            pData.m_iSupplyLV = GetIntToJson(pJson[iLoop], "supply_lv");
            m_pDatas.Add(pData);
        }

        return eErrorCode.Succeed;
    }
}
