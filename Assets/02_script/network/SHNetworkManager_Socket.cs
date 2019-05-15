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

    public void SendRequestSocket(string strEvent, JsonData body, Action<SHReply> callback)
    {
        SendRequestSocket(new SHRequestData(strEvent, HTTPMethodType.POST, body, callback));
    }

    public void SendRequestSocket(SHRequestData pRequestData)
    {
        m_pSocketRequestQueue.Add(pRequestData);

        if (true == m_bIsProcessingRetry)
            return;

        ProcessSendRequestSocket();
    }

    public void ProcessSendRequestSocket()
    {
        if (false == IsWebSocketConnected())
        {
            StartRetryProcess();
        }
        else
        {
            m_pSocketRequestQueue.ForEach((pReq) => 
            {
                m_pSocket.Emit(pReq.m_strPath, pReq.m_pBody.ToJson());

                pReq.m_pCallback(new SHReply()
                {
                    isSucceed = true,
                    data = pReq.m_pBody,
                    requestMethod = "socket",
                    requestUrl = pReq.m_strPath
                });
            });
            m_pSocketRequestQueue.Clear();
        }
    }

    public Socket ConnectWebSocket()
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
        m_pSocket.On("force_disconnect", OnSocketEventForForceDisconnect);
        m_pSocket.On("test_message", OnSocketEventForTestMessage);

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
        
        var pSocket = ConnectWebSocket();
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

    private Socket ClearSocket(Socket pSocket)
    {
        if (null == pSocket) {
            return null;
        }
        
        GameObject.DestroyImmediate(pSocket.gameObject);
        return null;
    }

    private bool IsWebSocketConnected()
    {
        return (m_pSocket && m_pSocket.IsConnected);
    }

    // 소켓 시스템 이벤트 함수
    private void OnSocketEventForConnect()
    {
        Debug.Log("[RECIVE] connect");

        // JsonData jsonData = new JsonData();
        // jsonData["message"] = "Connect Websocket!!";
        // Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));
    }
    private void OnSocketEventForConnectTimeOut()
    {
        Debug.Log("[RECIVE] connectTimeOut");

        // JsonData jsonData = new JsonData();
        // jsonData["message"] = "connectTimeOut Websocket!!";
        // Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));

        m_pSocket = ClearSocket(m_pSocket);
        StartRetryProcess();
    }
    private void OnSocketEventForConnectError(Exception pException)
    {
        Debug.Log("[RECIVE] connectError");

        // JsonData jsonData = new JsonData();
        // jsonData["message"] = "connectError Websocket!!";
        // Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));

        m_pSocket = ClearSocket(m_pSocket);
        StartRetryProcess();
    }
    private void OnSocketEventForDisconnect()
    {
        Debug.Log("[RECIVE] disconnect");

        // JsonData jsonData = new JsonData();
        // jsonData["message"] = "disconnect Websocket!!";
        // Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));

        m_pSocket = ClearSocket(m_pSocket);
        StartRetryProcess();
    }

    // 소켓 커스텀 이벤트 함수
    private void OnSocketEventForForceDisconnect(string strMessage)
    {
        Debug.Log("[RECIVE] forceDisconnect : " + strMessage);

        // JsonData jsonData = new JsonData();
        // jsonData["message"] = "forceDisconnect Websocket!!";
        // Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));

        m_pSocket = ClearSocket(m_pSocket);
    }
    private void OnSocketEventForTestMessage(string strMessage)
    {
        Debug.Log("[RECIVE] testMessage : " + strMessage);

        // JsonData jsonData = new JsonData();
        // jsonData["message"] = data;
        // Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));
    }
}