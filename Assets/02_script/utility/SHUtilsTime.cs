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
    public static DateTime GetDateTimeToString(string strDate, string strFormat)
    {
        return DateTime.ParseExact(strDate, strFormat, System.Globalization.CultureInfo.InstalledUICulture);
    }
    
    public static string GetStringToDateTime(DateTime pTime, string strFormat)
    {
        return pTime.ToString(strFormat, System.Globalization.CultureInfo.InstalledUICulture);
    }
}
