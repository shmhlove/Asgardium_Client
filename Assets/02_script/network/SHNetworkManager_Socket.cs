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
    private bool m_bIsRunWebsocketRetryCoroutine = false;
    private List<SHRequestData> m_pSocketRequestQueue = new List<SHRequestData>();
    private Dictionary<SystemEvents, List<Action>> m_pSystemEventObserver = new Dictionary<SystemEvents, List<Action>>();

    public void ConnectWebSocket()
    {
        StartCoroutine("CoroutineRetryWebSocketProcess");
    }

    public void AddSystemEventObserver(SystemEvents eEvent, Action pCallback)
    {
        if (false == m_pSystemEventObserver.ContainsKey(eEvent))
        {
            m_pSystemEventObserver.Add(eEvent, new List<Action>());
        }

        if (false == m_pSystemEventObserver[eEvent].Contains(pCallback))
        {
            m_pSystemEventObserver[eEvent].Add(pCallback);
        }
    }

    public void SendRequestSocket(string strEvent, JsonData body, Action<SHReply> callback)
    {
        SendRequestSocket(new SHRequestData(strEvent, HTTPMethodType.POST, body, callback));
    }

    public void SendRequestSocket(SHRequestData pRequestData)
    {
        m_pSocketRequestQueue.Add(pRequestData);

        if (true == m_bIsProcessingRetry)
            return;

        if (true == m_bIsRunWebsocketRetryCoroutine)
            return;

        ProcessSendRequestSocket();
    }

    public void ProcessSendRequestSocket()
    {
        if (false == IsWebSocketConnected())
        {
            StartRetryProcess();
            return;
        }

        foreach (var pReq in m_pSocketRequestQueue)
        {
            JsonData jsonBody = new JsonData
            {
                ["jwt_header"] = GetJWT(),
                ["body"] = pReq.m_pBody
            };

            // "는 전송이 안된다. "를 '으로 모두 바꾸고 보낸다.
            var strMessage = GetBodyMessage(jsonBody).Replace("\"", "\'");
            DebugLogOfSocketRequest(pReq.m_strPath, strMessage);
            m_pSocket.Emit(pReq.m_strPath, strMessage);

            var strEvent = pReq.m_strPath;
            var pCallback = pReq.m_pCallback;
            m_pSocket.On(strEvent, (string strResponse) =>
            {
                // "{\"key\": "\value\"}" 를 {"key": "value"}로 바꿔야한다.
                // \"를 \'으로 바꾸고, "를 모두제거한다. \'를 "으로 다시 바꾼다.
                SHReply pReply = new SHReply(strEvent, strResponse.Replace("\\\"", "\\\'").Replace("\"", "").Replace("\\\'", "\""));
                DebugLogOfSocketResponse(pReply);
                pCallback(pReply);

                // force_disconnect는 disconnect처리를 해준다.
                if (true == strEvent.Equals(SHAPIs.SH_SOCKET_REQ_FORCE_DISCONNECT))
                {
                    ClearSocket();
                }
            });
        }

        m_pSocketRequestQueue.Clear();
    }

    private Socket Connect()
    {
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
        m_pSocket.On(SHAPIs.SH_SOCKET_POLLING_MINING_ACTIVE_INFO, OnSocketEventForMiningActiveInfo);
        
        return m_pSocket;
    }

    private IEnumerator CoroutineRetryWebSocketProcess()
    {
        if (true == IsWebSocketConnected())
        {
            yield break;
        }

        if (true == m_bIsRunWebsocketRetryCoroutine)
        {
            yield break;
        }
        m_bIsRunWebsocketRetryCoroutine = true;
        
        var pSocket = Connect();
        pSocket.On(SystemEvents.connect, () => 
        {
            m_bIsRunWebsocketRetryCoroutine = false;
            OnSocketEventForConnect();

            StopRetryProcess();
            ProcessSendRequestSocket();
        });
        pSocket.On(SystemEvents.connectTimeOut, () => 
        {
            m_bIsRunWebsocketRetryCoroutine = false;
            OnSocketEventForConnectTimeOut();
        });
        pSocket.On(SystemEvents.connectError, (pException) => 
        {
            m_bIsRunWebsocketRetryCoroutine = false;
            OnSocketEventForConnectError(pException);
        });
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

    // 소켓 시스템 이벤트 함수
    private void OnSocketEventForConnect()
    {
        Debug.LogFormat("<color=#0033ff>[SOCKET_RESPONSE]</color> : {0}",
                "Connect");

        if (true == m_pSystemEventObserver.ContainsKey(SystemEvents.connect))
        {
            foreach (var pCallback in m_pSystemEventObserver[SystemEvents.connect])
            {
                pCallback();
            }
        }
    }
    private void OnSocketEventForConnectTimeOut()
    {
        Debug.LogFormat("<color=#0033ff>[SOCKET_RESPONSE]</color> : {0}",
                "ConnectTimeOut");

        ClearSocket();
        StartRetryProcess();

        if (true == m_pSystemEventObserver.ContainsKey(SystemEvents.connectTimeOut))
        {
            foreach (var pCallback in m_pSystemEventObserver[SystemEvents.connectTimeOut])
            {
                pCallback();
            }
        }
    }
    private void OnSocketEventForConnectError(Exception pException)
    {
        Debug.LogFormat("<color=#0033ff>[SOCKET_RESPONSE]</color> : {0}",
                "ConnectError");

        ClearSocket();
        StartRetryProcess();

        if (true == m_pSystemEventObserver.ContainsKey(SystemEvents.connectError))
        {
            foreach (var pCallback in m_pSystemEventObserver[SystemEvents.connectError])
            {
                pCallback();
            }
        }
    }
    private void OnSocketEventForDisconnect()
    {
        Debug.LogFormat("<color=#0033ff>[SOCKET_RESPONSE]</color> : {0}",
                "Disconnect");

        ClearSocket();
        StartRetryProcess();

        if (true == m_pSystemEventObserver.ContainsKey(SystemEvents.disconnect))
        {
            foreach (var pCallback in m_pSystemEventObserver[SystemEvents.disconnect])
            {
                pCallback();
            }
        }
    }

    private void OnSocketEventForMiningActiveInfo(string strResponse)
    {
        // "{\"key\": "\value\"}" 를 {"key": "value"}로 바꿔야한다.
        // \"를 \'으로 바꾸고, "를 모두제거한다. \'를 "으로 다시 바꾼다.
        SHReply pReply = new SHReply(SHAPIs.SH_SOCKET_POLLING_MINING_ACTIVE_INFO, strResponse.Replace("\\\"", "\\\'").Replace("\"", "").Replace("\\\'", "\""));
        DebugLogOfSocketResponse(pReply);
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