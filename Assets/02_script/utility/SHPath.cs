using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;

public static partial class SHPath
{
    // 경로 : (Project Root)
    public static string GetRoot()
    {
        return Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets") - 1);
    }

    // 경로 : (Assets)
    public static string GetAssets()
    {
        return Application.dataPath;
    }

    // 경로 : (Assets/Resources)
    public static string GetResources()
    {
        return string.Format("{0}/{1}", GetAssets(), "Resources");
    }

    // 경로 : (Assets/StreamingAssets)
    public static string GetStreamingAssets()
    {
        return Application.streamingAssetsPath;
    }

    // 경로 : (Build)
    public static string GetBuild()
    {
        return string.Format("{0}/{1}", GetRoot(), "Build");
    }
    
    // 경로 : (사용자디렉토리 : /AppData/LocalLow/회사이름/프로덕트이름/플랫폼)
    public static string GetPersistentData()
    {
#if UNITY_EDITOR
        return string.Format("{0}/{1}", Application.persistentDataPath, SHUtils.GetPlatformStringByEnum(EditorUserBuildSettings.activeBuildTarget));
#else
        return string.Format("{0}/{1}", Application.persistentDataPath, SHUtils.GetPlatformStringByEnum(Single.AppInfo.GetRuntimePlatform()));
#endif
    }

    // 경로 : (사용자디렉토리 : /AppData/LocalLow/회사이름/프로덕트이름/플랫폼/Bytes)
    public static string GetPersistentDataBytes()
    {
        return string.Format("{0}/{1}", SHPath.GetPersistentData(), "Byte");
    }

    // 경로 : (사용자디렉토리 : /AppData/LocalLow/회사이름/프로덕트이름/플랫폼/Json)
    public static string GetPersistentDataJson()
    {
        return string.Format("{0}/{1}", SHPath.GetPersistentData(), "Json");
    }

    // 경로 : (사용자디렉토리 : /AppData/LocalLow/회사이름/프로덕트이름/플랫폼/Json)
    public static string GetPersistentDataXML()
    {
        return string.Format("{0}/{1}", SHPath.GetPersistentData(), "XML");
    }
    
    // 경로 : (Assets/Resources/Table/XML)
    public static string GetResourceXMLTable()
    {
        return string.Format("{0}/{1}/{2}", SHPath.GetResources(), "Table", "XML");
    }

    // 경로 : (Assets/Resources/Table/Byte)
    public static string GetResourceByteTable()
    {
        return string.Format("{0}/{1}/{2}", SHPath.GetResources(), "Table", "Byte");
    }

    // 경로 : (Assets/Resources/Table/Json)
    public static string GetResourceJsonTable()
    {
        return string.Format("{0}/{1}/{2}", SHPath.GetResources(), "Table", "Json");
    }

    // 경로 : (Assets/StreamingAssets/Json)
    public static string GetStreamingAssetsJsonTable()
    {
        return string.Format("{0}/{1}", SHPath.GetStreamingAssets(), "Json");
    }
}