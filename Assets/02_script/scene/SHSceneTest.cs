using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHSceneTest : MonoBehaviour
{
	void Start ()
    {
        Single.network.GET(SHAPIs.SH_API_TEST, null, (reply) =>
        {
            if (reply.isSucceed)
            {

            }
            else
            {

            }
        });

        JsonData json = new JsonData();
        json["name"] = "unity test";
        json["pass"] = "1234";
        Single.network.POST(SHAPIs.SH_API_LOGIN, json, (reply) =>
        {
            if (reply.isSucceed)
            {

            }
            else
            {

            }
        });

        json["name"] = "unity test";
        json["pass"] = "1234";
        Single.network.POST(SHAPIs.SH_API_SIGNUP, json, (reply) => 
        {
            if (reply.isSucceed)
            {

            }
            else
            {

            }
        });
    }
}
