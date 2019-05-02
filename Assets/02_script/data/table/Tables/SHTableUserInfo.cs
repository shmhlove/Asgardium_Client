using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableUserInfo : SHBaseTable
{
    public string UserId = string.Empty;
    public string UserEmail = string.Empty;
    public string UserName = string.Empty;
    public string Password = string.Empty;
    public long CreatedAt = 0;
    public long UpdatedAt = 0;
    public long MiningPowerAt = 0;
	
    public SHTableUserInfo()
    {
        m_strIdentity = "UserInfo";
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

        m_bIsLoaded = true;

        return eErrorCode.Succeed;
    }
    
    public bool IsLogin()
    {
        return (false == string.IsNullOrEmpty(UserId));
    }

    public void CheckUserInfoLoadedForDevelop()
    {
        if (true == m_bIsLoaded)
            return;
        
        JsonData json = new JsonData
        {
            ["email"] = "shmhlove@naver.com",
            ["password"] = "1234"
        };
        Single.Network.POST(SHAPIs.SH_API_SIGNIN, json, (reply) =>
        {
            if (reply.isSucceed)
            {
                LoadJsonTable(reply.data);
            }
            else
            {
                Single.BusinessGlobal.ShowAlertUI(reply);
            }
        });
    }
}