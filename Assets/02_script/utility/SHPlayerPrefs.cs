/// <summary>
/// 
/// - 유니티 레퍼런스
///     => 설명
///         게임 세션의 저장과 액세스 사이에 플레이어 환경설정입니다.
///         Mac OS X에서 PlayerPrefs는 unity.[company name].[product name].plist 이름으로된 파일에, 
///         ~/Library/Preferences 폴더에 저장되는, 회사와 제품 이름은 프로젝트에서 설정한 이름입니다. 
///         같은 .plist파일이 프로젝트가 에디터에서 실행하고 독립 플레이어 사이에서 사용됩니다. 
///         윈도우에서는, PlayerPrefs는 HKEY_CURRENT_USER\Software\[company name]\[product name] 키를 아래의 레지스트리에 저장됩니다.
///         회사와 제품 이름은 프로젝트에 설정한 이름입니다. 
///         
///         웹에서 플레이어, PlayerPrefs가 
///         Mac OS X에서 ~/Library/Preferences/Unity/WebPlayerPrefs 바이너리 파일 아래에 저장되고,
///         윈도우에서는 %APPDATA%\Unity\WebPlayerPrefs에서 저장됩니다. 파일 크기가 1메가바이트로 제한되고 웹 플레이어 URL 파일 당 하나의 환경설정 파일입니다. 
///         만약 이 제한이 초과된다면, SetInt, SetFloat과 SetString이 값을 저장되지 않으며 PlayerPrefsException을 던집니다.
///     
///     => 클래스 함수
///         SetInt       key에의해 식별되는 환경의 값을 설정합니다. 
///         GetInt       만약 그것이 존재한다면 환경설정 파일에서 key에 해당하는 값을 반환합니다.
///         SetFloat     key에의해 식별되는 환경의 값을 설정합니다.
///         GetFloat     만약 그것이 존재한다면 환경설정 파일에서 key에 해당하는 값을 반환합니다.
///         SetString    key에의해 식별되는 환경의 값을 설정합니다.
///         GetString    만약 그것이 존재한다면 환경설정 파일에서 key에 해당하는 값을 반환합니다.
///         HasKey       만약 환경설정에서 key가 존재한다면 true를 반환합니다.
///         DeleteKey    환경설정으로부터 그것의 값에 해당하고 key를 제거합니다.
///         DeleteAll    환경설정으로부터 모든 키와 값을 제거합니다. 조심해서 사용하세요.
/// 
/// </summary>

using UnityEngine;

using System;
using System.Text;
using System.Collections;
using System.Security.Cryptography;

public class SHPlayerPrefs
{
    #region Members
    private static string  m_strPrivateKey = "df897g995l234slkjskljf";
    public static string[] m_strkeys       = new string[] { "f64fsgrtwer",
                                                            "sdfsdf5dg4w",
                                                            "kk67fgdgewa",
                                                            "fgdfgwjkhh8",
                                                            "drwerwerw34" };
    #endregion


    #region Interface Int
    public static void SetInt(string strKey, int iValue)
    {
        PlayerPrefs.SetInt(strKey, iValue);
        SaveEncryption(strKey, "int", iValue.ToString());
    }
    
    public static int GetInt(string strKey)
    {
        return GetInt(strKey, 0);
    }
    
    public static int GetInt(string strKey, int iDefaultValue)
    {
        int iValue = PlayerPrefs.GetInt(strKey);
        if (false == CheckEncryption(strKey, "int", iValue.ToString()))
            return iDefaultValue;

        return iValue;
    }
    #endregion


    #region Interface Float
    public static void SetFloat(string strKey, float fValue)
    {
        PlayerPrefs.SetFloat(strKey, fValue);
        SaveEncryption(strKey, "float", Mathf.Floor(fValue * 1000.0f).ToString());
    }
    
    public static float GetFloat(string strKey)
    {
        return GetFloat(strKey, 0f);
    }
    
    public static float GetFloat(string strKey, float iDefaultValue)
    {
        float fValue = PlayerPrefs.GetFloat(strKey);
        if (false == CheckEncryption(strKey, "float", Mathf.Floor(fValue * 1000.0f).ToString()))
            return iDefaultValue;

        return fValue;
    }
    #endregion


    #region Interface String
    public static void SetString(string strKey, string strValue)
    {
        PlayerPrefs.SetString(strKey, strValue);
        SaveEncryption(strKey, "string", strValue);
    }
    
    public static string GetString(string strKey)
    {
        return GetString(strKey, "");
    }
    
    public static string GetString(string strKey, string iDefaultValue)
    {
        string value = PlayerPrefs.GetString(strKey);
        if (false == CheckEncryption(strKey, "string", value))
            return iDefaultValue;

        return value;
    }
    #endregion


    #region Utility Functions
    public static string Md5(string strEncrypt)
    {
        var pEncoder = new UTF8Encoding();
        var pHashBytes = SHHash.GetMD5ToBuff(pEncoder.GetBytes(strEncrypt));

        string strHash = string.Empty;
        for (int iLoop = 0; iLoop < pHashBytes.Length; ++iLoop)
        {
            strHash += Convert.ToString(pHashBytes[iLoop], 16).PadLeft(2, '0');
        }
        return strHash.PadLeft(32, '0');
    }

    public static void SaveEncryption(string strKey, string strType, string strValue)
    {
        var iKeyIndex    = (int)UnityEngine.Mathf.Floor(UnityEngine.Random.value * m_strkeys.Length);
        var strSecretKey = m_strkeys[iKeyIndex];
        var strCheck     = Md5(string.Format("{0}_{1}_{2}_{3}", strType, m_strPrivateKey, strSecretKey, strValue));
        PlayerPrefs.SetString(string.Format("{0}_Encryption_Check", strKey), strCheck);
        PlayerPrefs.SetInt(string.Format("{0}_Used_Key", strKey), iKeyIndex);
    }

    public static bool CheckEncryption(string strKey, string strType, string strValue)
    {
        int iKeyIndex       = PlayerPrefs.GetInt(strKey + "_Used_Key");
        var strSecretKey    = m_strkeys[iKeyIndex];
        var strCheck        = Md5(string.Format("{0}_{1}_{2}_{3}", strType, m_strPrivateKey, strSecretKey, strValue));

        strKey = string.Format("{0}_Encryption_Check", strKey);
        if (false == PlayerPrefs.HasKey(strKey))
            return false;

        return (PlayerPrefs.GetString(strKey) == strCheck);
    }
    
    public static bool HasKey(string strKey)
    {
        return PlayerPrefs.HasKey(strKey);
    }

    public static void DeleteKey(string strKey)
    {
        PlayerPrefs.DeleteKey(strKey);
        PlayerPrefs.DeleteKey(string.Format("{0}_Encryption_Check", strKey));
        PlayerPrefs.DeleteKey(string.Format("{0}_Used_Key",         strKey));
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }
    #endregion
}