/*
    @@ 에러팝업출력 / 로그 / 인디케이터UI컨트롤 / 리트라이 하는 부분은 비지니스로 빼면 좋을꺼같다.
    @@ 여기는 진짜 요청받은대로 보내고, 받는것만 하고
*/

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

public enum eWebRequestStatus
{
    Ready,
    Requesting,
    Done,
}

public class SHWebRequestData
{
    public string m_strPath;
    public JsonData m_pBody;
    public HTTPMethodType m_eMethodType;
    public Action<SHReply> m_pCallback;
    public eWebRequestStatus m_eRequestStatus;

    public SHWebRequestData(string path, HTTPMethodType methoodType, JsonData body, Action<SHReply> callback)
    {
        this.m_strPath = path;
        this.m_pBody = body;
        this.m_eMethodType = methoodType;
        this.m_pCallback = (null == callback) ? (reply) => {} : callback;
        this.m_eRequestStatus = eWebRequestStatus.Ready;
    }
}

public partial class SHNetworkManager : SHSingleton<SHNetworkManager>
{
    private int m_iRetryCount = 0;
    private bool m_bIsProcessingRetry = false;
    private List<SHWebRequestData> m_pWebRequestQueue = new List<SHWebRequestData>();

    public void GET(string path, JsonData body, Action<SHReply> callback)
    {
        SendRequest(new SHWebRequestData(path, HTTPMethodType.GET, body, callback));
    }

    public void POST(string path, JsonData body, Action<SHReply> callback)
    {
        SendRequest(new SHWebRequestData(path, HTTPMethodType.POST, body, callback));
    }

    public async void SendRequest(SHWebRequestData pRequestData)
    {
        m_pWebRequestQueue.Add(pRequestData);

        if (true == m_bIsProcessingRetry)
            return;

        if (null == m_pStringTable)
        {
            m_pStringTable = await Single.Table.GetTable<SHTableClientString>();
        }
        
        StartCoroutine(CoroutineSendRequest(pRequestData));
    }

    private IEnumerator CoroutineSendRequest(SHWebRequestData pRequestData)
    {
        Single.BusinessGlobal.ShowIndicator();

        pRequestData.m_eRequestStatus = eWebRequestStatus.Requesting;

        var pWebRequest = CreateUnityRequestData(pRequestData);

        DebugLogOfRequest(pWebRequest);

        yield return pWebRequest.SendWebRequest();
        
        while (false == pWebRequest.isDone)
            yield return null;

        pRequestData.m_eRequestStatus = eWebRequestStatus.Done;

        var pReply = new SHReply(pWebRequest);

        DebugLogOfResponse(pReply);

        if (eErrorCode.Net_Common_HTTP == pReply.errorCode)
        {
            StartCoroutine(CoroutineRetryProcess(pRequestData));
        }
        else
        {
            CallbackAndRemovePool(pRequestData, pReply);

            if (0 == m_pWebRequestQueue.Count)
            {
                Single.BusinessGlobal.CloseIndicator();
            }
        }
    }

    private IEnumerator CoroutineRetryProcess(SHWebRequestData pRequestData)
    {
        // 이미 Retry 진행 중이면 대기(Retry 요청은 예외)
        if ((SHAPIs.SH_API_RETRY_REQUEST != pRequestData.m_strPath)
            && (true == m_bIsProcessingRetry) )
            yield break;

        m_bIsProcessingRetry = true;

        // 모든 요청이 돌아올때까지 대기
        while (true)
        {
            var pRequestings = m_pWebRequestQueue.Find((pReq) => 
            {
                return (pReq.m_eRequestStatus == eWebRequestStatus.Requesting);
            });

            if (null == pRequestings)
                break;
            
            yield return null;
        }

        Action pRetryAction = () => 
        {
            StartCoroutine(CoroutineSendRequest(new SHWebRequestData(SHAPIs.SH_API_RETRY_REQUEST, HTTPMethodType.GET, null, (pReply) => 
            {
                m_iRetryCount = 0;
                m_bIsProcessingRetry = false;
                Single.BusinessGlobal.UpdateIndicatorMessage(string.Empty);

                Debug.LogFormat("[LSH] WebRequestQueue Count : {0}", m_pWebRequestQueue.Count);

                foreach (var pReq in m_pWebRequestQueue)
                {
                    StartCoroutine(CoroutineSendRequest(pReq));
                }
            })));
        };
        
        var strErrorMessage = string.Empty;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            strErrorMessage = m_pStringTable.GetString("1006");
        }
        else
        {
            strErrorMessage = m_pStringTable.GetString("1007");
        }

        if (m_iRetryCount++ < m_iMaxRetryCount)
        {
            // 자동 재시도 처리
            var strRetryInfo = string.Format(m_pStringTable.GetString("1008"), m_iRetryCount, m_iMaxRetryCount);
            Single.BusinessGlobal.UpdateIndicatorMessage(string.Format("{0}\n{1}", strRetryInfo, strErrorMessage));

            yield return new WaitForSeconds(1.5f);
            pRetryAction();
        }
        else
        {            
            // 수동 재시도 처리
            var pAlertInfo = new SHUIAlertInfo();
            pAlertInfo.m_strMessage = string.Format("{0}\n{1}", strErrorMessage, m_pStringTable.GetString("1009"));
            pAlertInfo.m_strTwoLeftBtnLabel = m_pStringTable.GetString("10012");
            pAlertInfo.m_strTwoRightBtnLabel = m_pStringTable.GetString("10013");
            pAlertInfo.m_eButtonType = eAlertButtonType.TwoButton;
            pAlertInfo.m_pCallback = (eSelectBtnType) => 
            {
                if (eAlertButtonAction.Left_Button == eSelectBtnType)
                {
                    m_iRetryCount = 0;
                    pRetryAction();
                }
                else
                {
                    SHUtils.GameQuit();
                }
            };
            Single.BusinessGlobal.ShowAlertUI(pAlertInfo);
        }
    }

    private UnityWebRequest CreateUnityRequestData(SHWebRequestData pData)
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

    private void CallbackAndRemovePool(SHWebRequestData pRequestData, SHReply pReply)
    {
        pRequestData.m_pCallback(pReply);
        m_pWebRequestQueue.Remove(pRequestData);
    }

    private void DebugLogOfRequest(UnityWebRequest pData)
    {
        if (null == pData.uploadHandler)
        {
            Debug.LogFormat("[REQUEST] : {0} {1}\nheader = JWT {2}\nbody = {3}", 
                pData.method, pData.url, pData.GetRequestHeader("Authorization"), "{}");
                
        }
        else
        {
            Debug.LogFormat("[REQUEST] : {0} {1}\nheader = JWT {2}\nbody = {3}", 
                pData.method, pData.url, pData.GetRequestHeader("Authorization"), Encoding.UTF8.GetString(pData.uploadHandler.data));
        }
    }

    private void DebugLogOfResponse(SHReply pReply)
    {
        var strLog = string.Format("[RESPONSE] : {0} {1}\n{2}",
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