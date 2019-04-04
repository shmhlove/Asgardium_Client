using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHBusinessTest : MonoBehaviour
{
    void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    [FuncButton]
    void OnClickSendRequest()
    {
        Single.Network.GET(SHAPIs.SH_API_RETRY_REQUEST, null, (reply) =>
        {
            Single.BusinessGlobal.ShowAlertUI(reply);
        });
    }
}
