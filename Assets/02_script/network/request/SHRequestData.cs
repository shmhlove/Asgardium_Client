using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

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