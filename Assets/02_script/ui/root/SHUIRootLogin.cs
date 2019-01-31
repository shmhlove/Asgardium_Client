using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootLogin : SHUIRoot
{
    public void ShowLoginPanel(
        Action<string, string> pLoginCallback, 
        Action<string, string> pSignupCallback,
        string strEmail)
    {
        GetPanel<SHUIPanelLogin>(SHUIConstant.PANEL_LOGIN, (pPanel) => 
        {
            pPanel.Show(pLoginCallback, pSignupCallback, strEmail);
        });
    }

    public void CloseLoginPanel()
    {
        GetPanel<SHUIPanelLogin>(SHUIConstant.PANEL_LOGIN, (pPanel) => 
        {
            pPanel.Close();
        });
    }

    public void ShowSignupPanel(
        Action<string, string, string> pRegistCallback, 
        Action<string, string, string> pGobackCallback,
        string strEmail,
        string strName)
    {
        GetPanel<SHUIPanelSignup>(SHUIConstant.PANEL_SIGNUP, (pPanel) => 
        {
            pPanel.Show(pRegistCallback, pGobackCallback, strEmail, strName);
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
