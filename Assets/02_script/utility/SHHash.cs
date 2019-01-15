using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Security.Cryptography;

public static partial class SHHash
{
    // MD5
    public static string GetMD5ToBuff(byte[] pBuff)
    {
        MD5 pMD5 = new MD5CryptoServiceProvider();
        return BitConverter.ToString(pMD5.ComputeHash(pBuff)).Replace("-", string.Empty); ;
    }

    public static string GetMD5ToFile(string strFilePath)
    {
        if (false == File.Exists(strFilePath))
            return string.Empty;

        using (FileStream pStream = File.OpenRead(strFilePath))
        {
            MD5 pMD5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(pMD5.ComputeHash(pStream)).Replace("-", string.Empty);;
        }
    }

    // SHA1
    public static string GetSHA1ToBuff(byte[] pBuff)
    {
        SHA1 pSHA1 = new SHA1CryptoServiceProvider();
        return BitConverter.ToString(pSHA1.ComputeHash(pBuff)).Replace("-", string.Empty);
    }
    public static string GetSHA1ToFile(string strFilePath)
    {
        if (false == File.Exists(strFilePath))
            return string.Empty;

        using (FileStream pStream = File.OpenRead(strFilePath))
        {
            SHA1 pSHA1 = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(pSHA1.ComputeHash(pStream)).Replace("-", string.Empty);
        }
    }

    // // SHA256
    // public static string GetSHA256ToBuff(byte[] pBuff)
    // {
    //     SHA256 pSHA256 = new SHA256CryptoServiceProvider();
    //     return BitConverter.ToString(pSHA256.ComputeHash(pBuff)).Replace("-", string.Empty);
    // }
    // public static string GetSHA256ToFile(string strFilePath)
    // {
    //     if (false == File.Exists(strFilePath))
    //         return string.Empty;
    // 
    //     using (FileStream pStream = File.OpenRead(strFilePath))
    //     {
    //         SHA256 pSHA256 = new SHA256CryptoServiceProvider();
    //         return BitConverter.ToString(pSHA256.ComputeHash(pStream)).Replace("-", string.Empty);
    //     }
    // }
    // 
    // // SHA384
    // public static string GetSHA384ToBuff(byte[] pBuff)
    // {
    //     SHA384 pSHA384 = new SHA384CryptoServiceProvider();
    //     return BitConverter.ToString(pSHA384.ComputeHash(pBuff)).Replace("-", string.Empty);
    // }
    // public static string GetSHA384ToFile(string strFilePath)
    // {
    //     if (false == File.Exists(strFilePath))
    //         return string.Empty;
    // 
    //     using (FileStream pStream = File.OpenRead(strFilePath))
    //     {
    //         SHA384 pSHA384 = new SHA384CryptoServiceProvider();
    //         return BitConverter.ToString(pSHA384.ComputeHash(pStream)).Replace("-", string.Empty);
    //     }
    // }
    // 
    // // SHA512
    // public static string GetSHA512ToBuff(byte[] pBuff)
    // { 
    //     SHA512 pSHA512 = new SHA512CryptoServiceProvider();
    //     return BitConverter.ToString(pSHA512.ComputeHash(pBuff)).Replace("-", string.Empty);
    // }
    // public static string GetSHA512ToFile(string strFilePath)
    // {
    //     if (false == File.Exists(strFilePath))
    //         return string.Empty;
    // 
    //     using (FileStream pStream = File.OpenRead(strFilePath))
    //     {
    //         SHA512 pSHA512 = new SHA512CryptoServiceProvider();
    //         return BitConverter.ToString(pSHA512.ComputeHash(pStream)).Replace("-", string.Empty);
    //     }
    // }

    // DSA
    public static string GetDSAToBuff(byte[] pBuff)
    {
        DSACryptoServiceProvider pDSA = new DSACryptoServiceProvider();
        return BitConverter.ToString(pDSA.SignData(pBuff)).Replace("-", string.Empty);
    }
    public static string GetDSAToFile(string strFilePath)
    {
        if (false == File.Exists(strFilePath))
            return string.Empty;

        using (FileStream pStream = File.OpenRead(strFilePath))
        {
            DSACryptoServiceProvider pDSA = new DSACryptoServiceProvider();
            return BitConverter.ToString(pDSA.SignData(pStream)).Replace("-", string.Empty);
        }
    }

    // Hash128
    public static Hash128 GetHash128(string strBuff)
    {
        return Hash128.Parse(strBuff);
    }
}