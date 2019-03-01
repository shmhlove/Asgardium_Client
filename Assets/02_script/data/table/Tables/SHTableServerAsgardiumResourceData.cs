using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHTableServerAsgardiumResourceData
{
    public int m_iResourceId;
    public int m_iNameStrid;
    public int m_iIconId;
    public int m_iValue;
    public int m_iRIDFuel_1;
    public int m_iRIDFuel_2;
}

public class SHTableServerAsgardiumResource : SHBaseTable
{
    public Dictionary<int, SHTableServerAsgardiumResourceData> m_dicDatas = new Dictionary<int, SHTableServerAsgardiumResourceData>();
	
    public SHTableServerAsgardiumResource()
    {
        m_strIdentity = "ServerAsgardiumResourceData";
    }
    
    public override eErrorCode LoadServerTable(Action<eErrorCode> pCallback)
    {
        if (null == pCallback)
            return eErrorCode.Table_LoadFailed;

        Single.Network.GET(SHAPIs.SH_API_GET_ASGARDIUM_RESOURCE, null, (reply) => 
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
            var pData = new SHTableServerAsgardiumResourceData();
            pData.m_iResourceId = GetIntToJson(pJson[iLoop], "resource_id");
            pData.m_iNameStrid = GetIntToJson(pJson[iLoop], "name_strid");
            pData.m_iIconId = GetIntToJson(pJson[iLoop], "icon_id");
            pData.m_iValue = GetIntToJson(pJson[iLoop], "value");
            pData.m_iRIDFuel_1 = GetIntToJson(pJson[iLoop], "rid_fuel1");
            pData.m_iRIDFuel_2 = GetIntToJson(pJson[iLoop], "rid_fuel2");
            m_dicDatas.Add(pData.m_iResourceId, pData);
        }

        return eErrorCode.Succeed;
    }
}
