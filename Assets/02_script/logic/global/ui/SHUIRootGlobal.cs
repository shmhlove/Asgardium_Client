using UnityEngine;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootGlobal : SHUIRoot
{
    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public async Task ShowFadePanel(Action pCallback = null)
    {
        var pPanel = await GetPanel<SHUIPanelFade>(SHUIConstant.PANEL_FADE);
        pPanel.Show(pCallback);
    }

    public async Task CloseFadePanel(Action pCallback = null)
    {
        var pPanel = await GetPanel<SHUIPanelFade>(SHUIConstant.PANEL_FADE);
        pPanel.SetActive(true);
        pPanel.Close(pCallback);
    }

    public async Task ShowAlert(string strMessage, Action pCallback = null)
    {
        var pPanel = await GetPanel<SHUIPanelAlert>(SHUIConstant.PANEL_ALERT);

        Action<eAlertButtonAction> pResult = (eAction) =>
        {
            pPanel.Close(pCallback);
        };
        
        pPanel.Show("", strMessage, eAlertButtonType.OneButton, pResult);
    }
    
    public async Task ShowIndicator()
    {
        var pPanel = await GetPanel<SHUIPanelIndicator>(SHUIConstant.PANEL_INDICATOR);
        pPanel.Show();
    }

    public async Task CloseIndicator()
    {
        var pPanel = await GetPanel<SHUIPanelIndicator>(SHUIConstant.PANEL_INDICATOR);
        pPanel.Close();
    }
}