using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

class SHBuildScript
{
    static string[] SCENES    = FindEnabledEditorScenes();

    #region Android Build
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [MenuItem("SHTools/CI/App Build For Android")]
    static int KOR_AndroidAppBuild()
    { 
        try
        {
            AppBuild(eNationType.Korea, BuildTarget.Android, eServiceMode.Dev, BuildOptions.None);
            return 0;
        }
        catch(Exception e)
        {
            return e.HResult;
        }

        //AppBuild(eNationType.Korea, BuildTarget.Android, eServiceMode.Live, BuildOptions.None);
    }

    // [MenuItem("SHTools/CI/AssetBundles Packing For Android")]
	// static void KOR_AndroidAssetBundlesPacking()
    // {
    //     AssetBundlesPacking(BuildTarget.Android, eBundlePackType.All);
    // }

    // [MenuItem("SHTools/CI/AssetBundles Upload For Android")]
    // static void KOR_AndroidAssetBundlesUpload()
    // {
    //     UploadAssetBundles(BuildTarget.Android);
    // }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #endregion


    #region iOS Build
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [MenuItem("SHTools/CI/App Build For iOS")]
    static int KOR_iOSAppBuild()
    { 
        AppBuild(eNationType.Korea, BuildTarget.iOS, eServiceMode.Dev, BuildOptions.Development);
        //AppBuild(eNationType.Korea, BuildTarget.iOS, eServiceMode.Live, BuildOptions.None);
        return 0;
    }

    // [MenuItem("SHTools/CI/AssetBundles Packing For iOS")]
	// static void KOR_iOSAssetBundlesPacking()
    // {
    //     AssetBundlesPacking(BuildTarget.iOS, eBundlePackType.All);
    // }

    // [MenuItem("SHTools/CI/AssetBundles Upload For iOS")]
    // static void KOR_iOSAssetBundlesUpload()
    // {
    //     UploadAssetBundles(BuildTarget.iOS);
    // }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #endregion
    
    static void AppBuild(eNationType eNation, BuildTarget eTarget, eServiceMode eMode, BuildOptions eOption)
    {
        PreProcessor(eTarget);
		BuildApplication(SCENES, eTarget, eOption);
        PostProcessor(eTarget);
    }
    
    static void AssetBundlesPacking(BuildTarget eTarget, eBundlePackType ePackType)
    {
        PackingAssetBundles(eTarget, ePackType);
        PostProcessor(eTarget);
    }
    
    static void PreProcessor(BuildTarget eTarget)
    {
        // var pConfigFile = await Single.Table.GetTable<JsonClientConfig>();
        // switch (eTarget)
        // {
        //     case BuildTarget.Android:
        //         PlayerSettings.Android.keystoreName = string.Format("{0}/{1}", SHPath.GetRoot(), pConfigFile.AOS_KeyStoreName);
        //         PlayerSettings.Android.keystorePass = pConfigFile.AOS_KeyStorePass;
        //         PlayerSettings.Android.keyaliasName = pConfigFile.AOS_KeyAliasName;
        //         PlayerSettings.Android.keyaliasPass = pConfigFile.AOS_KeyAliasPass;
        //         EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC;
        //         break;
        //     case BuildTarget.iOS:
        //         EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.PVRTC;
        //         break;
        // }
        // PlayerSettings.bundleVersion = pConfigFile.Version;

        switch (eTarget)
        {
            case BuildTarget.Android:
                PlayerSettings.Android.keystoreName = string.Format("{0}/GoogleKeyStore/asgardium.keystore", SHPath.GetRoot());
                PlayerSettings.Android.keystorePass = "lee35235";
                PlayerSettings.Android.keyaliasName = "asgardium";
                PlayerSettings.Android.keyaliasPass = "lee35235";
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC;
                break;
            case BuildTarget.iOS:
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.PVRTC;
                break;
        }
        PlayerSettings.bundleVersion = "1.0.1";
    }
    
    static void PostProcessor(BuildTarget eTarget)
    {
        GameObject.DestroyImmediate(GameObject.Find("SHSingletons(Destroy)"));
        GameObject.DestroyImmediate(GameObject.Find("SHSingletons(DontDestroy)"));
    }

    static void BuildApplication(string[] strScenes, BuildTarget eTarget, BuildOptions eOptions)
    {
        string strBuildName = GetBuildName(eTarget, Single.AppInfo.GetProductName());
        Debug.LogFormat("** [SHBuilder] Build Start({0}) -> {1}", strBuildName, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
        {
            if (BuildTarget.Android == eTarget)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, eTarget);
            else
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, eTarget);

            string strExportPath = string.Format("{0}/{1}/{2}", SHPath.GetBuild(), SHUtils.GetPlatformStringByEnum(eTarget), strBuildName);
            SHUtils.CreateDirectory(strExportPath);

			BuildReport pReport = BuildPipeline.BuildPlayer(strScenes, strExportPath, eTarget, eOptions);
            if (BuildResult.Succeeded != pReport.result)
            {
                throw new Exception("[SHBuilder] BuildPlayer failure: " + pReport.ToString());
            }
        }
        Debug.LogFormat("** [SHBuilder] Build End({0}) -> {1}", strBuildName, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
    }
    
    static void PackingAssetBundles(BuildTarget eTarget, eBundlePackType eType)
    {
        Debug.LogFormat("** [SHBuilder] AssetBundles Packing Start({0}) -> {1}", eTarget, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
        {
            string strExportPath = string.Format("{0}/{1}/{2}", SHPath.GetBuild(), SHUtils.GetPlatformStringByEnum(eTarget), "AssetBundle");
            SHUtils.CreateDirectory(strExportPath);

            BuildPipeline.BuildAssetBundles(strExportPath, BuildAssetBundleOptions.None, eTarget);
        }
        Debug.LogFormat("** [SHBuilder] AssetBundles Packing End({0}) -> {1}", eTarget, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
    }
    
    static void UploadAssetBundles(BuildTarget eTarget)
    {
        // var strExportPath = string.Format("{0}/{1}/{2}", SHPath.GetBuild(), SHHard.GetPlatformStringByEnum(eTarget), "AssetBundle");
        // var strUploadRoot = string.Format("{0}/{1}", SHHard.GetPlatformStringByEnum(eTarget), "AssetBundle");
        // var pFileList = SHUtils.Search(strExportPath, (FileInfo pFile) =>
        // {
        //     var strUploadPath = string.Format("{0}/{1}", 
        //         strUploadRoot, pFile.FullName.Substring(pFile.FullName.IndexOf("AssetBundle") + "AssetBundle".Length + 1)).Replace("\\", "/");
            
        //     Single.Firebase.Storage.Upload(pFile.FullName.Replace("\\", "/"), strUploadPath, (pReply) => 
        //     {
        //         if (pReply.IsSucceed)
        //         {
        //             Debug.LogFormat("SUCCEED!! UploadPath : {0}", strUploadPath);
        //         }
        //         else
        //         {
        //             Debug.LogFormat("FAILED!! UploadPath : {0}", strUploadPath);
        //         }
        //     });
        // });
        
        // Single.Firebase.Storage.Upload(pFileList, strUploadRoot, (pReply) => 
        // {
        //     PostProcessor(eTarget);
        //     EditorApplication.Exit(0);
        // });
    }

    static string GetBuildName(BuildTarget eTarget, string strAppName)
    {
        if (BuildTarget.Android == eTarget)
            return string.Format("{0}.apk", strAppName);
        else
            return "xcode";
    }
    
    static string[] FindEnabledEditorScenes()
    {
        var pScenes   = new List<string>();
        var iMaxCount = EditorBuildSettings.scenes.Length;
        for (int iLoop = 0; iLoop < iMaxCount; ++iLoop)
        {
            var pScene = EditorBuildSettings.scenes[iLoop];
            if (false == pScene.enabled)
                continue;

            pScenes.Add(pScene.path);
        }
        
        return pScenes.ToArray();
    }
}