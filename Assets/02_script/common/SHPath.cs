using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Net;
using System.Collections;

public static partial class SHPath
{
    // 경로 : 서버 URL
    public static string GetServerURL()
    {
        return string.Empty;
        //return Single.Table.GetServerURL();
    }
    
    // 경로 : (Project Root)
    public static string GetRoot()
    {
        return Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets") - 1);
    }

    // 경로 : (Root : Assets)
    public static string GetAssets()
    {
        return Application.dataPath;
    }

    // 경로 : (Root : Assets/Resources)
    public static string GetResources()
    {
        return string.Format("{0}/{1}", GetAssets(), "Resources");
    }

    // 경로 : (Root : Assets/StreamingAssets)
    public static string GetStreamingAssets()
    {
        return Application.streamingAssetsPath;
    }

    // 경로 : (Root : Build)
    public static string GetBuild()
    {
        return string.Format("{0}/{1}", GetRoot(), "Build");
    }
    
    // 경로 : (사용자디렉토리 : /AppData/LocalLow/회사이름/프로덕트이름/플랫폼)
    public static string GetPersistentData()
    {
#if UNITY_EDITOR
        return string.Format("{0}/{1}", Application.persistentDataPath, SHHard.GetPlatformStringByEnum(EditorUserBuildSettings.activeBuildTarget));
#else
        return string.Format("{0}/{1}", Application.persistentDataPath, SHHard.GetPlatformStringByEnum(Single.AppInfo.GetRuntimePlatform()));
#endif
    }

    // 경로 : (사용자디렉토리 : /AppData/LocalLow/회사이름/프로덕트이름/플랫폼/Bytes)
    public static string GetPersistentDataBytes()
    {
        return string.Format("{0}/{1}", SHPath.GetPersistentData(), "Bytes");
    }

    // 경로 : (사용자디렉토리 : /AppData/LocalLow/회사이름/프로덕트이름/플랫폼/Json)
    public static string GetPersistentDataJson()
    {
        return string.Format("{0}/{1}", SHPath.GetPersistentData(), "JSons");
    }

    // 경로 : (사용자디렉토리 : /AppData/LocalLow/회사이름/프로덕트이름/플랫폼/Json)
    public static string GetPersistentDataXML()
    {
        return string.Format("{0}/{1}", SHPath.GetPersistentData(), "XML");
    }
    
    // 경로 : (Root : Assets/Resources/Table/XML)
    public static string GetResourceXMLTable()
    {
        return string.Format("{0}/{1}/{2}", SHPath.GetResources(), "Table", "XML");
    }

    // 경로 : (Root : Assets/Resources/Table/Bytes)
    public static string GetResourceBytesTable()
    {
        return string.Format("{0}/{1}/{2}", SHPath.GetResources(), "Table", "Bytes");
    }

    // 경로 : (Root : Assets/Resources/Table/Json)
    public static string GetResourceJsonTable()
    {
        return string.Format("{0}/{1}/{2}", SHPath.GetResources(), "Table", "Json");
    }

    // 경로 : (Root : Assets/StreamingAssets/JSons)
    public static string GetStreamingAssetsJsonTable()
    {
        return string.Format("{0}/{1}", SHPath.GetStreamingAssets(), "JSons");
    }

    public static string GetPathToPersistentJson()
    {
        return string.Format("{0}/{1}", SHPath.GetPathToPersistentData(), "Json");
    }

    public static string GetPathToJson()
    {
        return string.Format("{0}/{1}", SHPath.GetPathToStreamingAssets(), "JSons");
    }

    // 경로 : (Root : Assets/StreamingAssets)
    public static string GetPathToStreamingAssets()
    {
        return Application.streamingAssetsPath;
    }

    public static string GetPathToPersistentData()
    {
// #if UNITY_EDITOR
//         return string.Format("{0}/{1}", Application.persistentDataPath, SHHard.GetStrToPlatform(UnityEditor.EditorUserBuildSettings.activeBuildTarget));
// #else
        return string.Format("{0}/{1}", Application.persistentDataPath, SHHard.GetStrToPlatform(Single.AppInfo.GetRuntimePlatform()));
//#endif
    }
}