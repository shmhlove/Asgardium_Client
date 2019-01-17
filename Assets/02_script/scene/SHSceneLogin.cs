using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHSceneLogin : MonoBehaviour
{
    private SHUIRootLogin uiRoot = null;

    void Start()
    {
        uiRoot = Single.UI.GetRoot<SHUIRootLogin>();
        uiRoot.ShowLoginPanel(
            (strId, strPass) => 
            {
                JsonData json = new JsonData();
                json["name"] = strId;
                json["pass"] = strPass;
                Single.Network.POST(SHAPIs.SH_API_LOGIN, json, (reply) =>
                {
                    if (reply.isSucceed)
                    {
                    }
                    else
                    {
                    }
                });
            }, 
            (strId, strPass) => 
            {
                JsonData json = new JsonData();
                json["name"] = strId;
                json["pass"] = strPass;
                Single.Network.POST(SHAPIs.SH_API_SIGNUP, json, (reply) => 
                {
                    if (reply.isSucceed)
                    {
                    }
                    else
                    {
                    }
                });
            }, 
            () => 
            {
                Debug.Log("OnClickCloseButton");
            });
    }
}