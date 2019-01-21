using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public static partial class SHUtils
{
    // Component Missing 체크
    public static void CheckMissingComponent()
    {
#if UNITY_EDITOR
        var pObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (var pObject in pObjects)
        {
            if (null == pObject)
                return;

            foreach (var pComponent in (pObject as GameObject).GetComponents<Component>())
            {
                if (null == pComponent)
                {
                    UnityEngine.Debug.Log(string.Format("<color=red>[LSH] MissingComponent!!(GameObject{0})</color>", pObject.name));
                }
            }
        }
#endif
    }

    // 유니티 에디터의 Pause를 Toggle합니다.
    public static void EditorPauseOfToggle(bool bToggle)
    {
#if UNITY_EDITOR
        EditorApplication.isPaused = bToggle;
#endif
    }

    public static void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        var pThreads = Process.GetCurrentProcess().Threads;
        foreach(ProcessThread pThread in pThreads)
        {
            pThread.Dispose();
        }

        Process.GetCurrentProcess().Kill();
#endif
    }

    public static void OpenInFileBrowser(string strPath)
    {
        if (true == string.IsNullOrEmpty(strPath))
            return;

#if UNITY_EDITOR_WIN
        SHUtils.OpenInWinFileBrowser(strPath);
#elif UNITY_EDITOR_OSX
        SHUtils.OpenInMacFileBrowser(strPath);
#endif
    }

    public static void OpenInMacFileBrowser(string strPath)
     {
        strPath = strPath.Replace("\\", "/");

        if (false == strPath.StartsWith("\""))
            strPath = "\"" + strPath;

        if (false == strPath.EndsWith("\""))
            strPath = strPath + "\"";

        System.Diagnostics.Process.Start("open", ((true == Directory.Exists(strPath)) ? "" : "-R ") + strPath);
    }

    public static void OpenInWinFileBrowser(string strPath)
    {
        strPath = strPath.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", ((true == Directory.Exists(strPath)) ? "/root," : "/select,") + strPath);
    }
    
    // 콜스택 얻기
    public static string GetCallStack()
    {
        var pCallStack      = new StackTrace();
        var strCallStack    = string.Empty;
        foreach (var pFrame in pCallStack.GetFrames())
        {
            strCallStack += string.Format("{0}({1}) : {2}\n",
                pFrame.GetMethod(), pFrame.GetFileLineNumber(), pFrame.GetFileName());
        }

        return strCallStack;
    }
}