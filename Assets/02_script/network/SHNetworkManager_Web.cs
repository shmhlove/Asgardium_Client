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

public enum HTTPMethodType
{
    GET,
    POST,
    DELETE,
    UPDATE,
    PUT,
}

public enum eRequestStatus
{
    Ready,
    Requesting,
    Done,
}

public partial class SHNetworkManager : SHSingleton<SHNetworkManager>
{
    private bool m_bIsConnectWebServer = false;
    private bool m_bIsRetryingWebServerConnect = false;
    private List<SHRequestData> m_pWebRequestQueue = new List<SHRequestData>();

    public void GET(string path, JsonData body, Action<SHReply> callback)
    {
        SendRequestWeb(new SHRequestData(path, HTTPMethodType.GET, body, callback));
    }

    public void POST(string path, JsonData body, Action<SHReply> callback)
    {
        SendRequestWeb(new SHRequestData(path, HTTPMethodType.POST, body, callback));
    }

    public void SendRequestWeb(SHRequestData pRequestData)
    {
        m_pWebRequestQueue.Add(pRequestData);

        if (true == m_bIsProcessingRetry)
            return;
        
        if (true == m_bIsRetryingWebServerConnect)
            return;

        StartCoroutine(CoroutineSendRequest(pRequestData));
    }

    private IEnumerator CoroutineSendRequest(SHRequestData pRequestData)
    {
        Single.Global.GetIndicator().Show();

        pRequestData.m_eRequestStatus = eRequestStatus.Requesting;

        var pWebRequest = CreateUnityRequestData(pRequestData);

        DebugLogOfWebRequest(pWebRequest);

        yield return pWebRequest.SendWebRequest();
        while (false == pWebRequest.isDone)
            yield return null;

        pRequestData.m_eRequestStatus = eRequestStatus.Done;

        var pReply = new SHReply(pWebRequest);

        DebugLogOfWebResponse(pReply);

        if (true == (m_bIsConnectWebServer = ((int)eErrorCode.Net_Common_HTTP != pReply.errorCode)))
        {
            pRequestData.m_pCallback(pReply);
            m_pWebRequestQueue.Remove(pRequestData);

            if (0 == m_pWebRequestQueue.Count)
            {
                Single.Global.GetIndicator().Close();
            }
        }
        else
        {
            StartRetryProcess();
        }
    }

    private IEnumerator CoroutineRetryWebServerProcess()
    {
        // 예외처리 : 연결이 되어있으면 무시
        if (true == m_bIsConnectWebServer)
        {
            yield break;
        }

        // 예외처리 : 재시도 시도중이면 무시
        if (true == m_bIsRetryingWebServerConnect)
        {
            yield break;
        }

        // 재시도 준비
        m_bIsRetryingWebServerConnect = true;
        
        // 응답이 오지않은 요청들 기다려주기
        while (true)
        {
            var pRequestings = m_pWebRequestQueue.Find((pReq) => 
            {
                return (pReq.m_eRequestStatus == eRequestStatus.Requesting);
            });

            if (null == pRequestings)
            {
                break;
            }
            
            yield return null;
        }

        // 연결체크 Request Send
        var pWebRequest = CreateUnityRequestData(
            new SHRequestData(SHAPIs.SH_API_RETRY_REQUEST, HTTPMethodType.GET, null, null));

        DebugLogOfWebRequest(pWebRequest);

        yield return pWebRequest.SendWebRequest();
        while (false == pWebRequest.isDone)
            yield return null;

        var pReply = new SHReply(pWebRequest);

        DebugLogOfWebResponse(pReply);

        if (true == (m_bIsConnectWebServer = ((int)eErrorCode.Net_Common_HTTP != pReply.errorCode)))
        {
            StopRetryProcess();

            foreach (var pReq in m_pWebRequestQueue)
            {
                StartCoroutine(CoroutineSendRequest(pReq));
            }
        }
        else
        {
            StartRetryProcess();
        }

        m_bIsRetryingWebServerConnect = false;
    }

    private UnityWebRequest CreateUnityRequestData(SHRequestData pData)
    {
        var uri = new Uri(m_strWebHost + pData.m_strPath);

        if (HTTPMethodType.GET == pData.m_eMethodType)
        {
            var keyValueParamList = new List<string>();
            
            if (null != pData.m_pBody)
            {
                foreach (var key in pData.m_pBody.Keys)
                {
                    // EscapeURL : URL에는 정해진 ASKII 밖에 사용할 수 없기 때문에 특수문자에 대해 16진수코드값으로 변환,,
                    keyValueParamList.Add(key + "=" + UnityWebRequest.EscapeURL(pData.m_pBody[key].ToJson()));
                }
            }

            uri = new Uri(new Uri(m_strWebHost), 
                        string.Format("{0}?{1}", pData.m_strPath.TrimEnd('/'), string.Join("&", keyValueParamList.ToArray())));
        }
        
        var request = new UnityWebRequest(uri.AbsoluteUri);
        if (HTTPMethodType.POST == pData.m_eMethodType)
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(GetBodyMessage(pData.m_pBody)));
        }

        request.method = pData.m_eMethodType.ToString();
        request.downloadHandler = new DownloadHandlerBuffer();
        request.certificateHandler = new SHCustomCertificateHandler();

        request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Authorization", GetJWT());
        request.useHttpContinue = false;
        
        return request;
    }

    private bool IsWebServerConnected()
    {
        return m_bIsConnectWebServer;
    }

    private void DebugLogOfWebRequest(UnityWebRequest pData)
    {
        if (null == pData.uploadHandler)
        {
            Debug.LogFormat("<color=#666600>[WEB_REQUEST]</color> : {0} {1}\nheader = JWT {2}\nbody = {3}", 
                pData.method, pData.url, pData.GetRequestHeader("Authorization"), "{}");
        }
        else
        {
            Debug.LogFormat("<color=#666600>[WEB_REQUEST]</color> : {0} {1}\nheader = JWT {2}\nbody = {3}", 
                pData.method, pData.url, pData.GetRequestHeader("Authorization"), Encoding.UTF8.GetString(pData.uploadHandler.data));
        }
    }

    private void DebugLogOfWebResponse(SHReply pReply)
    {
        var strLog = string.Format("<color=#0033ff>[WEB_RESPONSE]</color> : {0} {1}\n{2}",
                pReply.requestMethod,
                pReply.requestUrl,
                pReply.ToString());

        if (pReply.isSucceed)
        {
            Debug.Log(strLog);
        }
        else
        {
            Debug.LogWarning(strLog);
        }
    }
}