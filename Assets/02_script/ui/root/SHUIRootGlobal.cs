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

    public async void ShowFadePanel(Action pCallback = null)
    {
        var pPanel = await GetPanel<SHUIPanelFade>(SHUIConstant.PANEL_FADE);
        pPanel.Show(pCallback);
    }

    public async void CloseFadePanel(Action pCallback = null)
    {
        var pPanel = await GetPanel<SHUIPanelFade>(SHUIConstant.PANEL_FADE);
        pPanel.SetActive(true);
        pPanel.Close(pCallback);
    }

    public async void ShowAlert(string strMessage, Action pCallback = null)
    {
        var pPanel = await GetPanel<SHUIPanelAlert>(SHUIConstant.PANEL_ALERT);

        Action<eAlertButtonAction> pResult = (eAction) =>
        {
            pPanel.Close(pCallback);
        };
        
        pPanel.SetActive(true);
        pPanel.Show("", strMessage, eAlertButtonType.OneButton, pResult);
    }
}