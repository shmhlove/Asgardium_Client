using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerGlobalUnitData
{
    public int m_iUnitId;
    public int m_iNameStrId;
    public string m_strIconImage;
    public int m_iWeight;
    public int m_iFuelUnitId_1;
    public int m_iFuelUnitId_2;
}

public class SHTableServerGlobalUnit : SHBaseTable
{
    public Dictionary<int, SHTableServerGlobalUnitData> m_dicDatas = new Dictionary<int, SHTableServerGlobalUnitData>();
	
    public SHTableServerGlobalUnit()
    {
        m_strIdentity = "ServerGlobalUnit";
    }
    
    public override eErrorCode LoadServerTable(Action<eErrorCode> pCallback)
    {
        if (null == pCallback)
            return eErrorCode.Table_LoadFailed;

        Single.Network.GET(SHAPIs.SH_API_GET_UNIT_TABLE, null, (reply) => 
        {
            if (reply.isSucceed)
            {
                pCallback(LoadJsonTable(reply.data));
            }
            else
            {
                pCallback(eErrorCode.Table_LoadFailed);
            }
        });

        return eErrorCode.Succeed;
    }
    
    public override eErrorCode LoadJsonTable(JsonData pJson)
    {
        if (null == pJson)
            return eErrorCode.Table_LoadFailed;

        m_dicDatas.Clear();

        for (int iLoop = 0; iLoop < pJson.Count; ++iLoop)
        {
            var pData = new SHTableServerGlobalUnitData();
            pData.m_iUnitId = GetIntToJson(pJson[iLoop], "unit_id");
            pData.m_iNameStrId = GetIntToJson(pJson[iLoop], "name_str_id");
            pData.m_strIconImage = GetStrToJson(pJson[iLoop], "icon_image");
            pData.m_iWeight = GetIntToJson(pJson[iLoop], "weight");
            pData.m_iFuelUnitId_1 = GetIntToJson(pJson[iLoop], "fuel_unit_id_1");
            pData.m_iFuelUnitId_2 = GetIntToJson(pJson[iLoop], "fuel_unit_id_2");
            m_dicDatas.Add(pData.m_iUnitId, pData);
        }

        return eErrorCode.Succeed;
    }

    public SHTableServerGlobalUnitData GetData(int iUnitId)
    {
        if (false == m_dicDatas.ContainsKey(iUnitId))
            return new SHTableServerGlobalUnitData();

        return m_dicDatas[iUnitId];
    }
}
