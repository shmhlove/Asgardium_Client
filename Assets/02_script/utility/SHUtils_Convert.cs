using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Json;

using LitJson;

public static partial class SHUtils
{
    public static string Base64Encode(byte[] data)
    {
        return System.Convert.ToBase64String(data);
    }

    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return Base64Encode(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static string Base64ForUrlEncode(string plainText)
    {
        return Base64Encode(plainText)
            .Replace("=", "")   // Remove any trailing '='s
            .Replace("+", "-")  // 62nd char of encoding
            .Replace("/", "_"); // 63rd char of encoding
    }

    public static string Base64ForUrlEncode(byte[] data)
    {
        return Base64Encode(data)
            .Replace("=", "")   // Remove any trailing '='s
            .Replace("+", "-")  // 62nd char of encoding
            .Replace("/", "_"); // 63rd char of encoding
    }

    public static string GetJsonStringFromObject<T>(T pObject)
    {
        return JsonMapper.ToJson(pObject);

        //var pSerializer = new DataContractJsonSerializer(typeof(T));
        //var pMemoryStream = new MemoryStream();
        //pSerializer.WriteObject(pMemoryStream, pObject);

        //var pSerializeJsonString = Encoding.Default.GetString(pMemoryStream.ToArray());

        //pMemoryStream.Close();

        //return pSerializeJsonString;
    }

    public static T GetObjectFromJsonString<T>(string json)
    {
        return JsonMapper.ToObject<T>(json);

        //var pMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
        //T pDeserializedObject = (T)ser.ReadObject(pMemoryStream);

        //pMemoryStream.Close();

        //return pDeserializedObject;
    }
}