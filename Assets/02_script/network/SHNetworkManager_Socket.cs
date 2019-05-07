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
    public Socket m_pSocket = null;

    public void ConnectWebSocket(Action<SHReply> callback)
    {
        if (true == IsConnectSocket(m_pSocket))
        {
            callback(new SHReply(new SHError(eErrorCode.Net_Socket_Aready_Connect, "already connect", null)));
            return;
        }

        // 재시도 시도, 재시도, 재시도중, 재시도실패, 재시도에러 핸들러에 대한 처리를 하는것보다는
        // 재시도에 대한 코드는 직접 짜는게 좋겠다.
        SocketManager.Instance.Reconnection = false;

        // 타임아웃은 3초로, 1초로 하니깐 컨넥션타임아웃발생함.
        //SocketManager.Instance.TimeOut = 30000;

        // 연결, 연결타임아웃, 연결에러, 연결종료 이벤트만 처리
        m_pSocket = Socket.Connect(m_strWebHost, new SHCustomCertificateHandler());
        callback(new SHReply());
        
        m_pSocket.On(SystemEvents.connect, () =>
        {
            Debug.Log("[RECIVE] connect");

            JsonData jsonData = new JsonData();
            jsonData["message"] = "Connect Websocket!!";
            Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));
        });

        m_pSocket.On(SystemEvents.connectTimeOut, () =>
        {
            Debug.Log("[RECIVE] connectTimeOut");

            JsonData jsonData = new JsonData();
            jsonData["message"] = "connectTimeOut Websocket!!";
            Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));

            m_pSocket = ClearSocket(m_pSocket);
        });

        m_pSocket.On(SystemEvents.connectError, (Exception exception) =>
        {
            Debug.Log("[RECIVE] connectError");

            JsonData jsonData = new JsonData();
            jsonData["message"] = "connectError Websocket!!";
            Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));

            m_pSocket = ClearSocket(m_pSocket);
        });

        m_pSocket.On(SystemEvents.disconnect, () =>
        {
            Debug.Log("[RECIVE] disconnect");

            JsonData jsonData = new JsonData();
            jsonData["message"] = "disconnect Websocket!!";
            Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));

            m_pSocket = ClearSocket(m_pSocket);
        });

        m_pSocket.On("forceDisconnect", (string data) =>
        {
            Debug.Log("[RECIVE] forceDisconnect");

            JsonData jsonData = new JsonData();
            jsonData["message"] = "forceDisconnect Websocket!!";
            Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));

            m_pSocket = ClearSocket(m_pSocket);
        });

        m_pSocket.On("test_message", (string data) =>
        {
            Debug.Log("[RECIVE] " + data);

            JsonData jsonData = new JsonData();
            jsonData["message"] = data;
            Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));
        });
    }

    public void DisconnectWebSocket(Action<SHReply> callback)
    {
        if (false == IsConnectSocket(m_pSocket))
        {
            callback(new SHReply(new SHError(eErrorCode.Net_Socket_Not_Connect, "need connect", null)));
            return;
        }

        m_pSocket.Emit("forceDisconnect", "");
        callback(new SHReply());
    }

    public void TestSendMessage(string strMessage, Action<SHReply> callback)
    {
        if (false == IsConnectSocket(m_pSocket))
        {
            callback(new SHReply(new SHError(eErrorCode.Net_Socket_Not_Connect, "need connect", null)));
            return;
        }

        m_pSocket.Emit("test_message", strMessage);
        callback(new SHReply());
    }

    private bool IsConnectSocket(Socket pSocket)
    {
        return ((null != pSocket) && (true == pSocket.IsConnected));
    }

    private Socket ClearSocket(Socket pSocket)
    {
        if (null == pSocket)
        {
            return null;
        }
            
        GameObject.DestroyImmediate(pSocket.gameObject);
        return null;
    }
}