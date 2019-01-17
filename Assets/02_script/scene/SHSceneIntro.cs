using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHSceneIntro : MonoBehaviour
{
    void Start()
    {
        var pUIRoot = Single.UI.GetRoot<SHUIRootIntro>();
        pUIRoot.Show(() => 
        {
            Single.Scene.LoadScene(eSceneType.Login, LoadSceneMode.Single, false, (pReply) => 
            {

            });
        });
    }
}
