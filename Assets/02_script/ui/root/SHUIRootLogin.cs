using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootLogin : SHUIRoot
{
    public void ShowLoginPanel(Action<string, string> pLoginAction, Action<string, string> pSigninAction, Action pCloseAction)
    {
        //SetEnableAllPanels(false);

        GetPanel<SHUIPanelLogin>(SHUIConstant.PANEL_LOGIN, (pPanel) => 
        {
            pPanel.SetActive(true);
            pPanel.OnLoginAction = pLoginAction;
            pPanel.OnSigninAction = pSigninAction;
            pPanel.OnloseAction = pCloseAction;
        });
    }

    public void CloseLoginPanel()
    {
        GetPanel<SHUIPanelLogin>(SHUIConstant.PANEL_LOGIN, (pPanel) => 
        {
            pPanel.SetActive(false);
            pPanel.OnLoginAction = null;
            pPanel.OnSigninAction = null;
            pPanel.OnloseAction = null;
        });
    }
}
