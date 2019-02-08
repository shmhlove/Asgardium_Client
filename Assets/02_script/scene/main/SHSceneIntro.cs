using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHSceneIntro : MonoBehaviour
{
    void Awake()
    {
        Single.AppInfo.CreateSingleton();
        Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL, (pUIRoot) => {});
    }

    void Start()
    {
        Single.Scene.LoadScene(eSceneType.Login, bIsUseFade: true, pCallback: (pReply) => 
        {

        });
    }
}
