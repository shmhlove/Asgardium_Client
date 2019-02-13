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
    public JsonData rawResponse;

    public SHReply()
    {
        this.isSucceed = true;
        this.data = null;
        this.error = null;
        this.rawResponse = null;
    }

    public SHReply(JsonData data)
    {
        this.isSucceed = true;
        this.data = data;
        this.error = null;
        this.rawResponse = null;
    }

    public SHReply(SHError error)
    {
        this.isSucceed = false;
        this.data = null;
        this.error = error;
        this.rawResponse = null;
    }

    public SHReply(UnityWebRequest request)
    {
        this.data = null;
        this.error = null;
        this.rawResponse = null;

#if UNITY_2017_1_OR_NEWER
        if (request.isNetworkError)
#else
        if (request.isError)
#endif
        {
            this.isSucceed = false;
            this.error = new SHError(eErrorCode.Net_Common_HTTP, request.error);
            this.rawResponse = new JsonData(request.error);
        }
        else
        {
            try
            {
                this.rawResponse = JsonMapper.ToObject(request.downloadHandler.text);
                if (0 == this.rawResponse.Keys.Count)
                {
                    this.isSucceed = false;
                    this.error = new SHError(eErrorCode.Net_Common_InvalidResponseData, request.downloadHandler.text);
                }
                else
                {
                    if (true == this.rawResponse.Keys.Contains("result"))
                    {
                        this.isSucceed = (bool)this.rawResponse["result"];
                    }

                    if ((true == this.rawResponse.Keys.Contains("data")) && (null != this.rawResponse["data"]))
                    {
                        this.data = this.rawResponse["data"];
                    }

                    if ((true == this.rawResponse.Keys.Contains("error")) && (null != this.rawResponse["error"]))
                    {
                        if (this.rawResponse["error"].Keys.Contains("extras"))
                            this.error = new SHError((eErrorCode)(int)this.rawResponse["error"]["code"], (string)this.rawResponse["error"]["message"], this.rawResponse["error"]["extras"]);
                        else
                            this.error = new SHError((eErrorCode)(int)this.rawResponse["error"]["code"], (string)this.rawResponse["error"]["message"]);
                    }
                }
            }
            catch
            {
                this.isSucceed = false;
                this.error = new SHError(eErrorCode.Net_Common_JsonParse, string.Format("{0}\n{1}", "Err Json Parse With Server ResponseData", request.downloadHandler.text));
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
                this.error.ToString());
        }
        
        if ( (null == this.data)
          && (null == this.error)
          && (null != this.rawResponse))
        {
            Debug.LogFormat("[RESPONSE rawData] : {0} {1}\n{2}",
                request.method,
                request.url,
                this.rawResponse.ToJson());
        }

        request.Dispose();
    }

    public override string ToString()
    {
        if (null != data)
        {
            return data.ToJson();
        }

        if (null != error)
        {
            return error.ToString();
        }

        return string.Empty;
    }
}
