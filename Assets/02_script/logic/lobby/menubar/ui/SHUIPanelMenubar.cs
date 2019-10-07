using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public enum eLobbyMenuType
{
    None,
    Mining,
    Storage,
    Market,
    Upgrade,
    Menu
}

public class SHUIPanelMenubar : SHUIPanel
{
    public SHUIButton m_pMiningButton;
    public SHUIButton m_pStorageButton;
    public SHUIButton m_pMarketButton;
    public SHUIButton m_pUpgradeButton;
    public SHUIButton m_pMenuButton;

    private eLobbyMenuType m_eCurrentMenu = eLobbyMenuType.None;
    private Action<eLobbyMenuType, eLobbyMenuType> m_pEventOfChangeLobbyMenu;

    public void SetEventForChangeLobbyMenu(Action<eLobbyMenuType, eLobbyMenuType> pCallback)
    {
        m_pEventOfChangeLobbyMenu = pCallback;
    }

    public eLobbyMenuType GetCurrentMenu()
    {
        return m_eCurrentMenu;
    }

    public void ExecuteClick(eLobbyMenuType eType)
    {
        switch(eType)
        {
            case eLobbyMenuType.Mining:
                m_pMiningButton.ExecuteClick();
                break;
            case eLobbyMenuType.Storage:
                m_pStorageButton.ExecuteClick();
                break;
            case eLobbyMenuType.Market:
                m_pMarketButton.ExecuteClick();
                break;
            case eLobbyMenuType.Upgrade:
                m_pUpgradeButton.ExecuteClick();
                break;
            case eLobbyMenuType.Menu:
                m_pMenuButton.ExecuteClick();
                break;
        }
    }

    private void SetMoveMenu(eLobbyMenuType eType)
    {
        m_eCurrentMenu = eType;
    }

    public void OnClickMiningButton()
    {
        m_pEventOfChangeLobbyMenu?.Invoke(eLobbyMenuType.Mining, m_eCurrentMenu);
        SetMoveMenu(eLobbyMenuType.Mining);
    }

    public void OnClickStorageButton()
    {
        m_pEventOfChangeLobbyMenu?.Invoke(eLobbyMenuType.Storage, m_eCurrentMenu);
        SetMoveMenu(eLobbyMenuType.Storage);
    }

    public void OnClickMarketButton()
    {
        m_pEventOfChangeLobbyMenu?.Invoke(eLobbyMenuType.Market, m_eCurrentMenu);
        SetMoveMenu(eLobbyMenuType.Market);
    }

    public void OnClickUpgradeButton()
    {
        m_pEventOfChangeLobbyMenu?.Invoke(eLobbyMenuType.Upgrade, m_eCurrentMenu);
        SetMoveMenu(eLobbyMenuType.Upgrade);
    }

    public void OnClickMenuButton()
    {
        m_pEventOfChangeLobbyMenu?.Invoke(eLobbyMenuType.Menu, m_eCurrentMenu);
        SetMoveMenu(eLobbyMenuType.Menu);
    }
}