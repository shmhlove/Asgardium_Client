using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerMiningActiveRechargeDelayData
{
    public int m_iLevel;
    public int m_iCostGold;
    public int m_iDelay;
}

public class SHTableServerMiningActiveRechargeDelay : SHBaseTable
{
    public Dictionary<int, SHTableServerMiningActiveRechargeDelayData> m_dicDatas = new Dictionary<int, SHTableServerMiningActiveRechargeDelayData>();
	
    public SHTableServerMiningActiveRechargeDelay()
    {
        m_strIdentity = "ServerMiningActiveRechargeDelay";
    }
    
    public override eErrorCode LoadServerTable(Action<eErrorCode> pCallback)
    {
        if (null == pCallback)
            return eErrorCode.Table_LoadFailed;

        Single.Network.GET(SHAPIs.SH_API_GET_MINING_ACTIVE_RECHARGE_DELAY_TABLE, null, (reply) => 
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
            var pData = new SHTableServerMiningActiveRechargeDelayData();
            pData.m_iLevel = GetIntToJson(pJson[iLoop], "level");
            pData.m_iCostGold = GetIntToJson(pJson[iLoop], "cost_gold");
            pData.m_iDelay = GetIntToJson(pJson[iLoop], "delay");
            m_dicDatas.Add(pData.m_iLevel, pData);
        }
        
        return eErrorCode.Succeed;
    }
}
