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
    private bool m_bIsConnectWebServer = true;
    private bool m_bIsRunRetryCoroutine = false;
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
        
        StartCoroutine(CoroutineSendRequest(pRequestData));
    }

    private IEnumerator CoroutineSendRequest(SHRequestData pRequestData)
    {
        Single.BusinessGlobal.ShowIndicator();

        pRequestData.m_eRequestStatus = eRequestStatus.Requesting;

        var pWebRequest = CreateUnityRequestData(pRequestData);

        DebugLogOfRequest(pWebRequest);

        yield return pWebRequest.SendWebRequest();
        while (false == pWebRequest.isDone)
            yield return null;

        pRequestData.m_eRequestStatus = eRequestStatus.Done;

        var pReply = new SHReply(pWebRequest);

        DebugLogOfResponse(pReply);

        m_bIsConnectWebServer = (eErrorCode.Net_Common_HTTP != pReply.errorCode);

        if (true == m_bIsConnectWebServer)
        {
            CallbackAndRemovePool(pRequestData, pReply);

            if (0 == m_pWebRequestQueue.Count)
            {
                Single.BusinessGlobal.CloseIndicator();
            }
        }
        else
        {
            StartRetryProcess();
        }
    }

    // 리트라이 상황이 발생하면 n초에 한번씩 무조건 호출된다.
    private IEnumerator CoroutineRetryWebServerProcess()
    {
        if (true == m_bIsConnectWebServer)
        {
            yield break;
        }

        if (true == m_bIsRunRetryCoroutine)
        {
            yield break;
        }

        m_bIsRunRetryCoroutine = true;

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

        // 여기서 꼬이네...
        // 응답이 오면 다시 재시도 가야되는데 흠...
        StartCoroutine(CoroutineSendRequest(new SHRequestData(SHAPIs.SH_API_RETRY_REQUEST, HTTPMethodType.GET, null, (pReply) => 
        {
            // @@ 아래코드는 대기타다가 웹소켓도 성공 했을때 처리해야한다.

            StopRetryProcess();
            
            Debug.LogFormat("[LSH] WebRequestQueue Count : {0}", m_pWebRequestQueue.Count);

            foreach (var pReq in m_pWebRequestQueue)
            {
                StartCoroutine(CoroutineSendRequest(pReq));
            }
        })));

        m_bIsRunRetryCoroutine = false;
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

        // payload
        // 등록된 클레임
        // iss: 토큰 발급자(issuer)
        // sub: 토큰 제목(subject)
        // aud: 토큰 대상자(audience)
        // exp: 토큰의 만료시간(expiraton), 시간은 NumericDate 형식으로 되어있어야 하며(예: 1480849147370) 언제나 현재 시간보다 이후로 설정되어있어야합니다.
        // nbf: Not Before 를 의미하며, 토큰의 활성 날짜와 비슷한 개념입니다. 여기에도 NumericDate 형식으로 날짜를 지정하며, 이 날짜가 지나기 전까지는 토큰이 처리되지 않습니다.
        // iat: 토큰이 발급된 시간(issued at), 이 값을 사용하여 토큰의 age 가 얼마나 되었는지 판단 할 수 있습니다.
        // jti: JWT의 고유 식별자로서, 주로 중복적인 처리를 방지하기 위하여 사용됩니다. 일회용 토큰에 사용하면 유용합니다.
        var payload = new Dictionary<string, object>();
        payload.Add("iss", "MangoNight.Studio.com");
        payload.Add("sub", "MangoNight.JWT");
        payload.Add("aud", "Asgardium");
        payload.Add("iat", ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString());

        // 비공개 클레임
        var pUserInfo = Single.Table.GetTableSync<SHTableUserInfo>();
        if ((null != pUserInfo ) &&
            (false == string.IsNullOrEmpty(pUserInfo.UserId))) {
            payload.Add("access_token", pUserInfo.UserId);
        }

        JWT.JsonWebToken.JsonSerializer = new SHCustomJsonSerializer();
        var strJWT = JWT.JsonWebToken.Encode(payload, SHCustomCertificateHandler.CERT_KEY, JWT.JwtHashAlgorithm.HS256);
        
        request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Authorization", strJWT);
        request.useHttpContinue = false;
        
        return request;
    }

    private void CallbackAndRemovePool(SHRequestData pRequestData, SHReply pReply)
    {
        pRequestData.m_pCallback(pReply);
        m_pWebRequestQueue.Remove(pRequestData);
    }
    
    private bool IsWebServerConnected()
    {
        return m_bIsConnectWebServer;
    }

    private void DebugLogOfRequest(UnityWebRequest pData)
    {
        if (null == pData.uploadHandler)
        {
            Debug.LogFormat("<color=#666600>[REQUEST]</color> : {0} {1}\nheader = JWT {2}\nbody = {3}", 
                pData.method, pData.url, pData.GetRequestHeader("Authorization"), "{}");
        }
        else
        {
            Debug.LogFormat("<color=#666600>[REQUEST]</color> : {0} {1}\nheader = JWT {2}\nbody = {3}", 
                pData.method, pData.url, pData.GetRequestHeader("Authorization"), Encoding.UTF8.GetString(pData.uploadHandler.data));
        }
    }

    private void DebugLogOfResponse(SHReply pReply)
    {
        var strLog = string.Format("<color=#0033ff>[RESPONSE]</color> : {0} {1}\n{2}",
                pReply.requestMethod,
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