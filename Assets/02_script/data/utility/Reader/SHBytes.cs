using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class SHBytes
{
    public byte[] m_pBytes = null;
    
    public SHBytes() { }
    public SHBytes(string strFileName)
    {
        if (true == string.IsNullOrEmpty(strFileName))
            return;

        strFileName = Path.GetFileNameWithoutExtension(strFileName);

        // 1�� : PersistentData�� Byte �����Ͱ� ������ �װ� �ε��ϵ��� �Ѵ�.
        // 2�� : ������ ��Ű������ �ε��ϵ��� �Ѵ�.

        string strSavePath = string.Format("{0}/{1}.bytes", SHPath.GetPersistentDataBytes(), strFileName);
        if (true == File.Exists(strSavePath))
            m_pBytes = LoadByPersistent(strSavePath);
        else
            m_pBytes = LoadByPackage(strFileName);
    }
    
    public bool CheckBytes()
    {
        return (null != m_pBytes);
    }

    public byte[] GetBytes()
    {
        return m_pBytes;
    }
    
    byte[] LoadByWWW(string strFilePath)
    {
        WWW pWWW = Single.Coroutine.WWWOfSync(new WWW(strFilePath));
        if (true != string.IsNullOrEmpty(pWWW.error))
        {
            Debug.LogError(string.Format("[LSH] Byte(*.bytes)������ �д� �� �����߻�!!(Path:{0}, Error:{1})", strFilePath, pWWW.error));
            return null;
        }
        
        return pWWW.bytes;
    }
    
    byte[] LoadByPersistent(string strFilePath)
    {
        var pBuff = File.ReadAllBytes(strFilePath);
        if (null == pBuff)
        {
            Debug.LogError(string.Format("[LSH] Byte(*.bytes)������ �д� �� �����߻�!!(Path:{0})", strFilePath));
            return null;
        }

        return pBuff;
    }

    byte[] LoadByPackage(string strFileName)
    {
        var pTextAsset = Single.Resources.GetTextAsset(Path.GetFileNameWithoutExtension(strFileName));
        if (null == pTextAsset)
            return null;

        return pTextAsset.bytes;
    }
}