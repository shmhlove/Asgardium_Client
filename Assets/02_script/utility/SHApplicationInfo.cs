using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using LitJson;

using DicResourceLoadInfo = System.Collections.Generic.Dictionary<eSceneType, System.Collections.Generic.List<string>>;

public partial class SHApplicationInfo : SHSingleton<SHApplicationInfo>
{
    [Header("Debug")]
    // 기타(디버그) : FPS 출력용 델타타임
    [ReadOnlyField]
    [SerializeField] private float              m_fDeltaTime        = 0.0f;

    // 기타(디버그) : 실시간 로드 리소스 리스트
    [HideInInspector] private DicResourceLoadInfo m_pDicResourceLoadInfo   = new DicResourceLoadInfo();

    public override void OnInitialize()
    {
        SetDontDestroy();

        // 어플리케이션 정보설정
        SetApplicationInfo();

        // 디바이스 정보 로그
        PrintDeviceInfo();
    }
    
    void OnApplicationPause(bool bIsPause)
    {
    }
    
    void OnApplicationFocus(bool bIsFocus)
    {
    }
    
    public void Update()
    {
        m_fDeltaTime += (Time.deltaTime - m_fDeltaTime) * 0.1f;

        if (true == Input.GetKeyDown(KeyCode.Escape))
        {
            SHUtils.GameQuit();
        }
    }
    
    void OnGUI()
    {
        DrawAppInformation();
    }
    
    async void SetApplicationInfo()
    {
        var pTable = await Single.Table.GetTable<JsonClientConfig>();
        
        SetFrameRate(pTable.FrameRate);
        SetSleepMode();
        SetOrientation();
        SetCrittercism();
        
        // UnityEngine.Debug.LogFormat("[LSH] ProcessID : {0}", GetProcessID());
        // UnityEngine.Debug.LogFormat("[LSH] DebugPort : {0}", GetDebugPort());
    }
    
    public bool IsEditorMode()
    {
        return ((GetRuntimePlatform() == RuntimePlatform.WindowsEditor) ||
                (GetRuntimePlatform() == RuntimePlatform.OSXEditor));
    }
    
    public RuntimePlatform GetRuntimePlatform()
    {
        return Application.platform;
    }
    
    public string GetProductName()
    {
        //Application.bundleIdentifier.Split('.')[2];
        return Application.productName;
    }
    
    public SystemLanguage GetSystemLanguage()
    {
        return Application.systemLanguage;
    }
    
    public int GetProcessID()
    {
        return Process.GetCurrentProcess().Id;
    }
    
    public int GetDebugPort()
    {
        return 56000 + (GetProcessID() % 1000);
    }
    
    void SetFrameRate(int iFrame)
    {
        Application.targetFrameRate = iFrame;
    }
    
    void SetSleepMode()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    
    void SetOrientation()
    {
        //Screen.orientation = ScreenOrientation.AutoRotation;
        //Screen.autorotateToPortrait = false;
        //Screen.autorotateToPortraitUpsideDown = true;
        //Screen.autorotateToLandscapeRight = false;
        //Screen.autorotateToLandscapeLeft = false;
    }
    
    void SetCrittercism()
    {
        // #if UNITY_ANDROID
        //         UnityEngine.Debug.LogFormat("[LSH] Crittercism.DidCrashOnLastLoad = {0}", CrittercismAndroid.DidCrashOnLastLoad());
        //         CrittercismAndroid.Init("20fb64bf760d44589b6aefeb6bcb220700555300");
        //         CrittercismAndroid.SetLogUnhandledExceptionAsCrash(true);
        // #elif UNITY_IPHONE || UNITY_IOS
        //         UnityEngine.Debug.LogFormat("[LSH] Crittercism.DidCrashOnLastLoad = {0}", CrittercismIOS.DidCrashOnLastLoad());
        //         CrittercismIOS.Init("7d02af2372694b93b84d75a999dd7dd400555300");
        //         CrittercismIOS.SetLogUnhandledExceptionAsCrash(true);
        // #endif
    }

    // UUID
    public string GetDeviceID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    public string GetDeviceName()
    {
        return SystemInfo.deviceName;
    }

    public string GetDeviceModel()
    {
        return SystemInfo.deviceModel;
    }

    public int GetSystemMemorySize()
    {
        return SystemInfo.systemMemorySize;
    }

    public int GetGraphiceMemorySize()
    {
        return SystemInfo.graphicsMemorySize;
    }

    public int GetMaxTextureSize()
    {
        return SystemInfo.maxTextureSize;
    }

    #region 디버그 정보 처리
    public int GetRatioW(int iValue)
    {
        return (int)(iValue * (Screen.width / 720.0f));
    }

    public int GetRatioH(int iValue)
    {
        return (int)(iValue * (Screen.height / 1280.0f));
    }

    // 디버그 : 실시간 로드 리소스 리스트 파일로 출력
    [FuncButton]
    public void SaveLoadResourceList()
    {
        var pJsonData = new JsonData();
        
        foreach (var kvp in m_pDicResourceLoadInfo)
        {
            foreach (var pValue in kvp.Value)
            {
                pJsonData[string.Format("Scene : {0}", kvp.Key)].Add(pValue);
            }
        }
        
        var pJsonWriter = new JsonWriter();
        pJsonWriter.PrettyPrint = true;
        JsonMapper.ToJson(pJsonData, pJsonWriter);

        string strSavePath = string.Format("{0}/{1}.json", SHPath.GetAssets(), "RealTimeResourceLoadInfo");
        SHUtils.SaveFile(pJsonWriter.ToString(), strSavePath);

        Process.Start(strSavePath);
    }
    [FuncButton]
    public void ClearLoadResourceList()
    {
        m_pDicResourceLoadInfo.Clear();
    }
    
    // 디버그 : 앱 정보 출력
    async void DrawAppInformation()
    {
        var pTable = await Single.Table.GetTable<JsonClientConfig>();

        GUIStyle pStyle = new GUIStyle(GUI.skin.box)
        {
            fontSize = GetRatioW(20)
        };

        // Left Bottom
        GUI.Box(new Rect(5, (Screen.height - GetRatioH(35)), GetRatioW(250), GetRatioH(30)),
            string.Format("{0} : {1} Scene", GetRuntimePlatform(), Single.Scene.GetActiveScene()), pStyle);
        
        // Center Bottom
        GUI.Box(new Rect((Screen.width * 0.5f) - (GetRatioW(120) * 0.5f), (Screen.height - GetRatioH(35)), GetRatioW(120), GetRatioH(30)),
            string.Format("v{0}", pTable.Version), pStyle);
        
        // Right Bottom
        GUI.Box(new Rect(Screen.width - GetRatioW(255), (Screen.height - GetRatioH(35)), GetRatioW(250), GetRatioH(30)),
            string.Format("ServiceMode : {0}", pTable.ServiceMode), pStyle);
    }
    
    // 디버그 : 실시간 로드 리소스 리스트
    public void SetLoadResource(string strInfo)
    {
#if !UNITY_EDITOR
        return;
#endif
        if (false == Application.isPlaying)
            return;
            
        if (false == m_pDicResourceLoadInfo.ContainsKey(Single.Scene.GetActiveScene()))
            m_pDicResourceLoadInfo.Add(Single.Scene.GetActiveScene(), new List<string>());

        //// 콜스택 남기기
        //strInfo += string.Format("\n< CallStack >\n{0}", SHUtils.GetCallStack());

        m_pDicResourceLoadInfo[Single.Scene.GetActiveScene()].Add(strInfo);
    }

    [FuncButton]
    public void PrintDeviceInfo()
    {
        string strBuff = "<color=yellow>==================Device Information==================</color>\n";
        {
            strBuff += string.Format("Screen.currentResolution.height : {0}\n", Screen.currentResolution.height);
            strBuff += string.Format("Screen.currentResolution.width : {0}\n", Screen.currentResolution.width);
            strBuff += string.Format("Screen.dpi : {0}\n", Screen.dpi);
            strBuff += string.Format("Screen.fullScreen : {0}\n", Screen.fullScreen);
            strBuff += string.Format("Screen.sleepTimeout : {0}\n", Screen.sleepTimeout);
            strBuff += string.Format("Screen.width : {0}\n", Screen.width);
            strBuff += string.Format("Screen.height : {0}\n", Screen.height);
            strBuff += string.Format("deviceModel : {0}\n", SystemInfo.deviceModel);
            strBuff += string.Format("deviceName : {0}\n", SystemInfo.deviceName);
            strBuff += string.Format("deviceType : {0}\n", SystemInfo.deviceType);
#if UNITY_EDITOR
            strBuff += string.Format("deviceUniqueIdentifier : {0}\n", SystemInfo.deviceUniqueIdentifier);
#endif
            strBuff += string.Format("graphicsDeviceID : {0}\n", SystemInfo.graphicsDeviceID);
            strBuff += string.Format("graphicsDeviceName : {0}\n", SystemInfo.graphicsDeviceName);
            strBuff += string.Format("graphicsDeviceType : {0}\n", SystemInfo.graphicsDeviceType);
            strBuff += string.Format("graphicsDeviceVendor : {0}\n", SystemInfo.graphicsDeviceVendor);
            strBuff += string.Format("graphicsDeviceVendorID : {0}\n", SystemInfo.graphicsDeviceVendorID);
            strBuff += string.Format("graphicsDeviceVersion : {0}\n", SystemInfo.graphicsDeviceVersion);
            strBuff += string.Format("graphicsMemorySize : {0}\n", SystemInfo.graphicsMemorySize);
            strBuff += string.Format("graphicsMultiThreaded : {0}\n", SystemInfo.graphicsMultiThreaded);
            strBuff += string.Format("graphicsShaderLevel : {0}\n", SystemInfo.graphicsShaderLevel);
            strBuff += string.Format("maxTextureSize : {0}\n", SystemInfo.maxTextureSize);
            strBuff += string.Format("npotSupport : {0}\n", SystemInfo.npotSupport);
            strBuff += string.Format("operatingSystem : {0}\n", SystemInfo.operatingSystem);
            strBuff += string.Format("processorCount : {0}\n", SystemInfo.processorCount);
            strBuff += string.Format("processorFrequency : {0}\n", SystemInfo.processorFrequency);
            strBuff += string.Format("processorType : {0}\n", SystemInfo.processorType);
            strBuff += string.Format("supportedRenderTargetCount : {0}\n", SystemInfo.supportedRenderTargetCount);
            strBuff += string.Format("supports3DTextures : {0}\n", SystemInfo.supports3DTextures);
            strBuff += string.Format("supportsAccelerometer : {0}\n", SystemInfo.supportsAccelerometer);
            strBuff += string.Format("supportsComputeShaders : {0}\n", SystemInfo.supportsComputeShaders);
            strBuff += string.Format("supportsGyroscope : {0}\n", SystemInfo.supportsGyroscope);
            strBuff += string.Format("supportsImageEffects : {0}\n", SystemInfo.supportsImageEffects);
            strBuff += string.Format("supportsInstancing : {0}\n", SystemInfo.supportsInstancing);
            strBuff += string.Format("supportsLocationService : {0}\n", SystemInfo.supportsLocationService);
            strBuff += string.Format("supportsRawShadowDepthSampling : {0}\n", SystemInfo.supportsRawShadowDepthSampling);
            strBuff += string.Format("supportsRenderToCubemap : {0}\n", SystemInfo.supportsRenderToCubemap);
            strBuff += string.Format("supportsShadows : {0}\n", SystemInfo.supportsShadows);
            strBuff += string.Format("supportsSparseTextures : {0}\n", SystemInfo.supportsSparseTextures);
            strBuff += string.Format("supportsVibration : {0}\n", SystemInfo.supportsVibration);
            strBuff += string.Format("systemMemorySize : {0}\n", SystemInfo.systemMemorySize);
        }

        UnityEngine.Debug.Log(strBuff);
    }
#endregion
}
