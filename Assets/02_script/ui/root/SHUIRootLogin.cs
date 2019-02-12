using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootLogin : SHUIRoot
{
    public void ShowLoginPanel(params object[] pArgs)
    {
        GetPanel<SHUIPanelLogin>(SHUIConstant.PANEL_LOGIN, (pPanel) => 
        {
            pPanel.Show(pArgs);
        });
    }

    public void CloseLoginPanel()
    {
        GetPanel<SHUIPanelLogin>(SHUIConstant.PANEL_LOGIN, (pPanel) => 
        {
            pPanel.Close();
        });
    }

    public void ShowSignupPanel(params object[] pArgs)
    {
        GetPanel<SHUIPanelSignup>(SHUIConstant.PANEL_SIGNUP, (pPanel) => 
        {
            pPanel.Show(pArgs);
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
