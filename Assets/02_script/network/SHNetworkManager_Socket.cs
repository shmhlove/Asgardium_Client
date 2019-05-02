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
    public void ConnectWebSocket(Action<SHReply> callback)
    {
        var test = Socket.Connect(m_strWebHost, new SHCustomCertificateHandler());
        
        test.On(SystemEvents.connect, () =>
        {
            Debug.Log("[LSH] Socket Event : SystemEvents.connect");
            test.Emit("test_message", "i connection now");
            callback(new SHReply());
        });
        
        test.On("test_message", (string data) =>
        {
            Debug.Log("[RECIVE] " + data);
        });
    }
}