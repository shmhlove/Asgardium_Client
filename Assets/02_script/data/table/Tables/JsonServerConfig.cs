using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class JsonServerConfig : SHBaseTable
{
    public int BasicMiningPower = 0;
    public int BasicChargeTime = 0;
	
    public JsonServerConfig()
    {
    }
    
    public override bool IsLoadTable()
    {
        return true;
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
