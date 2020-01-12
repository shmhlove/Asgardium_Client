using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerUserUpgradeInfo : SHBaseTable
{
    public string UserId;
    public int MiningPowerLv;
    public int ChargeTimeLv;

    public SHTableServerUserUpgradeInfo()
    {
        m_strIdentity = "ServerUserUpgradeInfo";
    }

    public override bool IsLoadTable()
    {
        return true;
    }

    public override eErrorCode LoadJsonTable(JsonData pJson)
    {
        if (null == pJson)
            return eErrorCode.Table_LoadFailed;
        
        UserId = GetStrToJson(pJson, "user_id");
        MiningPowerLv = GetIntToJson(pJson, "mining_power_lv");
        ChargeTimeLv = GetIntToJson(pJson, "charge_time_lv");

        m_bIsLoaded = true;

        return eErrorCode.Succeed;
    }

    public void RequestGetUserUpgradeInfo(Action<SHReply> pCallback)
    {
        Single.Network.POST(SHAPIs.SH_API_USER_GET_UPGRADE_INFO, null, (reply) =>
        {
            if (reply.isSucceed)
            {
                LoadJsonTable(reply.data);
            }
            
            pCallback(reply);
        });
    }
}