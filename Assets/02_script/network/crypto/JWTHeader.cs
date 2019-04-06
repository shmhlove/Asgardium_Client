using UnityEngine;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using LitJson;

public class JWTHeader
{
    public static string GetAuthorizationString()
    {
        StringBuilder sb = new StringBuilder();
        
        // header
        var header = new Dictionary<string, string>();
        header.Add("alg", "RS256");
        header.Add("typ", "JWT");
        
        // payload
        var payload = new Dictionary<string, object>();
        payload.Add("iss", "MangoNight.Client.com");
        payload.Add("sub", "MangoNight");
        payload.Add("aud", "Asgardium");
        payload.Add("iat", ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString());
        
        // 클라에서 헤더.페이로드를 만들고, 비밀키로 암호화한걸 시그니쳐로만들어 서버로 보낸다.
        // 서버는 전달 받은 헤더.페이로드로 비밀키로 암호화된 시그니쳐를 만들어, 클라에서 보낸 시그니쳐와 비교한다.
        // access_token은... 일단 패스

        // if (!string.IsNullOrEmpty(Session.Instance.Id))
        // {
        //     payload.Add("access_token", Session.Instance.Id);
        // }

        // if (this.AccessTokenType == AccessTokenType.Unknown)
        // {
        //     if (!string.IsNullOrEmpty(Session.Instance.Id))
        //     {
        //         this.AccessTokenType = AccessTokenType.Session;
        //     }
        //     else
        //     {
        //         this.AccessTokenType = AccessTokenType.App;
        //     }
        // }
        // payload.Add("typ", this.AccessTokenType.ToString().ToLower());

        sb.Append(SHUtils.Base64ForUrlEncode(JsonMapper.ToJson(header))).Append(".").Append(SHUtils.Base64ForUrlEncode(JsonMapper.ToJson(payload)));

        // Verify Signature
        //var strCertKey = SHCustomCertificateHandler.CERT_KEY;
        var strCertKey = "MIICizCCAfQCCQDwf4v6CJfa6TANBgkqhkiG9w0BAQsFADCBiTELMAkGA1UEBhMCa3IxDjAMBgNVBAgMBXNlb3VsMRAwDgYDVQQHDAdnYW5nbmFtMRMwEQYDVQQKDAptYW5nb25pZ2h0MRIwEAYDVQQLDAlhc2dhcmRpdW0xDDAKBgNVBAMMA2F3czEhMB8GCSqGSIb3DQEJARYSc2htaGxvdmVAbmF2ZXIuY29tMB4XDTE5MDIxMzE0MDYxN1oXDTE5MDMxNTE0MDYxN1owgYkxCzAJBgNVBAYTAmtyMQ4wDAYDVQQIDAVzZW91bDEQMA4GA1UEBwwHZ2FuZ25hbTETMBEGA1UECgwKbWFuZ29uaWdodDESMBAGA1UECwwJYXNnYXJkaXVtMQwwCgYDVQQDDANhd3MxITAfBgkqhkiG9w0BCQEWEnNobWhsb3ZlQG5hdmVyLmNvbTCBnzANBgkqhkiG9w0BAQEFAAOBjQAwgYkCgYEA9I4ydsDhikcLXt6UfzquTmuWhqpsEuQx+30I69voyRPoD5k3y42HDvBgolwCwximBfUuMX/+lpe9kBWC7Zk+dwldSmGB2mTM5gypLx3deU+uTTEnnO7dG1ARvF7NMwPXaqRsQpAEhvLkKUBNA61N7j9a5o12st/qU9WQUm5ycYECAwEAATANBgkqhkiG9w0BAQsFAAOBgQBkzdMbx9PT2UntCMSbRXnEWL8Qcxv2U1u8TjZph4qn1P11xG05DNCHHulOGVAKspUGkpyMw1/VoiaEKd2J7rPPHDJ6oZj5MsUm2weuERS9UOMDOjvBvfLmoZcPrR0TMPivu1EmyuoBtSab0X4JQbzYCVhqPlQ3RE2Mkh9gv8gS+w==";
        byte[] rawData = System.Convert.FromBase64String(strCertKey);
        string password = "Abcde12#";
        var cert = new X509Certificate2(rawData, password);
        
        using (var rsa = (RSACryptoServiceProvider)cert.PrivateKey)
        {             
            byte[] signature = rsa.SignData(Encoding.UTF8.GetBytes(sb.ToString()), "SHA256");
        
            string rsasha256Base64Encoded = SHUtils.Base64ForUrlEncode(signature);
            sb.Append(".").Append(rsasha256Base64Encoded);
        }
        
        return "JWT " + sb.ToString();
    }
}
