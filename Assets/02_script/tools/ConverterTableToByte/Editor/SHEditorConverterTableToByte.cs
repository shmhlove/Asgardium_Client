#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SHConverterTableToByte))]
public class SHEditorConverterTableToByte : Editor
{
    [MenuItem("SHTools/Converter of Table To Byte", false, 1)]
    [MenuItem("Assets/SHTools/Converter of Table To Byte", false, 1)]
    static void SelectToMenu()
    {
        EditorUtility.DisplayDialog("[SHTools] Converter Table To Byte",
            "Table클래스에서 Byte파일로 변환/생성 합니다!!", "확인");

        DateTime pStartTime = DateTime.Now;
        SHConverterTableToByte pTable = new SHConverterTableToByte();
        pTable.RunEditorToConverter();

        EditorUtility.DisplayDialog("[SHTools] Converter Table To Byte",
            string.Format("Bytes파일이 변환/생성 되었습니다.!!\n시간 : {0:F2}sec", 
            ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0f)), "확인");
    }
}
#endif