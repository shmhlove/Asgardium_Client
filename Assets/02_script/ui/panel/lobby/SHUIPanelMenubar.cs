using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelMenubar : SHUIPanel
{
    public enum eMainMenus
    {
        None,
        Mining,
        Storage,
        Market,
        Upgrade,
        Menu,
    }

    [Header("Menu Buttons")]
    public SHUIButton m_pButtonMining;
    public SHUIButton m_pButtonStorage;
    public SHUIButton m_pButtonMarket;
    public SHUIButton m_pButtonUpgrade;
    public SHUIButton m_pButtonMenu;

    private Action m_pEventMining;
    private Action m_pEventStorage;
    private Action m_pEventMarket;
    private Action m_pEventUpgrade;
    private Action m_pEventMenu;

    public override void OnBeforeShow(params object[] pArgs)
    {
        m_pEventMining = (Action)pArgs[0];
        m_pEventStorage = (Action)pArgs[1];
        m_pEventMarket = (Action)pArgs[2];
        m_pEventUpgrade = (Action)pArgs[3];
        m_pEventMenu = (Action)pArgs[4];

        SetButtonState(eMainMenus.Mining, UIButtonColor.State.Disabled);
    }

    private void SetButtonState(eMainMenus eButtonType, UIButtonColor.State eStateType)
    {
        Func<eMainMenus, bool> IsApplyState = 
        (eCurlType) => { return (eMainMenus.None == eButtonType) || (eCurlType == eButtonType); };

        m_pButtonMining.SetState(IsApplyState(eMainMenus.Mining) ? eStateType : UIButtonColor.State.Normal, false);
        m_pButtonStorage.SetState(IsApplyState(eMainMenus.Storage) ? eStateType : UIButtonColor.State.Normal, false);
        m_pButtonMarket.SetState(IsApplyState(eMainMenus.Market) ? eStateType : UIButtonColor.State.Normal, false);
        m_pButtonUpgrade.SetState(IsApplyState(eMainMenus.Upgrade) ? eStateType : UIButtonColor.State.Normal, false);
        m_pButtonMenu.SetState(IsApplyState(eMainMenus.Menu) ? eStateType : UIButtonColor.State.Normal, false);
    }

    public void OnClickMining()
    {
        if (null != m_pEventMining)
        {
            m_pEventMining();
        }

        SetButtonState(eMainMenus.Mining, UIButtonColor.State.Disabled);
    }

    public void OnClickStorage()
    {
        if (null != m_pEventStorage)
        {
            m_pEventStorage();
        }

        SetButtonState(eMainMenus.Storage, UIButtonColor.State.Disabled);
    }

    public void OnClickMarket()
    {
        if (null != m_pEventMarket)
        {
            m_pEventMarket();
        }

        SetButtonState(eMainMenus.Market, UIButtonColor.State.Disabled);
    }

    public void OnClickUpgrade()
    {
        if (null != m_pEventUpgrade)
        {
            m_pEventUpgrade();
        }

        SetButtonState(eMainMenus.Upgrade, UIButtonColor.State.Disabled);
    }

    public void OnClickMenu()
    {
        if (null != m_pEventMenu)
        {
            m_pEventMenu();
        }

        SetButtonState(eMainMenus.Menu, UIButtonColor.State.Disabled);
    }
}
