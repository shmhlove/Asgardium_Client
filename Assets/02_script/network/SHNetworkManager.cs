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

public class SHNetworkManager : SHSingleton<SHNetworkManager>
{
    //public string WebHost = "https://13.124.43.70:3002";
    public string WebHost = "https://localhost:3002";
    //public string WebHost = "http://localhost:3001";

    public override void OnInitialize()
    {
        SetDontDestroy();
    }

    public void GET(string path, JsonData body, Action<SHReply> callback)
    {
        SendRequest(path, HTTPMethodType.GET, body, callback);
    }

    public void POST(string path, JsonData body, Action<SHReply> callback)
    {
        SendRequest(path, HTTPMethodType.POST, body, callback);
    }

    public void SendRequest(string path, HTTPMethodType methoodType, JsonData body, Action<SHReply> callback)
    {
        StartCoroutine(CoroutineSendRequest(path, methoodType, body, callback));
    }

    public void ConnectWebSocket(Action<SHReply> callback)
    {
        //var test = Socket.Connect(WebHost + "/sockettest");
        var test = Socket.Connect(WebHost);

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

    private IEnumerator CoroutineSendRequest(string path, HTTPMethodType methodType, JsonData body, Action<SHReply> callback)
    {
        ShowIndicator();

        UnityWebRequest www = CreateRequestData(path, methodType, body);
        yield return www.SendWebRequest();

        CloseIndicator();

        callback(new SHReply(www));
    }

    private UnityWebRequest CreateRequestData(string path, HTTPMethodType methodType, JsonData body)
    {
        Uri uri = new Uri(WebHost + path);

        if (HTTPMethodType.GET == methodType)
        {
            var keyValueParamList = new List<string>();

            if (null != body)
            {
                foreach (var key in body.Keys)
                {
                    // EscapeURL : URL에 사용하는 문자열로 인코딩 : 특수문자에 대해 16진수코드값으로 변환,,
                    keyValueParamList.Add(key + "=" + UnityWebRequest.EscapeURL(body[key].ToJson()));
                }
            }

            uri = new Uri(new Uri(WebHost), 
                        string.Format("{0}?{1}", path.TrimEnd('/'), string.Join("&", keyValueParamList.ToArray())));
        }
        
        string bodyString = GetBodyMessage(body);
        
        UnityWebRequest request;
        if (HTTPMethodType.GET == methodType)
            request = UnityWebRequest.Get(uri.AbsoluteUri);
        else
        {
            request = new UnityWebRequest(uri.AbsoluteUri);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyString));
        }

        request.method = methodType.ToString();
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new SHCustomCertificateHandler();
        request.timeout = 3;
        
        Debug.LogFormat("[REQUEST] : {0} {1}\nbody = {2}", methodType, uri.OriginalString, bodyString);

        request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        request.SetRequestHeader("Accept", "application/json");
        //request.SetRequestHeader("Authorization", JWTHeader.GetAuthorizationString());
        request.useHttpContinue = false;

        return request;
    }

    private string GetBodyMessage(JsonData body)
    {
        return (null == body) ? "{}" : JsonMapper.ToJson(body);
    }

    private async void ShowIndicator()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        await pUIRoot.ShowIndicator();
    }

    private async void CloseIndicator()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        await pUIRoot.CloseIndicator();
    }
}