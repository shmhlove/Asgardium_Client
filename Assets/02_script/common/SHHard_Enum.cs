using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;

public static partial class SHHard
{
    public static eSceneType GetSceneTypeByString(string strType)
    {
        switch(strType.ToLower())
        {
            case "intro":         return eSceneType.Intro;
            case "login":         return eSceneType.Login;
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
