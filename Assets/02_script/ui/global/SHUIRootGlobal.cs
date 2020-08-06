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
    
    public async Task<SHUIPanelAlert> GetAlert()
    {
        return await GetPanel<SHUIPanelAlert>(SHUIConstant.PANEL_ALERT);
    }

    public async Task<SHUIPanelIndicator> GetIndicator()
    {
        return await GetPanel<SHUIPanelIndicator>(SHUIConstant.PANEL_INDICATOR);
    }

    public async Task<SHUIPanelFade> GetFade()
    {
        return await GetPanel<SHUIPanelFade>(SHUIConstant.PANEL_INDICATOR);
    }
}