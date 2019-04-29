using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public enum eLobbyMenuType
{
    Mining,
    Storage,
    Market,
    Upgrade,
    Menu
}

public class SHUIPanelMenubar : SHUIPanel
{
    private Action<eLobbyMenuType> m_pEventOfChangeLobbyMenu;

    public void SetEventOfChangeLobbyMenu(Action<eLobbyMenuType> pCallback)
    {
        m_pEventOfChangeLobbyMenu = pCallback;
    }

    public void OnClickMiningButton()
    {
        m_pEventOfChangeLobbyMenu?.Invoke(eLobbyMenuType.Mining);
    }

    public void OnClickStorageButton()
    {
        m_pEventOfChangeLobbyMenu?.Invoke(eLobbyMenuType.Storage);
    }

    public void OnClickMarketButton()
    {
        m_pEventOfChangeLobbyMenu?.Invoke(eLobbyMenuType.Market);
    }

    public void OnClickUpgradeButton()
    {
        m_pEventOfChangeLobbyMenu?.Invoke(eLobbyMenuType.Upgrade);
    }

    public void OnClickMenuButton()
    {
        m_pEventOfChangeLobbyMenu?.Invoke(eLobbyMenuType.Menu);
    }
}