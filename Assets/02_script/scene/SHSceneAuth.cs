using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;

public class SHSceneAuth : MonoBehaviour
{
    private SHUIRootAuth uiRoot = null;

    void Start()
    {
        uiRoot = Single.UI.GetRoot<SHUIRootAuth>();
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