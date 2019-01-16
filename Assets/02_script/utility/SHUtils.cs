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
    #region 형 변환 관련 ( Enum.Parse 엄청느립니다. 가급적 사용금지!!! )
    // String을 Enum으로
    public static T GetStringToEnum<T>(string strEnum, string strErrorLog = "")
    {
        if ((true == string.IsNullOrEmpty(strEnum)) || 
            (false == Enum.IsDefined(typeof(T), strEnum)))
        {
            UnityEngine.Debug.LogError("[LSH] " + string.Format("{0} ( Enum:{1} )", strErrorLog, strEnum));
            return default(T);
        }

        return (T)Enum.Parse(typeof(T), strEnum);
    }
    // string을 DateTime으로
    public static DateTime GetDateTimeToString(string strDate, string strFormat)
    {
        return DateTime.ParseExact(strDate, strFormat, System.Globalization.CultureInfo.InstalledUICulture);
    }
    // DateTime을 string으로
    public static string GetStringToDateTime(DateTime pTime, string strFormat)
    {
        return pTime.ToString(strFormat, System.Globalization.CultureInfo.InstalledUICulture);
    }
    #endregion


    #region 반복문 관련
    // For Enum
    public static void ForToEnum<T>(Action<T> pCallback)
    {
        var pEnumerator = Enum.GetValues(typeof(T)).GetEnumerator();
        while (pEnumerator.MoveNext())
        {
            pCallback((T)pEnumerator.Current);
        }
    }
    #endregion


    #region 유니티 에디터 관련
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
    #endregion


    #region 디렉토리 관련
    public static List<FileInfo> Search(string strPath, Action<FileInfo> pCallback)
    {
        var pFileList = new List<FileInfo>();
#if UNITY_EDITOR
        DirectoryInfo pDirInfo = new DirectoryInfo(strPath);
        SearchFiles(pDirInfo, pFileList, pCallback);
        SearchDirs(pDirInfo, pFileList, pCallback);
#endif
        return pFileList;
    }
    static void SearchDirs(DirectoryInfo pDirInfo, List<FileInfo> pOutFileList, Action<FileInfo> pCallback)
    {
#if UNITY_EDITOR
        if (false == pDirInfo.Exists)
            return;
        
        foreach (var pDir in pDirInfo.GetDirectories())
        {
            SearchFiles(pDir, pOutFileList, pCallback);
            SearchDirs(pDir, pOutFileList, pCallback);
        }
#endif
    }
    static void SearchFiles(DirectoryInfo pDirInfo, List<FileInfo> pOutFileList, Action<FileInfo> pCallback)
    {
#if UNITY_EDITOR
        foreach (var pFile in pDirInfo.GetFiles())
        {
            pCallback(pFile);
            pOutFileList.Add(pFile);
        }
#endif
    }
    public static void CreateDirectory(string strPath)
    {
        if (false == string.IsNullOrEmpty(Path.GetExtension(strPath)))
            strPath = Path.GetDirectoryName(strPath);

        DirectoryInfo pDirectoryInfo = new DirectoryInfo(strPath);
        if (true == pDirectoryInfo.Exists)
            return;

        pDirectoryInfo.Create();
    }
    public static void DeleteDirectory(string strPath)
    {
        DirectoryInfo pDirInfo = new DirectoryInfo(strPath);
        if (false == pDirInfo.Exists)
            return;

        FileInfo[] pFiles = pDirInfo.GetFiles("*.*", SearchOption.AllDirectories);
        foreach (var pFile in pFiles)
        {
            if (false == pFile.Exists)
                return;

            pFile.Attributes = FileAttributes.Normal;
        }

        Directory.Delete(strPath, true);
    }
	public static bool IsExistsDirectory(string strPath)
	{
		if (false == string.IsNullOrEmpty(Path.GetExtension(strPath)))
			strPath = Path.GetDirectoryName(strPath);
		
		DirectoryInfo pDirectoryInfo = new DirectoryInfo(strPath);
		return pDirectoryInfo.Exists;
	}
    #endregion


    #region 파일 관련
    public static void SaveFile(string strBuff, string strSavePath)
    {
        SHUtils.CreateDirectory(strSavePath);

        var pFile    = new FileStream(strSavePath, FileMode.Create, FileAccess.Write);
        var pWriter  = new StreamWriter(pFile);
        pWriter.WriteLine(strBuff);
        pWriter.Close();
        pFile.Close();

// ICloud에 백업 안되게 파일에 대해 SetNoBackupFlag를 해주자!!
#if UNITY_IPHONE
        UnityEngine.iOS.Device.SetNoBackupFlag(strSavePath);
#endif

        UnityEngine.Debug.Log(string.Format("[LSH] {0} File 저장", strSavePath));
    }
    public static string ReadFile(string strReadPath)
    {
        var pFile   = new FileStream(strReadPath, FileMode.Open, FileAccess.Read);
        var pReader = new StreamReader(pFile);
        string strBuff = pReader.ReadToEnd();
        pReader.Close();
        pFile.Close();

        return strBuff;
    }
    public static void SaveByte(byte[] pBytes, string strSavePath)
    {
        SHUtils.CreateDirectory(strSavePath);

        var pFile       = new FileStream(strSavePath, FileMode.Create, FileAccess.Write);
        var pWriter     = new BinaryWriter(pFile);
        pWriter.Write(pBytes);
        pWriter.Close();
        pFile.Close();

// ICloud에 백업 안되게 파일에 대해 SetNoBackupFlag를 해주자!!
#if UNITY_IPHONE
        UnityEngine.iOS.Device.SetNoBackupFlag(strSavePath);
#endif

        UnityEngine.Debug.Log(string.Format("[LSH] {0} Byte 저장", strSavePath));
    }
    public static byte[] ReadByte(string strReadPath)
    {
        var pFile = new FileStream(strReadPath, FileMode.Open, FileAccess.Read);
        var pReader = new BinaryReader(pFile);

        var pBytes = new byte[pFile.Length];
        pReader.Read(pBytes, 0, (int)pFile.Length);

        pReader.Close();
        pFile.Close();

        return pBytes;
    }
    public static void DeleteFile(string strFilePath)
    {
        if (false == File.Exists(strFilePath))
            return;

        FileInfo pFile = new FileInfo(strFilePath);
        pFile.Attributes = FileAttributes.Normal;
        File.Delete(strFilePath);
        
    }
    public static void CopyFile(string strSource, string strDest)
    {
        if (false == File.Exists(strSource))
            return;

        SHUtils.CreateDirectory(strDest);

        File.Copy(strSource, strDest, true);

// ICloud에 백업 안되게 파일에 대해 SetNoBackupFlag를 해주자!!
#if UNITY_IPHONE
        UnityEngine.iOS.Device.SetNoBackupFlag(strDest);
#endif
    }
    #endregion


    #region 탐색기 열기
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
    #endregion


    #region 기타
    // WWW.error 메시지로 에러코드 얻기
    public static int GetWWWErrorCode(string strErrorMsg)
    {
        if (true == string.IsNullOrEmpty(strErrorMsg))
            return 0;

        int      iErrorCode = 0;
        string[] strSplit   = strErrorMsg.Split(new char[]{ ' ' });
        int.TryParse(strSplit[0], out iErrorCode);
        return iErrorCode;
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
    #endregion
}