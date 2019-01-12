using UnityEngine;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using LitJson;

public class SHReply
{
    public bool isSucceed;
    public JsonData data;
    public SHError error;

    public SHReply(UnityWebRequest request)
    {
        this.isSucceed = !request.isError;
        this.data = null;
        this.error = null;

#if UNITY_2017_1_OR_NEWER
        if (request.isNetworkError)
#else
        if (request.isError)
#endif
        {
            this.isSucceed = false;
            this.error = new SHError(SHErrorCode.HTTP, request.error);
        }
        else
        {
            try
            {
                JsonData response = JsonMapper.ToObject(request.downloadHandler.text);
                if (0 == response.Keys.Count)
                {
                    this.isSucceed = false;
                    this.error = new SHError(SHErrorCode.Common_InvalidResponseData, request.downloadHandler.text);
                }
                else
                {
                    if (true == response.Keys.Contains("result"))
                    {
                        this.isSucceed = (bool)response["result"];
                    }

                    if ((true == response.Keys.Contains("data")) && (null != response["data"]))
                    {
                        this.data = response["data"];
                    }

                    if ((true == response.Keys.Contains("error")) && (null != response["error"]))
                    {
                        if (response["error"].Keys.Contains("extras"))
                            this.error = new SHError((SHErrorCode)(int)response["error"]["code"], (string)response["error"]["message"], response["error"]["extras"]);
                        else
                            this.error = new SHError((SHErrorCode)(int)response["error"]["code"], (string)response["error"]["message"]);
                    }
                }
            }
            catch
            {
                this.isSucceed = false;
                this.error = new SHError(SHErrorCode.Common_JsonParse, string.Format("{0}\n{1}", "Err Json Parse With Server ResponseData", request.downloadHandler.text));
            }
        }

        if (null != this.data)
        {
            Debug.LogFormat("[RESPONSE Succeed] : {0} {1}\n{2}",
                request.method,
                request.url,
                this.data.ToJson());
        }

        if (null != this.error)
        {
            Debug.LogFormat("[RESPONSE Error] : {0} {1}\n{2}",
                request.method,
                request.url,
                this.error.toString());
        }
        
        request.Dispose();
    }
}
