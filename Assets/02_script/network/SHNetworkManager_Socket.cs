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

public partial class SHNetworkManager : SHSingleton<SHNetworkManager>
{
    public void ConnectWebSocket(Action<SHReply> callback)
    {
        SetServerInfo(() => 
        {
            //var test = Socket.Connect(m_strWebHost + "/sockettest");
            var test = Socket.Connect(m_strWebHost, new SHCustomCertificateHandler());
            
            test.On(SystemEvents.connect, () =>
            {
                Debug.Log("[LSH] Socket Event : SystemEvents.connect");
                test.Emit("message", "i connection now");
            });
            
            test.On("message", (string data) =>
            {
                Debug.Log("[RECIVE] " + data);
            });
        });
    }
}