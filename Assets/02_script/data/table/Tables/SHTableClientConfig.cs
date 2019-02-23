using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableClientConfig : SHBaseTable
{
    public string ServerConfigURL  = string.Empty;
    public string ServiceMode      = string.Empty;
    public string Version          = string.Empty;
	
    public string AOS_KeyStoreName = string.Empty;
    public string AOS_KeyStorePass = string.Empty;
    public string AOS_KeyAliasName = string.Empty;
    public string AOS_KeyAliasPass = string.Empty;

    public string IOS_TeamID       = string.Empty;

    public string FB_StorageBaseURL = string.Empty;

    public int    FrameRate        = 60;

    public SHTableClientConfig()
    {
        m_strIdentity = "ClientConfig";
    }

    public override eErrorCode LoadJsonTable(JsonData pJson)
    {
        if (null == pJson)
            return eErrorCode.Table_LoadFailed;

        JsonData pDataNode = pJson[0];

        ServerConfigURL = GetStrToJson(pDataNode, "ServerConfigURL");
        ServiceMode = GetStrToJson(pDataNode, "ServiceMode");
        Version = GetStrToJson(pDataNode, "Version");
        
        AOS_KeyStoreName = GetStrToJson(pDataNode, "AOS_KeyStoreName");
        AOS_KeyStorePass = GetStrToJson(pDataNode, "AOS_KeyStorePass");
        AOS_KeyAliasName = GetStrToJson(pDataNode, "AOS_KeyAliasName");
        AOS_KeyAliasPass = GetStrToJson(pDataNode, "AOS_KeyAliasPass");
        
        IOS_TeamID = GetStrToJson(pDataNode, "IOS_TeamID");

        FB_StorageBaseURL = GetStrToJson(pDataNode, "FB_StorageBaseURL");

        FrameRate = GetIntToJson(pDataNode, "FrameRate");
        
        return eErrorCode.Succeed;
    }
}