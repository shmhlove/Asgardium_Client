﻿using UnityEngine;

using System;
using System.Collections;

using LitJson;
using System.Xml;

public abstract class SHBaseTable
{
    public string       m_strFileName;
    public string       m_strByteFileName;
    public eDataType    m_eDataType = eDataType.Table;


    #region Virtual Functions
    public virtual void Initialize() { }

    public abstract bool IsLoadTable();

    // 다양화(로드) : 코드로 데이터를 생성(Hard한 데이터)
    public virtual eErrorCode LoadStaticTable()                { return eErrorCode.Table_Not_Override; }

    // 다양화(로드) : Json파일에서 로드
    public virtual eErrorCode LoadJsonTable(JsonData pJson)    { return eErrorCode.Table_Not_Override; }

    // 다양화(로드) : XML파일에서 로드
    public virtual eErrorCode LoadXMLTable(XmlNode pNode)      { return eErrorCode.Table_Not_Override; }

    // 다양화(로드) : Byte파일에서 로드
    public virtual eErrorCode LoadBytesTable(byte[] pByte)     { return eErrorCode.Table_Not_Override; }

    // 다양화(로드) : 컨테이너를 시리얼 라이즈해서 Byte파일로 내어주는 함수
    public virtual byte[] GetBytesTable()                      { return null; }
    #endregion

    #region Interface Load Functions
    // 인터페이스 : Static 한 데이터 로드
    public virtual void LoadStatic(Action<eErrorCode> pCallback)
    {
        if (eErrorCode.Table_Not_Override == LoadStaticTable())
        {
            pCallback(eErrorCode.Table_Not_Override);
        }
        
        Initialize();

        pCallback(LoadStaticTable());
    }

    // 인터페이스 : Json파일 로드
    public virtual void LoadJson(string strFileName, Action<eErrorCode> pCallback)
    {
        if (eErrorCode.Table_Not_Override == LoadJsonTable(null))
        {
            pCallback(eErrorCode.Table_Not_Override);
            return;
        }
        
        new SHJson(strFileName, (pJson) => 
        {
            if (false == pJson.CheckJson())
            {
                pCallback(eErrorCode.Table_Not_ExsitFile);
                return;
            }

            Initialize();

            pCallback(LoadJsonTable(pJson.m_pJsonNode));
        });
    }

    // 인터페이스 : XML파일 로드
    public virtual void LoadXML(string strFileName, Action<eErrorCode> pCallback) 
    {
        if (eErrorCode.Table_Not_Override == LoadXMLTable(null))
        {
            pCallback(eErrorCode.Table_Not_Override);
            return;
        }
        
        new SHXML(strFileName, (pXML) => 
        {
            if (false == pXML.CheckXML())
            {
                pCallback(eErrorCode.Table_Not_ExsitFile);
                return;
            }
            
            XmlNodeList pNodeList = pXML.GetNodeList(m_strFileName);
            if (null == pNodeList)
            {
                pCallback(eErrorCode.Table_Error_Grammar);
                return;
            }

            Initialize();

            int iMaxNodeCount = pNodeList.Count;
            for (int iLoop = 0; iLoop < iMaxNodeCount; ++iLoop)
            {
                var eResult = LoadXMLTable(pNodeList[iLoop]);

                if (eErrorCode.Succeed != eResult)
                {
                    pCallback(eResult);
                    return;
                }
            }

            pCallback(eErrorCode.Succeed);
        });
    }
    
    // 인터페이스 : Byte파일 로드
    public virtual void LoadByte(string strFileName, Action<eErrorCode> pCallback)
    {
        if (eErrorCode.Table_Not_Override == LoadBytesTable(null))
        {
            pCallback(eErrorCode.Table_Not_Override);
            return;
        }
        
        new SHBytes(strFileName, (pBytes) => 
        {
            if (false == pBytes.CheckBytes())
            {
                pCallback(eErrorCode.Table_Not_ExsitFile);
                return;
            }

            Initialize();

            pCallback(LoadBytesTable(pBytes.m_pBytes));
        });
    }

    // 인터페이스 : 테이블 타입
    public eTableLoadType GetTableType()
    {
        if (eErrorCode.Table_Not_Override != LoadStaticTable())    return eTableLoadType.Static;
        if (eErrorCode.Table_Not_Override != LoadBytesTable(null)) return eTableLoadType.Byte;
        if (eErrorCode.Table_Not_Override != LoadXMLTable(null))   return eTableLoadType.XML;
        if (eErrorCode.Table_Not_Override != LoadJsonTable(null))  return eTableLoadType.Json;

        return eTableLoadType.None;
    }
    #endregion

    #region Json Pasing Utility
    // 유틸 : Json에서 String데이터 얻기
    public string GetStrToJson(JsonData pNode, string strKey)
    {
        if (null == pNode)
            return string.Empty;

        if (false == pNode.Keys.Contains(strKey))
            return string.Empty;

        if (JsonType.String != pNode[strKey].GetJsonType())
            return string.Empty;

        return (string)pNode[strKey];
    }

    // 유틸 : Json에서 int데이터 얻기
    public int GetIntToJson(JsonData pNode, string strKey)
    {
        if (null == pNode)
            return 0;

        if (false == pNode.Keys.Contains(strKey))
            return 0;

        if (JsonType.Int != pNode[strKey].GetJsonType())
            return 0;

        return (int)pNode[strKey];
    }

    // 유틸 : Json에서 long데이터 얻기
    public long GetLongToJson(JsonData pNode, string strKey)
    {
        if (null == pNode)
            return 0;

        if (false == pNode.Keys.Contains(strKey))
            return 0;

        if (JsonType.Long != pNode[strKey].GetJsonType())
            return 0;

        return (long)pNode[strKey];
    }

    // 유틸 : Json에서 float데이터 얻기
    public float GetFloatToJson(JsonData pNode, string strKey)
    {
        if (null == pNode)
            return 0.0f;

        if (false == pNode.Keys.Contains(strKey))
            return 0.0f;

        if (JsonType.Double != pNode[strKey].GetJsonType())
            return 0.0f;

        return (float)pNode[strKey];
    }

    // 유틸 : Json에서 bool데이터 얻기
    public bool GetBoolToJson(JsonData pNode, string strKey)
    {
        if (null == pNode)
            return false;

        if (false == pNode.Keys.Contains(strKey))
            return false;

        if (JsonType.Boolean != pNode[strKey].GetJsonType())
            return false;

        return (bool)pNode[strKey];
    }

    // 유틸 : Json에서 vector3데이터 얻기
    public Vector3 GetVector3ToJson(JsonData pArray)
    {
        if ((null == pArray) || (3 > pArray.Count))
            return Vector3.zero;
        
        return new Vector3((float)pArray[0],
                           (float)pArray[1],
                           (float)pArray[2]);
    }

    // 유틸 : Json에서 vector2데이터 얻기
    public Vector3 GetVector2ToJson(JsonData pArray)
    {
        if ((null == pArray) || (2 > pArray.Count))
            return Vector2.zero;

        return new Vector2((float)pArray[0],
                           (float)pArray[1]);
    }
    #endregion


    #region XML Pasing Utility
    // 유틸 : XML에서 Value데이터 얻기(형변환이 안된 데이터)
    public string GetValue(XmlNode pNode, string strKey)
    {
        if (null == pNode)
            return string.Empty;

        return pNode.Attributes.GetNamedItem(strKey).Value;
    }

    // 유틸 : XML에서 Int데이터 얻기
    public int GetInt(XmlNode pNode, string strKey)
    {
        int iValue = 0;
        int.TryParse(GetValue(pNode, strKey), out iValue);
        return iValue;
    }

    // 유틸 : XML에서 uInt데이터 얻기
    public uint GetuInt(XmlNode pNode, string strKey)
    {
        uint uiValue = 0;
        uint.TryParse(GetValue(pNode, strKey), out uiValue);
        return uiValue;
    }

    // 유틸 : XML에서 Long데이터 얻기
    public long GetLong(XmlNode pNode, string strKey)
    {
        long lValue = 0;
        long.TryParse(GetValue(pNode, strKey), out lValue);
        return lValue;
    }

    // 유틸 : XML에서 uLong데이터 얻기
    public ulong GetuLong(XmlNode pNode, string strKey)
    {
        ulong ulValue = 0;
        ulong.TryParse(GetValue(pNode, strKey), out ulValue);
        return ulValue;
    }

    // 유틸 : XML에서 Float데이터 얻기
    public float GetFloat(XmlNode pNode, string strKey)
    {
        float fValue = 0.0f;
        float.TryParse(GetValue(pNode, strKey), out fValue);
        return fValue;
    }

    // 유틸 : XML에서 Bool데이터 얻기
    public bool GetBool(XmlNode pNode, string strKey)
    {
        return 0 != GetInt(pNode, strKey);
    }

    // 유틸 : XML에서 String데이터 얻기
    public string GetStr(XmlNode pNode, string strKey)
    {
        return GetValue(pNode, strKey);
    }

    // 유틸 : XML에서 Time데이터 얻기
    public DateTime GetTime(XmlNode pNode, string strKey, string strFormat)
    {
        string strDate = GetStr(pNode, strKey);
        if (true == string.IsNullOrEmpty(strDate))
            return DateTime.Now;

        return SHUtils.GetDateTimeToString(strDate, strFormat);
    }
    #endregion
}
