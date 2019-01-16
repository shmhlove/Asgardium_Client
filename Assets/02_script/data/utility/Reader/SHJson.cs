using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHJson
{
    private JsonData m_pJsonNode = null;
    public JsonData Node { get { return m_pJsonNode; } }


    public SHJson() { }
    public SHJson(string strFileName)
    {
        if (true == string.IsNullOrEmpty(strFileName))
            return;

        strFileName = Path.GetFileNameWithoutExtension(strFileName);

        // 1차 : PersistentDataPath에 Json데이터가 있으면 그걸 로드하도록 한다.
        // 2차 : 없으면 패키지에서 로드하도록 한다.

        if (null != (m_pJsonNode = LoadToPersistent(strFileName)))
            return;

        m_pJsonNode = LoadToStreamingForWWW(strFileName);
    }

    ~SHJson()
    {
        SetJsonNode(null);
    }

    // 인터페이스 : JsonNode설정
    public JsonData SetJsonNode(JsonData pNode)
    {
        return (m_pJsonNode = pNode);
    }

    // 인터페이스 : Persistent에서 로드
    public JsonData LoadToPersistent(string strFileName)
    {
        string strSavePath = string.Format("{0}/{1}.json", SHPath.GetPathToPersistentJson(), Path.GetFileNameWithoutExtension(strFileName));
        if (false == File.Exists(strSavePath))
            return null;

        return SetJsonNode(LoadLocal(strSavePath));
    }

    // 인터페이스 : Streaming에서 LoaclLoad로 로드
    public JsonData LoadToStreamingForLocal(string strFileName)
    {
        string strSavePath = string.Format("{0}/{1}.json", SHPath.GetPathToJson(), Path.GetFileNameWithoutExtension(strFileName));
        if (false == File.Exists(strSavePath))
            return null;

        return SetJsonNode(LoadLocal(strSavePath));
    }

    // 인터페이스 : Streaming에서 WWW로 로드
    public JsonData LoadToStreamingForWWW(string strFileName)
    {
        return SetJsonNode(LoadWWW(GetStreamingPath(strFileName)));
    }

    // 인터페이스 : Json파일 로드
    public JsonData LoadWWW(string strFilePath)
    {
        WWW pWWW = Single.Coroutine.WWWOfSync(new WWW(strFilePath));
        if (true != string.IsNullOrEmpty(pWWW.error))
        {
            Debug.LogWarningFormat("Json(*.json)파일을 읽는 중 오류발생!!(Path:{0}, Error:{1})", strFilePath, pWWW.error);
            return null;
        }

        return GetJsonParseToString(pWWW.text);
    }
    
    // 인터페이스 : Byte로 Json파싱
    public JsonData GetJsonParseToByte(byte[] pByte)
    {
        System.Text.UTF8Encoding pEncoder = new System.Text.UTF8Encoding();
        return JsonMapper.ToJson(pEncoder.GetString(pByte));
    }

    // 인터페이스 : string으로 Json파싱
    public JsonData GetJsonParseToString(string strBuff)
    {
        MemoryStream pStream = new MemoryStream(Encoding.UTF8.GetBytes(strBuff));
        StreamReader pReader = new StreamReader(pStream, true);
        string strEncodingBuff = pReader.ReadToEnd();
        pReader.Close();
        pStream.Close();

        return JsonMapper.ToJson(strEncodingBuff);
    }

    // 인터페이스 : Json파일 로드 체크
    public bool CheckJson()
    {
        return (null != m_pJsonNode);
    }

    // 유틸 : Json파일 로드
    public JsonData LoadLocal(string strFilePath)
    {
        if (false == File.Exists(strFilePath))
            return null;

        string strBuff = File.ReadAllText(strFilePath);
        if (true == string.IsNullOrEmpty(strBuff))
        {
            Debug.LogWarningFormat("Json(*.json)파일을 읽는 중 오류발생!!(Path:{0})", strFilePath);
            return null;
        }

        return GetJsonParseToString(strBuff);
    }

    // 유틸 : StreamingPath경로 만들기
    public static string GetStreamingPath(string strFileName)
    {
        string strPath = string.Empty;

#if UNITY_EDITOR || UNITY_STANDALONE
        strPath = string.Format("{0}{1}", "file://", SHPath.GetPathToStreamingAssets());
#elif UNITY_ANDROID
        strPath = string.Format("{0}{1}{2}", "jar:file://", SHPath.GetPathToAssets(), "!/assets");
#elif UNITY_IOS
        strPath = string.Format("{0}{1}{2}", "file://", SHPath.GetPathToAssets(), "/Raw");
#endif

        return string.Format("{0}/JSons/{1}.json", strPath, Path.GetFileNameWithoutExtension(strFileName));
    }
}