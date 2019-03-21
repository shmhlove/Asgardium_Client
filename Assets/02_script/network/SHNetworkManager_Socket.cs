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
        //var test = Socket.Connect(m_strWebHost + "/sockettest");
        //var test = Socket.Connect(m_strWebHost);
        var test = Socket.Connect("http://13.124.43.70:3001");
        
        // 소켓 연결되었을 때
        test.On(SystemEvents.connect, () =>
        {
            Debug.Log("[LSH] Socket Event : SystemEvents.connect");
            test.Emit("message", "i connection now");
        });
        // // 소켓 연결이 타임아웃 되었을 때
        // test.On(SystemEvents.connectTimeOut, () =>
        // {
        //     Debug.Log("[LSH] Socket Event : SystemEvents.connectTimeOut");
        // });
        // test.On(SystemEvents.reconnectAttempt, () =>
        // {
        //     Debug.Log("[LSH] Socket Event : SystemEvents.reconnectAttempt");
        // });
        // test.On(SystemEvents.reconnectFailed, () =>
        // {
        //     Debug.Log("[LSH] Socket Event : SystemEvents.reconnectFailed");
        // });
        // // 소켓 연결 끊켰을 때
        // test.On(SystemEvents.disconnect, () =>
        // {
        //     Debug.Log("[LSH] Socket Event : SystemEvents.disconnect");
        // });
        // // 다시 소켓 연결이 되었을 때
        // test.On(SystemEvents.reconnect, () =>
        // {
        //     Debug.Log("[LSH] Socket Event : SystemEvents.reconnect");
        // });
        // test.On(SystemEvents.reconnecting, () =>
        // {
        //     Debug.Log("[LSH] Socket Event : SystemEvents.reconnecting");
        // });
        // test.On(SystemEvents.connectError, () =>
        // {
        //     Debug.Log("[LSH] Socket Event : SystemEvents.connectError");
        // });
        // test.On(SystemEvents.reconnectError, () =>
        // {
        //     Debug.Log("[LSH] Socket Event : SystemEvents.reconnectError");
        // });
        
        test.On("message", (string data) =>
        {
            Debug.Log("[RECIVE] " + data);
        });
    }
}