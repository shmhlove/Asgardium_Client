using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootLogin : SHUIRoot
{
    public void ShowLoginPanel(Action<string, string> pLoginCallback, Action pSignupCallback)
    {
        GetPanel<SHUIPanelLogin>(SHUIConstant.PANEL_LOGIN, (pPanel) => 
        {
            pPanel.Show(pLoginCallback, pSignupCallback);
        });
    }

    public void CloseLoginPanel()
    {
        GetPanel<SHUIPanelLogin>(SHUIConstant.PANEL_LOGIN, (pPanel) => 
        {
            pPanel.Close();
        });
    }

    public void ShowSignupPanel(Action<string, string> pRegistCallback, Action pGobackCallback)
    {
        GetPanel<SHUIPanelSignup>(SHUIConstant.PANEL_SIGNUP, (pPanel) => 
        {
            pPanel.Show(pRegistCallback, pGobackCallback);
        });
    }

    public void CloseSignupPanel()
    {
        GetPanel<SHUIPanelSignup>(SHUIConstant.PANEL_SIGNUP, (pPanel) => 
        {
            pPanel.Close();
        });
    }
}
