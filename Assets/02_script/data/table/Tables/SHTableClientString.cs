using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableClientString : SHBaseTable
{
    public Dictionary<string, string> m_dicData = new Dictionary<string, string>();

    public SHTableClientString()
    {
        var strLanguage = SHPlayerPrefs.GetString("language");
        if (string.IsNullOrEmpty(strLanguage))
        {
            strLanguage = GetLanguageCode(Single.AppInfo.GetSystemLanguage());
            SHPlayerPrefs.SetString("language", strLanguage);
        }
        
        m_strIdentity = string.Format("ClientString_{0}", strLanguage);
    }

    public override eErrorCode LoadJsonTable(JsonData pJson)
    {
        if (null == pJson)
            return eErrorCode.Table_LoadFailed;
        
        m_dicData.Clear();

        for (int iLoop = 0; iLoop < pJson.Count; ++iLoop)
        {
            foreach (var element in pJson[iLoop].Keys)
            {
                m_dicData.Add(element, GetStrToJson(pJson[iLoop], element));
            }
        }

        return eErrorCode.Succeed;
    }

    public void ChangeLanguage(SystemLanguage eLanguage, Action<eErrorCode> pCallback)
    {
        SHPlayerPrefs.SetString("language", GetLanguageCode(eLanguage));
        m_strIdentity = string.Format("ClientString_{0}", GetLanguageCode(eLanguage));
        LoadJson(pCallback);
    }
    
    private string GetLanguageCode(SystemLanguage eLanguage)
    {
        switch(eLanguage)
        {
            case SystemLanguage.English: return "en";
            case SystemLanguage.Korean:  return "kr";
            default:                     return "en";
        }
    }
}