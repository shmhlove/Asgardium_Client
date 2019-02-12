using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHSceneLobby : MonoBehaviour
{
    public void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    public async void Start()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        Action EventMenuMining = OnClickMenuMining;
        Action EventMenuStorage = OnClickMenuStorage;
        Action EventMenuMarket = OnClickMenuMarket;
        Action EventMenuUpgrade = OnClickMenuUpgrade;
        Action EventMenuMenu = OnClickMenuMenu;
        pUIRoot.ShowMenubarPanel(EventMenuMining, EventMenuStorage, EventMenuMarket, EventMenuUpgrade, EventMenuMenu);
    }

    public async void OnClickMenuMining()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        pUIRoot.ShowMiningPanel();
    }

    public async void OnClickMenuStorage()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        pUIRoot.ShowStoragePanel();
    }

    public async void OnClickMenuMarket()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        pUIRoot.ShowMarketPanel();
    }

    public async void OnClickMenuUpgrade()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        pUIRoot.ShowUpgradePanel();
    }

    public async void OnClickMenuMenu()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        pUIRoot.ShowMenuPanel();
    }
}
