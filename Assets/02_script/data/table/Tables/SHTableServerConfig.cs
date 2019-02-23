using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerConfig : SHBaseTable
{
    public int BasicMiningPower = 0;
    public int BasicChargeTime = 0;
	
    public SHTableServerConfig()
    {
        m_strIdentity = "ServerConfig";
    }
    
    public override eErrorCode LoadServerTable(Action<eErrorCode> pCallback)
    {
        if (null == pCallback)
            return eErrorCode.Table_LoadFailed;

        Single.Network.GET(SHAPIs.SH_API_GET_CONFIG, null, (reply) => 
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
        
        BasicMiningPower = GetIntToJson(pJson, "basic_mining_power");
        BasicChargeTime = GetIntToJson(pJson, "basic_charge_time");

        return eErrorCode.Succeed;
    }
}
