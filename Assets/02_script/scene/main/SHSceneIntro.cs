using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHSceneIntro : MonoBehaviour
{
    void Start()
    {
        Single.UI.GetRoot<SHUIRootIntro>((pUIRoot) => 
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
