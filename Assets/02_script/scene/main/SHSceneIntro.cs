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
    }

    void Start()
    {
        Single.UI.GetRoot<SHUIRootIntro>(SHUIConstant.ROOT_INTRO, (pUIRoot) => 
        {
            pUIRoot.Show(() => 
            {
                Single.Scene.LoadScene(eSceneType.Login, bIsUseFade: true, pCallback: (pReply) => 
                {

                });
            });
        });
    }
}
