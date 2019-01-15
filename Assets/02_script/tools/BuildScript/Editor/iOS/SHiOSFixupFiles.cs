using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Text.RegularExpressions;

class SHiOSFixupFiles
{
    private const string AutoGenerateMessageBegin = "// BEGIN Auto-generated codes via SHiOSFixupFiles.cs";
    private const string AutoGenerateMessageEnd = "// END";

    protected static string Load(string fullPath)
    {
        string data;
        FileInfo projectFileInfo = new FileInfo(fullPath);
        StreamReader fs = projectFileInfo.OpenText();
        data = fs.ReadToEnd();
        fs.Close();

        return data;
    }

    protected static void Save(string fullPath, string data)
    {
        System.IO.StreamWriter writer = new System.IO.StreamWriter(fullPath, false);
        writer.Write(data);
        writer.Close();
    }

    public static void FixFirebaseCode(string strPath)
    {
        // string strFullPath = Path.Combine(strPath, Path.Combine("Classes", "UnityAppController.mm"));
        // string strData = Load(strFullPath);

        // if (!Regex.IsMatch(strData, "<NcMobileSdkBase/NcMobileSdkBase.h>"))
        // {
        //     strData = Regex.Replace(strData,
        //         "#import\\s+\"UnityAppController.h\"\\s*\n",
        //         "#import \"UnityAppController.h\"\n" +
        //         AutoGenerateMessageBegin + "\n" +
        //         "#import <NcMobileSdkBase/NcMobileSdkBase.h>\n" +
        //         "#import <UserNotifications/UserNotifications.h>\n" +
        //         AutoGenerateMessageEnd + "\n"
        //     );
        // }

        // Save(strFullPath, strData);
    }
}