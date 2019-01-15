using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.Collections.Generic;

class SHBuildPostProcess
{
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget eTarget, string strTargetPath)
    {
        if (eTarget == BuildTarget.iOS)
        {
            SHiOSFixupFiles.FixFirebaseCode(strTargetPath);
        }
    }
}