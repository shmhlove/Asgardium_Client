#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHActivator : Editor
{
    // % :  Winows의 Ctrl, macOS의 cmd키
    // # : shift
    // & : alt
    // 숫자나 문자 등을 사용하려면 _(언더바)를 같이 쓰면 된다.
    // ex) G키를 등록하고 싶으면 _g
    // 언더바 앞에 꼭 공백(space)문자가 있어야한다.
    // LEFT, RIGHT, UP, DOWN, F1..F12, HOME, END, PGUP, PGDN 등의 키도 사용 가능.

    [MenuItem("SHTools/Editor/GameObject Activator &a", false, 1000)]
    static void GameObjectActivator()
    {
        foreach (var pObject in Selection.gameObjects)
        {
            pObject.SetActive(!pObject.activeInHierarchy);
        }
    }
}
#endif