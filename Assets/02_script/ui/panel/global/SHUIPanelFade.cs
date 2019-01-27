using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelFade : SHUIPanel
{
    public override void OnAfterShow(params object[] pArgs)
    {
        if ((null == pArgs) || (1 > pArgs.Length))
            return;
        
        var pCallback = ((Action)pArgs[0]);
        if (null == pCallback)
            return;

        pCallback();
    }

    public override void OnAfterClose(params object[] pArgs)
    {
        if ((null == pArgs) || (1 > pArgs.Length))
            return;

        var pCallback = ((Action)pArgs[0]);
        if (null == pCallback)
            return;

        pCallback();
    }
}
