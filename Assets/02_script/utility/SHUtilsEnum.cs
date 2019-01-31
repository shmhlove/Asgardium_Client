using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public static partial class SHUtils
{
    public static T GetStringToEnum<T>(string strEnum)
    {
        if (true == string.IsNullOrEmpty(strEnum))
        {
            UnityEngine.Debug.LogErrorFormat("[LSH] Enum String is null");
            return default(T);
        }

        if (false == Enum.IsDefined(typeof(T), strEnum))
        {
            UnityEngine.Debug.LogErrorFormat("[LSH] Not Enum String({0})", strEnum);
            return default(T);
        }
        
        return (T)Enum.Parse(typeof(T), strEnum);
    }

    public static void ForToEnum<T>(Action<T> pCallback)
    {
        var pEnumerator = Enum.GetValues(typeof(T)).GetEnumerator();
        while (pEnumerator.MoveNext())
        {
            pCallback((T)pEnumerator.Current);
        }
    }

    public static eSceneType GetSceneTypeByString(string strType)
    {
        switch(strType.ToLower())
        {
            case "intro":         return eSceneType.Intro;
            case "login":         return eSceneType.Login;
            case "lobby":         return eSceneType.Lobby;
        }                        
        return eSceneType.None;
    }

    public static eResourceType GetResourceTypeByExtension(string strExtension)
    {
        switch(strExtension.ToLower())
        {
            case ".prefab":     return eResourceType.Prefab;
            case ".anim":       return eResourceType.Animation;
            case ".mat":        return eResourceType.Material;
            case ".png":        return eResourceType.Texture;
            case ".jpg":        return eResourceType.Texture;
            case ".tga":        return eResourceType.Texture;
            case ".pdf":        return eResourceType.Texture;
            case ".mp3":        return eResourceType.Sound;
            case ".wav":        return eResourceType.Sound;
            case ".ogg":        return eResourceType.Sound;
            case ".bytes":      return eResourceType.Text;
            case ".xml":        return eResourceType.Text;
            case ".json":       return eResourceType.Text;
            case ".txt":        return eResourceType.Text;
        }
        return eResourceType.None;
    }

    public static string GetPlatformStringByEnum(RuntimePlatform eType)
    {
        switch(eType)
        {
            case RuntimePlatform.Android:      return "AOS";
            case RuntimePlatform.IPhonePlayer: return "IOS";
            default:                           return "PC";
        }
    }
    
#if UNITY_EDITOR
    public static string GetPlatformStringByEnum(BuildTarget eType)
    {
        switch (eType)
        {
            case BuildTarget.Android: return "AOS";
            case BuildTarget.iOS:     return "IOS";
            default:                  return "PC";
        }
    }
#endif
}
