using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerActiveMiningSupplyData
{
    public int m_iLevel;
    public int m_iCostUnit;
    public int m_iSupply;
}

public class SHTableServerActiveMiningSupply : SHBaseTable
{
    public Dictionary<int, SHTableServerActiveMiningSupplyData> m_dicDatas = new Dictionary<int, SHTableServerActiveMiningSupplyData>();
	
    public SHTableServerActiveMiningSupply()
    {
        m_strIdentity = "ServerActiveMiningSupply";
    }
    
    public override eErrorCode LoadServerTable(Action<eErrorCode> pCallback)
    {
        if (null == pCallback)
            return eErrorCode.Table_LoadFailed;

        Single.Network.GET(SHAPIs.SH_API_GET_ACTIVE_MINING_SUPPLY, null, (reply) => 
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
            var pData = new SHTableServerActiveMiningSupplyData();
            pData.m_iLevel = GetIntToJson(pJson[iLoop], "level");
            pData.m_iCostUnit = GetIntToJson(pJson[iLoop], "cost_unit");
            pData.m_iSupply = GetIntToJson(pJson[iLoop], "supply");
            m_dicDatas.Add(pData.m_iLevel, pData);
        }
        
        return eErrorCode.Succeed;
    }
}
