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
        if ((null == pJson) || (0 == pJson.Count))
        {
            return eErrorCode.Table_LoadFailed;
        }
        
        UserId = GetStrToJson(pJson, "user_id");
        UserEmail = GetStrToJson(pJson, "user_email");
        UserName = GetStrToJson(pJson, "user_name");
        Password = GetStrToJson(pJson, "password");
        CreatedAt = GetLongToJson(pJson, "created_at");
        UpdatedAt = GetLongToJson(pJson, "updated_at");

        m_bIsLoaded = true;

        return eErrorCode.Succeed;
    }
    
    public bool IsLogin()
    {
        return (false == string.IsNullOrEmpty(UserId));
    }
    
    public void RequestLoginForDevelop(Action<SHReply> pCallback)
    {
        if (true == m_bIsLoaded)
        {
            pCallback(new SHReply());
            return;
        }
        
        JsonData json = new JsonData
        {
            ["email"] = "shmhlove@naver.com",
            ["name"] = "이상호",
            ["password"] = "1234"
        };

        Single.Network.POST(SHAPIs.SH_API_AUTH_IS_SIGNUP, json, (isSignupReply) =>
        {
            if (false == isSignupReply.isSucceed)
            {
                pCallback(isSignupReply);
                return;
            }
            
            if (true == GetBoolToJson(isSignupReply.data, "is_signup"))
            {
                Single.Network.POST(SHAPIs.SH_API_AUTH_SIGNIN, json, (signinReply) =>
                {
                    pCallback(signinReply);
                });
            }
            else
            {
                Single.Network.POST(SHAPIs.SH_API_AUTH_SIGNUP, json, (signupReply) =>
                {
                    if (signupReply.isSucceed)
                    {
                        Single.Network.POST(SHAPIs.SH_API_AUTH_SIGNIN, json, (signinReply) =>
                        {
                            pCallback(signinReply);
                        });
                    }
                    else
                    {
                        pCallback(signupReply);
                    }
                });
            }
        });
    }
}