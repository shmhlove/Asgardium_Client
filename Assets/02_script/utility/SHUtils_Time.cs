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
    public const long MilliPerDay = 8640000;
    public const long MilliPerHour = 360000;
    public const long MilliPerMinute = 60000;
    public const long MilliPerSecond = 1000;

    public static double GetElapsedSecond(DateTime pTime)
    {
        return ((DateTime.Now - pTime).TotalMilliseconds / 1000.0);
    }

    public static DateTime GetDateTimeByString(string strDate, string strFormat)
    {
        return DateTime.ParseExact(strDate, strFormat, System.Globalization.CultureInfo.InstalledUICulture);
    }
    
    public static string GetStringByDateTime(DateTime pTime, string strFormat)
    {
        return pTime.ToString(strFormat, System.Globalization.CultureInfo.InstalledUICulture);
    }

    public static DateTime GetUctTimeByMillisecond(long milliseconds)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds).ToUniversalTime();
    }

    public static long GetTotalMillisecondsForUTC()
    {
        return (long)DateTime.UtcNow.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    }
}
