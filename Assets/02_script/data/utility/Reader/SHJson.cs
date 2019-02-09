using UnityEngine;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHJson
{
    public JsonData m_pJsonNode = null;

    public SHJson(string strFileName, Action<SHJson> pCallback)
    {
        if (true == string.IsNullOrEmpty(strFileName))
        {
            pCallback(this);
            return;
        }

        strFileName = Path.GetFileNameWithoutExtension(strFileName);

        // 1차 : PersistentDataPath에 데이터가 있으면 그걸 로드하도록 한다.
        // 2차 : 없으면 패키지에서 로드하도록 한다.

        string strSavePath = string.Format("{0}/{1}.json", SHPath.GetPersistentDataJson(), strFileName);
        if (true == File.Exists(strSavePath))
        {
            LoadByPersistent(strSavePath, (pJson) => 
            {
                m_pJsonNode = pJson;
                pCallback(this);
            });
        }
        else
        {
            LoadByPackage(strFileName, (pJson) => 
            {
                m_pJsonNode = pJson;
                pCallback(this);
            });
        }
    }
    
    public bool CheckJson()
    {
        return (null != m_pJsonNode);
    }

    private void LoadByWWW(string strFileName, Action<JsonData> pCallback)
    {
        var strFilePath = GetStreamingPath(strFileName);
        Single.Coroutine.WWW(new UnityWebRequest(strFilePath), (pWWW) => 
        {
            if (true != string.IsNullOrEmpty(pWWW.error))
            {
                Debug.LogWarningFormat("[LSH] Json(*.json)파일을 읽는 중 오류발생!!(Path:{0}, Error:{1})", strFilePath, pWWW.error);
            }
            
            pCallback(GetJsonParseToString(pWWW.downloadHandler.text));
        });
    }

    private void LoadByPersistent(string strFilePath, Action<JsonData> pCallback)
    {
        string strBuff = File.ReadAllText(strFilePath);
        if (true == string.IsNullOrEmpty(strBuff))
        {
            Debug.LogWarningFormat("[LSH] Json(*.json)파일을 읽는 중 오류발생!!(Path:{0})", strFilePath);
        }

        pCallback(GetJsonParseToString(strBuff));
    }

    private async void LoadByPackage(string strFileName, Action<JsonData> pCallback)
    {
        var pTextAsset = await Single.Resources.GetTextAsset(Path.GetFileNameWithoutExtension(strFileName));
        if (null == pTextAsset)
        {
            pCallback(null);
        }
        else
        {
            pCallback(GetJsonParseToByte(pTextAsset.bytes));
        }
    }
    
    private JsonData GetJsonParseToByte(byte[] pByte)
    {
        System.Text.UTF8Encoding pEncoder = new System.Text.UTF8Encoding();
        return JsonMapper.ToObject(pEncoder.GetString(pByte));
    }

    private JsonData GetJsonParseToString(string strBuff)
    {
        if (true == string.IsNullOrEmpty(strBuff))
        {
            return null;
        }

        MemoryStream pStream = new MemoryStream(Encoding.UTF8.GetBytes(strBuff));
        StreamReader pReader = new StreamReader(pStream, true);
        string strEncodingBuff = pReader.ReadToEnd();
        pReader.Close();
        pStream.Close();

        return JsonMapper.ToJson(strEncodingBuff);
    }

    private static string GetStreamingPath(string strFileName)
    {
        string strPath = string.Empty;

#if UNITY_EDITOR || UNITY_STANDALONE
        strPath = string.Format("{0}{1}", "file://", SHPath.GetStreamingAssets());
#elif UNITY_ANDROID
        strPath = string.Format("{0}{1}{2}", "jar:file://", SHPath.GetAssets(), "!/assets");
#elif UNITY_IOS
        strPath = string.Format("{0}{1}{2}", "file://", SHPath.GetAssets(), "/Raw");
#endif

        return string.Format("{0}/Json/{1}.json", strPath, Path.GetFileNameWithoutExtension(strFileName));
    }
}