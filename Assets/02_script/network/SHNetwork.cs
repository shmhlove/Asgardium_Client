using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using LitJson;

public enum HTTPMethodType
{
    GET,
    POST,
    DELETE,
    UPDATE,
    PUT,
}

public class SHNetwork : SHSingleton<SHNetwork>
{
    public string WebHost = "http://13.124.43.70:3001";
    //public string WebHost = "http://localhost:3001";

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
        StartCoroutine(CoroutineSendRequest(CreateRequestData(path, methoodType, body), callback));
    }

    private IEnumerator CoroutineSendRequest(UnityWebRequest www, Action<SHReply> callback)
    {
        yield return www.Send();
        
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
                    keyValueParamList.Add(key + "=" + WWW.EscapeURL(body[key].ToJson()));
                }
            }
            
            uri = new Uri(new Uri(WebHost), 
                        string.Format("{0}?{1}", path.TrimEnd('/'), string.Join("&", keyValueParamList.ToArray())));
        }
        
        string bodyString = GetBodyMessage((HTTPMethodType.GET == methodType) ? null : body);

        UnityWebRequest request = new UnityWebRequest(uri.AbsoluteUri);
        request.method = methodType.ToString();
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyString));
        request.timeout = 3;
        
        Debug.LogFormat("[REQUEST] : {0} {1}\nbody = {2}",
         methodType,
         uri.OriginalString,
         bodyString);

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
}