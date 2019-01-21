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

        var pFile   = new FileStream(strSavePath, FileMode.Create, FileAccess.Write);
        var pWriter = new BinaryWriter(pFile);
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
}
