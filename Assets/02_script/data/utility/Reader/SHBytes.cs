using UnityEngine;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class SHBytes
{
    public byte[] m_pBytes = null;
    
    public SHBytes(string strFileName, Action<SHBytes> pCallback)
    {
        if (true == string.IsNullOrEmpty(strFileName))
        {
            pCallback(this);
            return;
        }

        strFileName = Path.GetFileNameWithoutExtension(strFileName);

        // 1차 : PersistentDataPath에 데이터가 있으면 그걸 로드하도록 한다.
        // 2차 : 없으면 패키지에서 로드하도록 한다.

        string strSavePath = string.Format("{0}/{1}.bytes", SHPath.GetPersistentDataBytes(), strFileName);
        if (true == File.Exists(strSavePath))
        {
            LoadByPersistent(strSavePath, (pBytes) => 
            {
                m_pBytes = pBytes;
                pCallback(this);
            });
        }
        else
        {
            LoadByPackage(strFileName, (pBytes) => 
            {
                m_pBytes = pBytes;
                pCallback(this);
            });
        }
    }
    
    public bool CheckBytes()
    {
        return (null != m_pBytes);
    }

    private void LoadByWWW(string strFilePath, Action<byte[]> pCallback)
    {
        Single.Coroutine.WWW(new UnityWebRequest(strFilePath), (pWWW) => 
        {
            if (true != string.IsNullOrEmpty(pWWW.error))
            {
                Debug.LogWarningFormat("[LSH] Byte(*.bytes)파일을 읽는 중 오류발생!!(Path:{0}, Error:{1})", strFilePath, pWWW.error);
            }
            
            pCallback(pWWW.downloadHandler.data); 
        });
    }
    
    private void LoadByPersistent(string strFilePath, Action<byte[]> pCallback)
    {
        var pBuff = File.ReadAllBytes(strFilePath);
        if (null == pBuff)
        {
            Debug.LogError(string.Format("[LSH] Byte(*.bytes)파일을 읽는 중 오류발생!!(Path:{0})", strFilePath));
        }

        pCallback(pBuff);
    }

    private async void LoadByPackage(string strFileName, Action<byte[]> pCallback)
    {
        var pTextAsset = await Single.Resources.GetTextAsset(Path.GetFileNameWithoutExtension(strFileName));
        if (null == pTextAsset)
        {
            pCallback(null);
        }
        else
        {
            pCallback(pTextAsset.bytes);
        }
    }
}