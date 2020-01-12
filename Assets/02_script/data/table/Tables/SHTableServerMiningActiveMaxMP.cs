using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerMiningActiveMaxMPData
{
    public int m_iLevel;
    public int m_iCostGold;
    public int m_iMaxMP;
}

public class SHTableServerMiningActiveMaxMP : SHBaseTable
{
    public Dictionary<int, SHTableServerMiningActiveMaxMPData> m_dicDatas = new Dictionary<int, SHTableServerMiningActiveMaxMPData>();
	
    public SHTableServerMiningActiveMaxMP()
    {
        m_strIdentity = "ServerMiningActiveMaxMP";
    }
    
    public override eErrorCode LoadServerTable(Action<eErrorCode> pCallback)
    {
        if (null == pCallback)
            return eErrorCode.Table_LoadFailed;

        Single.Network.GET(SHAPIs.SH_API_GET_MINING_ACTIVE_MAX_MP_TABLE, null, (reply) => 
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
            var pData = new SHTableServerMiningActiveMaxMPData();
            pData.m_iLevel = GetIntToJson(pJson[iLoop], "level");
            pData.m_iCostGold = GetIntToJson(pJson[iLoop], "cost_gold");
            pData.m_iMaxMP = GetIntToJson(pJson[iLoop], "max_mp");
            m_dicDatas.Add(pData.m_iLevel, pData);
        }
        
        return eErrorCode.Succeed;
    }

    public SHTableServerMiningActiveMaxMPData GetData(int iLevel)
    {
        if (m_dicDatas.ContainsKey(iLevel))
        {
            return m_dicDatas[iLevel];
        }
        else
        {
            return null;
        }
    }
}
