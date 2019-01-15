using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHResourcesLister
{
    // 멤버 : 리스팅된 리소스 리스트
    public Dictionary<string, SHResourcesInfo> m_dicResources = new Dictionary<string, SHResourcesInfo>();

    // 멤버 : 리스팅된 번들 리스트
    //public Dictionary<string, AssetBundleInfo>      m_dicAssetBundles   = new Dictionary<string, AssetBundleInfo>();

    // 멤버 : 중복파일 리스트
    public Dictionary<string, List<string>> m_dicDuplications = new Dictionary<string, List<string>>();

    // 인터페이스 : 초기화
    public void Initialize()
    {
        m_dicResources.Clear();
        //m_dicAssetBundles.Clear();
        m_dicDuplications.Clear();
    }

    // 인터페이스 : strSearchPath 내에 있는 모든 파일을 ResourceTableData형식에 맞게 Json으로 리스팅
    public int Listing(string strSearchPath)
    {
        SHUtils.Search(strSearchPath, (pFileInfo) =>
        {
            var pResourceInfo = MakeResourceInfo(pFileInfo);
            if (null == pResourceInfo)
                return;

            var strDuplicationFile = CheckToDuplicationFile(m_dicResources, pResourceInfo.m_strFileName);
            if (false == string.IsNullOrEmpty(strDuplicationFile))
            {
                string strFirst = string.Format("{0}", strDuplicationFile);
                string strSecond = string.Format("{0}", pResourceInfo.m_strPath);

#if UNITY_EDITOR
                EditorUtility.DisplayDialog("[SHTools] Resources Listing",
                    string.Format("중복 파일발견!! 파일명은 중복되면 안됩니다!!\r\n1번 파일을 리스팅 하겠습니다.\r\n1번 : {0}\r\n2번 {1}",
                    strFirst, strSecond), "확인");
#endif

                if (false == m_dicDuplications.ContainsKey(pResourceInfo.m_strFileName))
                {
                    m_dicDuplications[pResourceInfo.m_strFileName] = new List<string>();
                    m_dicDuplications[pResourceInfo.m_strFileName].Add(strFirst);
                }

                m_dicDuplications[pResourceInfo.m_strFileName].Add(strSecond);
                return;
            }

            AddResourceInfo(pResourceInfo);
            AddAssetBundleInfo(pResourceInfo);
        });

        return m_dicResources.Count;
    }

    // 인터페이스 : 리소스 리스트를 Json형태로 쓰기
    public static void SaveToResourcesInfo(Dictionary<string, SHResourcesInfo> dicTable, string strSaveFilePath)
    {
        if (0 == dicTable.Count)
            return;

        var pResourcesJsonData = new JsonData();
        SHUtils.ForToDic(dicTable, (pKey, pValue) =>
        {
            pResourcesJsonData.Add(MakeResourceJsonData(pValue));
        });
        
        var pJsonWriter = new JsonWriter();
        pJsonWriter.PrettyPrint = true;
        JsonMapper.ToJson(pResourcesJsonData, pJsonWriter);

        SHUtils.SaveFile(pJsonWriter.ToString(), strSaveFilePath);
    }

    // 인터페이스 : 번들 리스트를 Json형태로 번들정보파일포맷으로 쓰기
    //public static void SaveToAssetBundleInfo(Dictionary<string, AssetBundleInfo> dicTable, string strSaveFilePath)
    //{
    //    if (0 == dicTable.Count)
    //        return;

    //    var pBundleListJsonData = new JsonData();
    //    SHUtils.ForToDic(dicTable, (pKey, pValue) =>
    //    {
    //        var pBundleJsonData = new JsonData();
    //        pBundleJsonData["s_BundleName"] = pValue.m_strBundleName;
    //        pBundleJsonData["s_BundleSize"] = pValue.m_lBundleSize;
    //        pBundleJsonData["s_BundleHash"] = pValue.m_pHash128.ToString();

    //        SHUtils.ForToDic(pValue.m_dicResources, (pResKey, pResValue) =>
    //        {
    //            pBundleJsonData["p_Resources"].Add(MakeResourceJsonData(pResValue));
    //        });

    //        pBundleListJsonData.Add(pBundleJsonData);
    //    });
    
    //    var pJsonWriter = new JsonWriter();
    //    pJsonWriter.PrettyPrint = true;
    //    JsonMapper.ToJson(pBundleListJsonData, pJsonWriter);

    //    SHUtils.SaveFile(pJsonWriter.ToString(), strSaveFilePath);
    //}

    // 인터페이스 : 중복파일 리스트 내보내기
    public static void SaveToDuplicationList(Dictionary<string, List<string>> dicDuplications, string strSaveFilePath)
    {
        if (0 == dicDuplications.Count)
            return;

        var pDuplicationJsonData = new JsonData();
        SHUtils.ForToDic(dicDuplications, (pKey, pValue) =>
        {
            var pFilesJsonData = new JsonData();
            SHUtils.ForToList(pValue, (strPath) =>
            {
                pFilesJsonData.Add(strPath);
            });
            pDuplicationJsonData[string.Format("FileName_{0}", pKey)] = pFilesJsonData;
        });
        
        var pJsonWriter = new JsonWriter();
        pJsonWriter.PrettyPrint = true;
        JsonMapper.ToJson(pDuplicationJsonData, pJsonWriter);

        SHUtils.SaveFile(pJsonWriter.ToString(), strSaveFilePath);
    }

    // 인터페이스 : 파일정보 Json 포맷으로 만들어주기
    public static JsonData MakeResourceJsonData(SHResourcesInfo pInfo)
    {
        JsonData pJsonData = new JsonData();

        pJsonData["s_Name"]      = pInfo.m_strName;
        pJsonData["s_FileName"]  = pInfo.m_strFileName;
        pJsonData["s_Extension"] = pInfo.m_strExtension;
        pJsonData["s_Size"]      = pInfo.m_strSize;
        //pJsonData["s_LastWriteTime"] = pInfo.m_strLastWriteTime;
        pJsonData["s_Hash"]      = pInfo.m_strHash;
        pJsonData["s_Path"]      = pInfo.m_strPath;

        return pJsonData;
    }
    
    // 유틸 : 리소스 리스트 추가
    void AddResourceInfo(SHResourcesInfo pInfo)
    {
        if (null == pInfo)
            return;

        m_dicResources[pInfo.m_strFileName] = pInfo;
    }

    // 유틸 : 번들 리스트 추가
    // 1. 리소스의 최상위 폴더이름을 번들이름으로 하여 등록시킴.
    // 2. 프리팹을 제외한 모든 리소스를 번들 리스트로 등록시킴.
    void AddAssetBundleInfo(SHResourcesInfo pInfo)
    {
        //if (null == pInfo)
        //    return;

        //if (true == CheckFilteringToAssetBundleInfo(pInfo))
        //    return;

        //// 번들이름 만들기
        //string strBundleName    = "Root";
        //string[] strSplitPath   = pInfo.m_strPath.Split(new char[] { '/' });
        //if (1 < strSplitPath.Length)
        //    strBundleName = strSplitPath[0];

        //// 번들정보 생성하기
        //if (false == m_dicAssetBundles.ContainsKey(strBundleName))
        //{
        //    var pBundleInfo = new AssetBundleInfo();
        //    pBundleInfo.m_strBundleName = strBundleName;
        //    m_dicAssetBundles.Add(strBundleName, pBundleInfo);
        //}

        //m_dicAssetBundles[strBundleName].AddResourceInfo(pInfo);
    }

    // 유틸 : 번들로 묶지 않을 파일에 대한 필터링
    bool CheckFilteringToAssetBundleInfo(SHResourcesInfo pInfo)
    {
        // 프리팹 파일 필터링
        if (".prefab" == pInfo.m_strExtension)
            return true;

        // 테이블 파일 필터링
        if (".bytes" == pInfo.m_strExtension)
            return true;
        
        return false;
    }

    // 유틸 : 파일로 부터 정보얻어서 테이블 데이터 객체만들기
    SHResourcesInfo MakeResourceInfo(FileInfo pFile)
    {      
        // 예외처리 : 리스팅에서 제외할 파일
        if (true == CheckExceptionFile(pFile))
            return null;
     
        // 알리아싱
        string strRoot              = "Resources";
        string strFullName          = pFile.FullName.Substring(pFile.FullName.IndexOf(strRoot) + strRoot.Length + 1).Replace ("\\", "/");
        string strExtension         = Path.GetExtension(strFullName);

        // 기록
        var pInfo                   = new SHResourcesInfo();
        pInfo.m_strName             = Path.GetFileNameWithoutExtension(strFullName);
        pInfo.m_strFileName         = Path.GetFileName(strFullName);
        pInfo.m_strExtension        = strExtension;
        pInfo.m_strSize             = pFile.Length.ToString();
        //pInfo.m_strLastWriteTime    = pFile.LastWriteTime.ToString("yyyy-MM-dd-HH:mm:ss.fff");
        pInfo.m_strHash             = SHHash.GetMD5ToFile(pFile.FullName);
        pInfo.m_strPath             = strFullName.Substring(0, strFullName.Length - strExtension.Length);
        
        return pInfo;
    }
    
    // 유틸 : 예외파일 체크
    bool CheckExceptionFile(FileInfo pFile)
    {
        if (null == pFile)
            return true;

        switch(Path.GetExtension(pFile.FullName).ToLower())
        {
            case ".meta":       return true;
            case ".shader":     return true;
        }

        return false;
    }

    // 유틸 : 이름중복체크
    string CheckToDuplicationFile(Dictionary<string, SHResourcesInfo> dicFiles, string strFileName)
    {
        foreach (var kvp in dicFiles)
        {
            if (kvp.Key == strFileName)
                return kvp.Value.m_strPath;
        }
        return null;
    }
}