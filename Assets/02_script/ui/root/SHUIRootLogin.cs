using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootLogin : SHUIRoot
{
    void Awake()
    {
        Single.UI.AddRoot(typeof(SHUIRootLogin), this);
    }

    public void ShowLoginPanel(Action<string, string> pLoginAction, Action<string, string> pSigninAction, Action pCloseAction)
    {
        //SetEnableAllPanels(false);

        var panel = GetPanel<SHUIPanelLogin>();
        panel.SetActive(true);
        panel.OnLoginAction = pLoginAction;
        panel.OnSigninAction = pSigninAction;
        panel.OnloseAction = pCloseAction;
    }

    public void CloseLoginPanel()
    {
        var panel = GetPanel<SHUIPanelLogin>();
        panel.SetActive(false);
        panel.OnLoginAction = null;
        panel.OnSigninAction = null;
        panel.OnloseAction = null;
    }
}
