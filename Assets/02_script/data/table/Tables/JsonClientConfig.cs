using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class JsonClientConfig : SHBaseTable
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

    public bool   VSyncCount       = false;
    public int    FrameRate        = 60;
    public int    CacheSize        = 200;

    public JsonClientConfig()
    {
        m_strFileName = "JsonClientConfig";
    }

    public override bool IsLoadTable()
    {
        return (false == string.IsNullOrEmpty(Version));
    }

    public override eErrorCode LoadJsonTable(JsonData pJson, string strFileName)
    {
        if (null == pJson)
            return eErrorCode.Table_Load_Fail;

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

        VSyncCount = GetBoolToJson(pDataNode, "VSyncCount");
        FrameRate = GetIntToJson(pDataNode, "FrameRate");
        CacheSize = GetIntToJson(pDataNode, "CacheSize");
        
        return eErrorCode.Succeed;
    }

    //public void SaveJsonFile()
    //{
    //    SaveJsonFile(SHPath.GetPathToJson());
    //}
    //public void SaveJsonFile(string strSavePath)
    //{
    //    var pClientConfigJsonData = new JsonData();

    //    pClientConfigJsonData["ServerConfigURL"] = ServerConfigURL;
    //    pClientConfigJsonData["ServiceMode"] = ServiceMode;
    //    pClientConfigJsonData["Version"] = Version;

    //    pClientConfigJsonData["AOS_KeyStoreName"] = AOS_KeyStoreName;
    //    pClientConfigJsonData["AOS_KeyStorePass"] = AOS_KeyStorePass;
    //    pClientConfigJsonData["AOS_KeyAliasName"] = AOS_KeyAliasName;
    //    pClientConfigJsonData["AOS_KeyAliasPass"] = AOS_KeyAliasPass;

    //    pClientConfigJsonData["IOS_TeamID"] = IOS_TeamID;

    //    pClientConfigJsonData["VSyncCount"] = VSyncCount;
    //    pClientConfigJsonData["FrameRate"] = FrameRate;
    //    pClientConfigJsonData["CacheSize"] = CacheSize;
    
    //    var pJsonWriter = new JsonWriter();
    //    pJsonWriter.PrettyPrint = true;
    //    JsonMapper.ToJson(pClientConfigJsonData, pJsonWriter);

    //    SHUtils.SaveFile(pJsonWriter.ToString(), string.Format("{0}/{1}.json", strSavePath, m_strFileName));
    //}
}