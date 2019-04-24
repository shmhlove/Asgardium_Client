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
        
        var strJWT = JWTHeader.GetAuthorizationString();
        Debug.Log(strJWT);
        
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
            Debug.LogFormat("[REQUEST] : {0} {1}\nbody = {2}", 
                pData.method, pData.url, "{}");
        }
        else
        {
            Debug.LogFormat("[REQUEST] : {0} {1}\nbody = {2}", 
                pData.method, pData.url, Encoding.UTF8.GetString(pData.uploadHandler.data));
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