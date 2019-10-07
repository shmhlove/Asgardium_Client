using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerUserUpgradeInfo : SHBaseTable
{
    public string UserId;
    public long MiningPowerLv;
    public long ChargeTimeLv;

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
        MiningPowerLv = GetLongToJson(pJson, "mining_power_lv");
        ChargeTimeLv = GetLongToJson(pJson, "charge_time_lv");

        m_bIsLoaded = true;

        return eErrorCode.Succeed;
    }

    public void RequestGetUserUpgradeInfo(string strUserId, Action<SHReply> pCallback)
    {
        JsonData json = new JsonData
        {
            ["user_id"] = strUserId
        };
        Single.Network.POST(SHAPIs.SH_API_USER_GET_UPGRADE_INFO, json, (reply) =>
        {
            if (reply.isSucceed)
            {
                LoadJsonTable(reply.data);
            }
            
            pCallback(reply);
        });
    }
}