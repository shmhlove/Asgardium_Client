using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerMiningActiveQuantityData
{
    public int m_iLevel;
    public int m_iCostUnitPerWeight;
    public int m_iQuantity;
}

public class SHTableServerMiningActiveQuantity : SHBaseTable
{
    public Dictionary<int, SHTableServerMiningActiveQuantityData> m_dicDatas = new Dictionary<int, SHTableServerMiningActiveQuantityData>();
	
    public SHTableServerMiningActiveQuantity()
    {
        m_strIdentity = "ServerMiningActiveQuantity";
    }
    
    public override eErrorCode LoadServerTable(Action<eErrorCode> pCallback)
    {
        if (null == pCallback)
            return eErrorCode.Table_LoadFailed;

        Single.Network.GET(SHAPIs.SH_API_GET_MINING_ACTIVE_QUANTITY_TABLE, null, (reply) => 
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
            var pData = new SHTableServerMiningActiveQuantityData();
            pData.m_iLevel = GetIntToJson(pJson[iLoop], "level");
            pData.m_iCostUnitPerWeight = GetIntToJson(pJson[iLoop], "cost_unit_per_weight");
            pData.m_iQuantity = GetIntToJson(pJson[iLoop], "quantity");
            m_dicDatas.Add(pData.m_iLevel, pData);
        }
        
        return eErrorCode.Succeed;
    }

    public SHTableServerMiningActiveQuantityData GetData(int iLevel)
    {
        if (false == m_dicDatas.ContainsKey(iLevel))
            return new SHTableServerMiningActiveQuantityData();

        return m_dicDatas[iLevel];
    }
}
