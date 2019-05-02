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
        if (m_pSocket)
        {
            callback(new SHReply(new SHError(eErrorCode.Net_Socket_Aready_Connect, "already connect", null)));
            return;
        }

        m_pSocket = Socket.Connect(m_strWebHost, new SHCustomCertificateHandler());
        
        m_pSocket.On(SystemEvents.connect, () =>
        {
            JsonData jsonData = new JsonData();
            jsonData["message"] = "Succeed Websocket Connect!!";
            callback(new SHReply(jsonData));
        });
        
        m_pSocket.On("test_message", (string data) =>
        {
            Debug.Log("[RECIVE] " + data);

            JsonData jsonData = new JsonData();
            jsonData["message"] = data;
            Single.BusinessGlobal.ShowAlertUI(new SHReply(jsonData));
        });
    }
    
    public void TestSendMessage(string strMessage, Action<SHReply> callback)
    {
        if (null == m_pSocket)
        {
            callback(new SHReply(new SHError(eErrorCode.Net_Socket_Not_Connect, "need connect", null)));
            return;
        }

        m_pSocket.Emit("test_message", strMessage);
    }
}