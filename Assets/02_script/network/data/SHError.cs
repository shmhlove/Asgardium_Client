using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHError
{
    public eErrorCode code;
    public string message;
    public JsonData extras;

    public SHError(eErrorCode errorCode, string message, JsonData extras = null)
    {
        this.code = errorCode;
        this.message = message;
        this.extras = extras;
    }

    public override string ToString()
    {
        if (null != extras)
            return string.Format("{0} ({1})\n{2}", message, (int)code, extras.ToJson());
        else
            return string.Format("{0} ({1})", message, (int)code);
    }
}
