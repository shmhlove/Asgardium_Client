using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootGlobal : SHUIRoot
{
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void ShowFadePanel(Action pCallback = null)
    {
        GetPanel<SHUIPanelFade>(SHUIConstant.PANEL_FADE, (pPanel) => 
        {
            pPanel.Show(pCallback);
        });
    }

    public void CloseFadePanel(Action pCallback = null)
    {
        GetPanel<SHUIPanelFade>(SHUIConstant.PANEL_FADE, (pPanel) =>
        {
            pPanel.SetActive(true);
            pPanel.Close(pCallback);
        });
    }

    public void ShowAlert(string strMessage, Action pCallback = null)
    {
        GetPanel<SHUIPanelAlert>(SHUIConstant.PANEL_ALERT, (pPanel) =>
        {
            Action<eAlertButtonAction> pResult = (eAction) =>
            {
                pPanel.Close(pCallback);
            };
            
            pPanel.SetActive(true);
            pPanel.Show("", strMessage, eAlertButtonType.OneButton, pResult);
        });
    }
}