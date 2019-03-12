using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerMiningActiveSupplyData
{
    public int m_iLevel;
    public int m_iCostUnitPerWeight;
    public int m_iSupply;
}

public class SHTableServerMiningActiveSupply : SHBaseTable
{
    public Dictionary<int, SHTableServerMiningActiveSupplyData> m_dicDatas = new Dictionary<int, SHTableServerMiningActiveSupplyData>();
	
    public SHTableServerMiningActiveSupply()
    {
        m_strIdentity = "ServerMiningActiveSupply";
    }
    
    public override eErrorCode LoadServerTable(Action<eErrorCode> pCallback)
    {
        if (null == pCallback)
            return eErrorCode.Table_LoadFailed;

        Single.Network.GET(SHAPIs.SH_API_GET_MINING_ACTIVE_SUPPLY_TABLE, null, (reply) => 
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
            var pData = new SHTableServerMiningActiveSupplyData();
            pData.m_iLevel = GetIntToJson(pJson[iLoop], "level");
            pData.m_iCostUnitPerWeight = GetIntToJson(pJson[iLoop], "cost_unit_per_weight");
            pData.m_iSupply = GetIntToJson(pJson[iLoop], "supply");
            m_dicDatas.Add(pData.m_iLevel, pData);
        }
        
        return eErrorCode.Succeed;
    }
}
