using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHError
{
    public int code;
    public string message;
    public JsonData extras;

    public SHError(eErrorCode errorCode, string message, JsonData extras = null)
    {
        this.code = (int)errorCode;
        this.message = message;
        this.extras = extras;
    }

    public SHError(int errorCode, string message, JsonData extras = null)
    {
        this.code = errorCode;
        this.message = message;
        this.extras = extras;
    }

    public override string ToString()
    {
        if (null != extras)
            return string.Format("{0} ({1})\n{2}", message, code, extras.ToJson());
        else
            return string.Format("{0} ({1})", message, code);
    }
}
