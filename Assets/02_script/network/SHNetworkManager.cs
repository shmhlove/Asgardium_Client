using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

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

public enum eRequestStatus
{
    Ready,
    Requesting,
    Done,
}

public class SHRequestData
{
    public string m_strPath;
    public JsonData m_pBody;
    public HTTPMethodType m_eMethodType;
    public Action<SHReply> m_pCallback;
    public eRequestStatus m_eRequestStatus;

    public SHRequestData(string path, HTTPMethodType methoodType, JsonData body, Action<SHReply> callback)
    {
        this.m_strPath = path;
        this.m_pBody = body;
        this.m_eMethodType = methoodType;
        this.m_pCallback = (null == callback) ? (reply) => { } : callback;
        this.m_eRequestStatus = eRequestStatus.Ready;
    }
}

public partial class SHNetworkManager : SHSingleton<SHNetworkManager>
{
    private string m_strWebHost = "";
    
    private int m_iRetryCount = 0;
    private int m_iMaxRetryCount = 5;
    private bool m_bIsProcessingRetry = false;

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