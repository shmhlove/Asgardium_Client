using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class JsonUserInfo : SHBaseTable
{
    public string UserId = string.Empty;
    public string UserEmail = string.Empty;
    public string UserName = string.Empty;
    public string Password = string.Empty;
    public long CreatedAt = 0;
    public long UpdatedAt = 0;
    public long MiningPowerAt = 0;
	
    public JsonUserInfo()
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
        
        UserId = GetStrToJson(pJson, "user_id");
        UserEmail = GetStrToJson(pJson, "user_email");
        UserName = GetStrToJson(pJson, "user_name");
        Password = GetStrToJson(pJson, "password");
        CreatedAt = GetLongToJson(pJson, "created_at");
        UpdatedAt = GetLongToJson(pJson, "updated_at");
        MiningPowerAt = GetLongToJson(pJson, "mining_power_at");

        return eErrorCode.Succeed;
    }
}