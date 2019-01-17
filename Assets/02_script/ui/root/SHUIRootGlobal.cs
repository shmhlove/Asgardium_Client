using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootGlobal : SHUIRoot
{
    void Awake()
    {
        Single.UI.AddRoot(typeof(SHUIRootGlobal), this);
    }

    public void ShowFade(string state, Action callback)
    {
        // var uiPanel = GetPanel<SHUIPanelFade>();
        // uiPanel.Show()
    }
}
