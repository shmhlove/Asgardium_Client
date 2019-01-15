#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHEditorCleanner : Editor 
{
    [MenuItem("SHTools/Editor/Clean Cache", false, 100)]
    [MenuItem("Assets/SHTools/Editor/Clean Cache", false, 100)]
    static void SelectToCleanCache()
    {
        if (true == Caching.CleanCache())
        {
            EditorUtility.DisplayDialog("알림", "캐시가 삭제되었습니다.", "확인");
        }
        else
        {
            EditorUtility.DisplayDialog("오류", "캐시 삭제에 실패했습니다.", "확인");
        }
    }

    [MenuItem("SHTools/Editor/Clean PlayerPrefs", false, 101)]
    [MenuItem("Assets/SHTools/Editor/Clean PlayerPrefs", false, 101)]
    static void SelectToCleanPlayerPrefs()
    {
        SHPlayerPrefs.DeleteAll();
        EditorUtility.DisplayDialog("알림", "PlayerPrefs가 삭제되었습니다.", "확인");
    }

    [MenuItem("SHTools/Editor/Delete PersistentData", false, 102)]
    [MenuItem("Assets/SHTools/Editor/Delete PersistentData", false, 102)]
    static void SelectToDeletePersistentData()
    {
        SHUtils.DeleteDirectory(Application.persistentDataPath);
        EditorUtility.DisplayDialog("알림", "PersistentData가 삭제되었습니다.", "확인");
    }
}
#endif