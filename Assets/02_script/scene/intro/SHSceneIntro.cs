﻿using UnityEngine;
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
        Single.Scene.LoadScene(eSceneType.Update, bIsUseFade: true, pCallback: (pReply) => 
        {

        });
    }
}