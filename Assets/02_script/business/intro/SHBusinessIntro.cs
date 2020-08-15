using UnityEngine;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class SHBusinessIntro : MonoBehaviour
{
    void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    void Start()
    {
        Single.Scene.LoadScene(eSceneType.Patch, bIsUseFade: true, pCallback: (pReply) => 
        {

        });
    }
}
