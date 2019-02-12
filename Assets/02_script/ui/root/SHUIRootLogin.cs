using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootLogin : SHUIRoot
{
    public async void ShowLoginPanel(params object[] pArgs)
    {
        var pPanel = await GetPanel<SHUIPanelLogin>(SHUIConstant.PANEL_LOGIN);
        pPanel.Show(pArgs);
    }

    public async void CloseLoginPanel()
    {
        var pPanel = await GetPanel<SHUIPanelLogin>(SHUIConstant.PANEL_LOGIN);
        pPanel.Close();
    }

    public async void ShowSignupPanel(params object[] pArgs)
    {
        var pPanel = await GetPanel<SHUIPanelSignup>(SHUIConstant.PANEL_SIGNUP);
        pPanel.Show(pArgs);
    }

    public async void CloseSignupPanel()
    {
        var pPanel = await GetPanel<SHUIPanelSignup>(SHUIConstant.PANEL_SIGNUP);
        pPanel.Close();
    }
}
