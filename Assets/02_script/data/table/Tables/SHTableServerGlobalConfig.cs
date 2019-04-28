using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerGlobalConfig : SHBaseTable
{
    public int m_iBasicMiningPowerCount = 0;
    public int m_iBasicChargeTime = 0;
    public int m_iBasicActiveMiningSupply = 0;
    public int m_iBasicRefreshTimeOUMining = 0;
    public int m_iCostUnitCompany1 = 0;
    public int m_iCostUnitCompany2 = 0;
    public int m_iCostUnitCompany3 = 0;
	
    public SHTableServerGlobalConfig()
    {
        m_strIdentity = "ServerGlobalConfig";
    }
    
    public override eErrorCode LoadServerTable(Action<eErrorCode> pCallback)
    {
        if (null == pCallback)
            return eErrorCode.Table_LoadFailed;

        Single.Network.GET(SHAPIs.SH_API_GET_CONFIG_TABLE, null, (reply) => 
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

        for (int iLoop = 0; iLoop < pJson.Count; ++iLoop)
        {
            m_iBasicMiningPowerCount = GetIntToJson(pJson[iLoop], "basic_mining_power_count");
            m_iBasicChargeTime = GetIntToJson(pJson[iLoop], "basic_charge_time");
            m_iBasicActiveMiningSupply = GetIntToJson(pJson[iLoop], "basic_active_mining_supply");
            m_iBasicRefreshTimeOUMining = GetIntToJson(pJson[iLoop], "basic_refresh_time_ou_mining");
            m_iCostUnitCompany1 = GetIntToJson(pJson[iLoop], "cost_unit_company1");
            m_iCostUnitCompany2 = GetIntToJson(pJson[iLoop], "cost_unit_company2");
            m_iCostUnitCompany3 = GetIntToJson(pJson[iLoop], "cost_unit_company3");
        }

        return eErrorCode.Succeed;
    }
}
