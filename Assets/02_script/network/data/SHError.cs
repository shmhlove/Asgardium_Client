using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHError
{
    public SHErrorCode errorCode;
    public string message;

    public SHError(SHErrorCode errorCode, string message)
    {
        this.errorCode = errorCode;
        this.message = message;
    }

    public string toString()
    {
        return string.Format("(ErrorCode : {0}) {1}", errorCode, message);
    }
}
