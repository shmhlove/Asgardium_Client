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
        return Base64Encode(plainText).Replace("=", "").Replace("+", "-").Replace("/", "_");
    }

    public static string Base64ForUrlEncode(byte[] data)
    {
        return Base64Encode(data).Replace("=", "").Replace("+", "-").Replace("/", "_");
    }

    public static string GetJsonStringFromObject<T>(T pObject)
    {
        var pSerializer = new DataContractJsonSerializer(typeof(T));
        var pMemoryStream = new MemoryStream();
        pSerializer.WriteObject(pMemoryStream, pObject);

        var jsonString = Encoding.Default.GetString(pMemoryStream.ToArray());

        pMemoryStream.Close();

        return jsonString;
        
        //string jsonString = (new JavaScriptSerializer()).Serialize((object)dictss);
    }

    public static T GetObjectFromJsonString<T>(string json)
    {
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
        T deserializedObject = (T)ser.ReadObject(ms);
        ms.Close();
        
        return deserializedObject;
    }
}