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

    public string requestMethod;
    public string requestUrl;

    public eErrorCode errorCode
    {
        get
        {
            if (null == error)
            {
                if (isSucceed)
                    return eErrorCode.Succeed;
                else
                    return eErrorCode.Failed;
            }
            
            return error.code;
        }
    }

    public SHReply()
    {
        this.isSucceed = true;
        this.data = null;
        this.error = null;
        this.rawResponse = null;
        this.requestMethod = string.Empty;
        this.requestUrl = string.Empty;
    }
    
    public SHReply(JsonData data)
    {
        this.isSucceed = true;
        this.data = data;
        this.error = null;
        this.rawResponse = data;
        this.requestMethod = string.Empty;
        this.requestUrl = string.Empty;
    }

    public SHReply(SHError error)
    {
        this.isSucceed = false;
        this.data = null;
        this.error = error;
        this.rawResponse = null;
        this.requestMethod = string.Empty;
        this.requestUrl = string.Empty;
    }

    public SHReply(UnityWebRequest request)
    {
        this.data = null;
        this.error = null;
        this.rawResponse = null;
        this.requestMethod = request.method;
        this.requestUrl = request.url;
        
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

                    if ((true == this.rawResponse.Keys.Contains("data")) 
                        && (null != this.rawResponse["data"]))
                    {
                        this.data = this.rawResponse["data"];
                    }

                    if ((true == this.rawResponse.Keys.Contains("error")) 
                        && (null != this.rawResponse["error"]))
                    {
                        var pError = this.rawResponse["error"];
                        if (pError.Keys.Contains("extras"))
                            this.error = new SHError((eErrorCode)(int)pError["code"], (string)pError["message"], pError["extras"]);
                        else
                            this.error = new SHError((eErrorCode)(int)pError["code"], (string)pError["message"]);
                    }
                }
            }
            catch
            {
                this.isSucceed = false;
                this.error = new SHError(
                    eErrorCode.Net_Common_JsonParse, 
                    string.Format("{0}\n{1}", "Err Json Parse With Server ResponseData", 
                    request.downloadHandler.text));
            }
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

        if (null != rawResponse)
        {
            return rawResponse.ToJson();
        }

        return string.Empty;
    }
}
