using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerUserInventory : SHBaseTable
{
    public string UserId = string.Empty;
    public long MiningPowerAt = 0;
	public Dictionary<int, int> HasUnits = new Dictionary<int, int>();

    public SHTableServerUserInventory()
    {
        m_strIdentity = "ServerUserInventory";
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
        MiningPowerAt = GetLongToJson(pJson, "mining_power_at");

        HasUnits.Clear();
        JsonData jsonHasUnits = pJson["has_units"];
        for (int iLoop = 0; iLoop < jsonHasUnits.Count; ++iLoop)
        {
            var unitId = GetIntToJson(jsonHasUnits[iLoop], "unit_id");
            var quantity = GetIntToJson(jsonHasUnits[iLoop], "quantity");
            HasUnits.Add(unitId, quantity);
        }
        
        m_bIsLoaded = true;

        return eErrorCode.Succeed;
    }

    public void RequestGetUserInventory(Action<SHReply> pCallback)
    {
        Single.Network.POST(SHAPIs.SH_API_USER_GET_INVENTORY, null, (reply) =>
        {
            if (reply.isSucceed)
            {
                LoadJsonTable(reply.data);
            }
            
            pCallback(reply);
        });
    }
}