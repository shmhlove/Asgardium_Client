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

public class SHRequestData
{
    public string m_strPath;
    public JsonData m_pBody;
    public HTTPMethodType m_eMethodType;
    public Action<SHReply> m_pCallback;
    public eRequestStatus m_eRequestStatus;

    public SHRequestData(string path, HTTPMethodType methoodType, JsonData body, Action<SHReply> callback)
    {
        this.m_strPath = path;
        this.m_pBody = body;
        this.m_eMethodType = methoodType;
        this.m_pCallback = (null == callback) ? (reply) => { } : callback;
        this.m_eRequestStatus = eRequestStatus.Ready;
    }
}

public partial class SHNetworkManager : SHSingleton<SHNetworkManager>
{
    private string m_strWebHost = "";
    
    public override void OnInitialize()
    {
        SetServerInfo(() => {});
        SetDontDestroy();
    }
    
    private async void SetServerInfo(Action pCallback)
    {
        var pTable = await Single.Table.GetTable<SHTableClientConfig>();
        m_strWebHost     = pTable.ServerHost;
        m_iMaxRetryCount = pTable.MaxRetryRequestCount;
        pCallback();
    }
    
    private string GetBodyMessage(JsonData body)
    {
        return (null == body) ? "{}" : JsonMapper.ToJson(body);
    }

    private string GetJWT()
    {
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
        if ((null != pUserInfo) &&
            (false == string.IsNullOrEmpty(pUserInfo.UserId)))
        {
            payload.Add("access_token", pUserInfo.UserId);
        }

        JWT.JsonWebToken.JsonSerializer = new SHCustomJsonSerializer();
        return JWT.JsonWebToken.Encode(payload, SHCustomCertificateHandler.CERT_KEY, JWT.JwtHashAlgorithm.HS256);
    }
}