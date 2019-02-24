using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHSceneTest : MonoBehaviour
{
    void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    void Start ()
    {
        Single.Network.GET(SHAPIs.SH_API_TEST, null, async (reply) =>
        {
            var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
            pUIRoot.ShowAlert(reply.ToString());
        });
    }
}
