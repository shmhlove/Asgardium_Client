using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHError
{
    public eErrorCode errorCode;
    public string message;
    public JsonData extras;

    public SHError(eErrorCode errorCode, string message, JsonData extras = null)
    {
        this.errorCode = errorCode;
        this.message = message;
        this.extras = extras;
    }

    public string toString()
    {
        if (null != extras)
            return string.Format("(ErrorCode : {0}) {1}, {2}", errorCode, message, extras.ToJson());
        else
            return string.Format("(ErrorCode : {0}) {1}", errorCode, message);
    }
}
