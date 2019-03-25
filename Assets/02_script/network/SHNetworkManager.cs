using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;

using LitJson;
using socket.io;

public enum HTTPMethodType
{
    GET,
    POST,
    DELETE,
    UPDATE,
    PUT,
}

public partial class SHNetworkManager : SHSingleton<SHNetworkManager>
{
    private string m_strWebHost = "";
    private int m_iMaxRetryCount = 5;
    private SHTableClientString m_pStringTable;

    public override void OnInitialize()
    {
        SetServerInfo(() => {});
        SetStringTable();
        SetDontDestroy();
    }
    
    private async void SetServerInfo(Action pCallback)
    {
        var pTable = await Single.Table.GetTable<SHTableClientConfig>();
        m_strWebHost     = pTable.ServerHost;
        m_iMaxRetryCount = pTable.MaxRetryRequestCount;
        pCallback();
    }
    
    private async void SetStringTable()
    {
        m_pStringTable = await Single.Table.GetTable<SHTableClientString>();
    }

    private string GetBodyMessage(JsonData body)
    {
        return (null == body) ? "{}" : JsonMapper.ToJson(body);
    }
}