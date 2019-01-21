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
}
