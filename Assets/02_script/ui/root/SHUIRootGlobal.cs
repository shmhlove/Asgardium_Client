using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootGlobal : SHUIRoot
{
    public void ShowFade(string strPanelName, Action callback)
    {
        GetPanel<SHUIPanelFade>(strPanelName, (pPanel) => 
        {
            pPanel.Show();
        });
    }
}