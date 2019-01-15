#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHEditorResourcesLister : Editor
{
    public static string m_strMsg_1 = "리소스 폴더를 뒤질껍니다!!\n\n완료되면 완료 메시지 출력됩니다.\n\n조금만 기다려 주세요!! 싫으면 취소...ㅠ";
    public static string m_strMsg_2 = "{0}개의 리소스 파일이 리스팅 되었습니다.!!\n\n저장경로 : {1}\n\n리스팅 시간 : {2:F2}sec";

    // 인터페이스 : 리소스 폴더 전체파일 리스팅 : ResourcesInfo.json, AssetBundleInfo.json, DuplicationResourcesList.txt
    [MenuItem("SHTools/Update Resources Info", false, 0)]
    [MenuItem("Assets/SHTools/Update Resources Info", false, 0)]
    static void AllFilsInResourcesFolderWithAssetBundleInfo()
    {
        // 시작팝업
        if (false == ShowDialog("[SHTools] Update Resources Info",
                                SHEditorResourcesLister.m_strMsg_1, 
                                "확인", "취소"))
            return;

        // 알리아싱
        var pStartTime = DateTime.Now;
        var strSaveResourcePath = string.Format("{0}/{1}", SHPath.GetResourceJsonTable(), "JsonResourcesInfo.json");
        //var strSaveBundlePath = string.Format("{0}/{1}", SHPath.GetResourceJsonTable(), "JsonAssetBundleInfo.json");
        var strSaveDuplicationPath = string.Format("{0}/{1}", SHPath.GetRoot(), "DuplicationResourcesList.txt");

        // 리스팅
        var pLister = new SHResourcesLister();
        var iFileCount = pLister.Listing(SHPath.GetResources());
        SHResourcesLister.SaveToResourcesInfo(pLister.m_dicResources, strSaveResourcePath);
        //SHResourcesLister.SaveToAssetBundleInfo(pLister.m_dicAssetBundles, strSaveBundlePath);
        SHResourcesLister.SaveToDuplicationList(pLister.m_dicDuplications, strSaveDuplicationPath);

        // 종료팝업
        if (true == ShowDialog("[SHTools] Update Resources Info",
                    string.Format(SHEditorResourcesLister.m_strMsg_2,
                    iFileCount, strSaveResourcePath, ((DateTime.Now - pStartTime).TotalMilliseconds / 1000.0)), 
                    "파일확인", "닫기"))
            System.Diagnostics.Process.Start(strSaveResourcePath);
    }

    // 유틸 : 팝업
    static bool ShowDialog(string strTitle, string strMessage, string strOkBtn, string strCancleBtn = "")
    {
        if (false == string.IsNullOrEmpty(strCancleBtn))
            return EditorUtility.DisplayDialog(strTitle, strMessage, strOkBtn, strCancleBtn);
        else
            return EditorUtility.DisplayDialog(strTitle, strMessage, strOkBtn);
    }
}
#endif