using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHSceneLobby : MonoBehaviour
{
    public void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }
}
