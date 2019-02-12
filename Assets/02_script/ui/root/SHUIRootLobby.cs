using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootLobby : SHUIRoot
{
    public async void ShowMenubarPanel(params object[] pArgs)
    {
        var pPanel = await GetPanel<SHUIPanelMenubar>(SHUIConstant.PANEL_MENU_BAR);
        pPanel.Show(pArgs);
    }

    public async void ShowMiningPanel(params object[] pArgs)
    {
        CloseAllMenuPanels();

        var pPanel = await GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        pPanel.Show(pArgs);
    }

    public async void ShowStoragePanel(params object[] pArgs)
    {
        CloseAllMenuPanels();

        var pPanel = await GetPanel<SHUIPanelStorage>(SHUIConstant.PANEL_STORAGE);
        pPanel.Show(pArgs);
    }

    public async void ShowMarketPanel(params object[] pArgs)
    {
        CloseAllMenuPanels();

        var pPanel = await GetPanel<SHUIPanelMarket>(SHUIConstant.PANEL_MARKET);
        pPanel.Show(pArgs);
    }

    public async void ShowUpgradePanel(params object[] pArgs)
    {
        CloseAllMenuPanels();

        var pPanel = await GetPanel<SHUIPanelUpgrade>(SHUIConstant.PANEL_UPGRADE);
        pPanel.Show(pArgs);
    }

    public async void ShowMenuPanel(params object[] pArgs)
    {
        CloseAllMenuPanels();
        
        var pPanel = await GetPanel<SHUIPanelMenu>(SHUIConstant.PANEL_MENU);
        pPanel.Show(pArgs);
    }

    public async void CloseAllMenuPanels()
    {
        var pMining = await GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        pMining.Close();
        var pStorage = await GetPanel<SHUIPanelStorage>(SHUIConstant.PANEL_STORAGE);
        pStorage.Close();
        var pMarket = await GetPanel<SHUIPanelMarket>(SHUIConstant.PANEL_MARKET);
        pMarket.Close();
        var pUpgrade = await GetPanel<SHUIPanelUpgrade>(SHUIConstant.PANEL_UPGRADE);
        pUpgrade.Close();
        var pMenu = await GetPanel<SHUIPanelMenu>(SHUIConstant.PANEL_MENU);
        pMenu.Close();
    }
}
