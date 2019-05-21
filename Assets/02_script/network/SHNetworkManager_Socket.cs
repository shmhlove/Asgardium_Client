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

public partial class SHNetworkManager : SHSingleton<SHNetworkManager>
{
    private Socket m_pSocket = null;
    private bool m_bIsRetryingWebSocketConnect = false;
    private List<SHRequestData> m_pSocketRequestQueue = new List<SHRequestData>();
    private Dictionary<string, List<Action<SHReply>>> m_pSocketEventObserver = new Dictionary<string, List<Action<SHReply>>>();
    
    public void SendRequestSocket(string strEvent, JsonData pBody, Action<SHReply> pCallback)
    {
        m_pSocketRequestQueue.Add(new SHRequestData(strEvent, HTTPMethodType.POST, pBody, pCallback));

        if (true == m_bIsProcessingRetry)
            return;

        if (true == m_bIsRetryingWebSocketConnect)
            return;

        ProcessSendRequestSocket();
    }

    private void ProcessSendRequestSocket()
    {
        if (false == IsWebSocketConnected())
        {
            StartRetryProcess();
            return;
        }

        foreach (var pReq in m_pSocketRequestQueue)
        {
            // 전송 중 연결종료가 발생하면 요청이 유실되므로 즉시 콜백을 주도록 하자.
            if (false == IsWebSocketConnected()) {
                SHReply pReply = new SHReply(new SHError(eErrorCode.Net_Socket_Disconnect, "Disconnected Socket"));
                pReply.requestMethod = "socket";
                pReply.requestUrl = pReq.m_strPath;
                pReq.m_pCallback(pReply);
                continue;
            }

            JsonData jsonBody = new JsonData
            {
                ["jwt_header"] = GetJWT(),
                ["body"] = pReq.m_pBody
            };

            // 와... 뭐이런게 다있노ㅋ -> "는 전송이 안된다. "를 '으로 모두 바꾸고 보낸다.
            var strMessage = GetBodyMessage(jsonBody).Replace("\"", "\'");
            DebugLogOfSocketRequest(pReq.m_strPath, strMessage);
            m_pSocket.Emit(pReq.m_strPath, strMessage);

            var strEvent = pReq.m_strPath;
            var pCallback = pReq.m_pCallback;
            
            m_pSocket.On(strEvent, (string strResponse) =>
            {
                SHReply pReply = new SHReply(strEvent, strResponse);
                DebugLogOfSocketResponse(pReply);
                pCallback(pReply);

                // force_disconnect는 예외로 여기서 바로 disconnect 처리를 해준다.
                if (true == strEvent.Equals(SHAPIs.SH_SOCKET_REQ_FORCE_DISCONNECT))
                {
                    ClearSocket();
                }
            });
        }

        m_pSocketRequestQueue.Clear();
    }

    public void ConnectWebSocket()
    {
        ProcessRetryWebSocketConnect();
    }

    private void ProcessRetryWebSocketConnect()
    {
        if (true == IsWebSocketConnected()) {
            return;
        }

        if (true == m_bIsRetryingWebSocketConnect) {
            return;
        }

        m_bIsRetryingWebSocketConnect = true;
        
        // 소켓 셋팅
        SocketManager.Instance.Reconnection = false;
        //SocketManager.Instance.TimeOut = 30000;

        // 연결
        m_pSocket = Socket.Connect(m_strWebHost, new SHCustomCertificateHandler());
        
        // 시스템 이벤트 함수 등록
        m_pSocket.On(SystemEvents.connect, OnSocketEventForConnect);
        m_pSocket.On(SystemEvents.connectTimeOut, OnSocketEventForConnectTimeOut);
        m_pSocket.On(SystemEvents.connectError, OnSocketEventForConnectError);
        m_pSocket.On(SystemEvents.disconnect, OnSocketEventForDisconnect);

        // 커스텀 이벤트 함수 등록
        m_pSocket.On(SHAPIs.SH_SOCKET_POLLING_MINING_ACTIVE_INFO, OnSocketPollingEventForMiningActiveInfo);
    }

    private void ClearSocket()
    {
        if (null == m_pSocket) {
            return;
        }
        
        GameObject.DestroyImmediate(m_pSocket.gameObject);
        m_pSocket = null;
    }

    private bool IsWebSocketConnected()
    {
        return (m_pSocket && m_pSocket.IsConnected);
    }

    public void AddEventObserver(string strEvent, Action<SHReply> pCallback)
    {
        if (false == m_pSocketEventObserver.ContainsKey(strEvent))
        {
            m_pSocketEventObserver.Add(strEvent, new List<Action<SHReply>>());
        }

        if (false == m_pSocketEventObserver[strEvent].Contains(pCallback))
        {
            m_pSocketEventObserver[strEvent].Add(pCallback);
        }
    }
    
    private void SendEventForObserver(string strEvent, SHReply pReply)
    {
        if (false == m_pSocketEventObserver.ContainsKey(strEvent))
            return;

        foreach (var pCallback in m_pSocketEventObserver[strEvent])
        {
            pCallback(pReply);
        }
    }

    // 소켓 시스템 이벤트 함수
    private void OnSocketEventForConnect()
    {
        m_bIsRetryingWebSocketConnect = false;

        StopRetryProcess();
        ProcessSendRequestSocket();

        var pReply = new SHReply(new JsonData
        {
            ["message"] = "socket connect",
            ["url"] = m_pSocket.Namespace
        });
        SendEventForObserver(SystemEvents.connect.ToString(), pReply);
        DebugLogOfSocketResponse(pReply);
    }
    private void OnSocketEventForConnectTimeOut()
    {
        m_bIsRetryingWebSocketConnect = false;

        ClearSocket();
        StartRetryProcess();

        var pReply = new SHReply(new SHError(eErrorCode.Net_Socket_Connect_Timeout, "Timeout Socket Connect"));
        SendEventForObserver(SystemEvents.connectTimeOut.ToString(), pReply);
        DebugLogOfSocketResponse(pReply);
    }
    private void OnSocketEventForConnectError(Exception pException)
    {
        m_bIsRetryingWebSocketConnect = false;

        ClearSocket();
        StartRetryProcess();

        var pReply = new SHReply(new SHError(eErrorCode.Net_Socket_Connect_Error, pException.ToString()));
        SendEventForObserver(SystemEvents.connectError.ToString(), pReply);
        DebugLogOfSocketResponse(pReply);
    }
    private void OnSocketEventForDisconnect()
    {
        ClearSocket();
        StartRetryProcess();
        
        var pReply = new SHReply(new SHError(eErrorCode.Net_Socket_Disconnect, "Disconnected Socket"));
        SendEventForObserver(SystemEvents.connectTimeOut.ToString(), pReply);
        DebugLogOfSocketResponse(pReply);
    }

    // 소켓 커스텀 이벤트 함수
    private void OnSocketPollingEventForMiningActiveInfo(string strResponse)
    {
        SHReply pReply = new SHReply(SHAPIs.SH_SOCKET_POLLING_MINING_ACTIVE_INFO, strResponse);
        SendEventForObserver(SHAPIs.SH_SOCKET_POLLING_MINING_ACTIVE_INFO, pReply);
        //DebugLogOfSocketResponse(pReply);
    }
    
    private void DebugLogOfSocketRequest(string strEvent, string strMessage)
    {
        Debug.LogFormat("<color=#666600>[SOCKET_REQUEST]</color> : {0}\n{1}",
            strEvent, strMessage);
    }

    private void DebugLogOfSocketResponse(SHReply pReply)
    {
        var strLog = string.Format("<color=#0033ff>[SOCKET_RESPONSE]</color> : {0}\n{1}",
                pReply.requestUrl,
                pReply.ToString());

        if (pReply.isSucceed)
        {
            Debug.Log(strLog);
        }
        else
        {
            Debug.LogError(strLog);
        }
    }
}