using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelMenubar : SHUIPanel
{
    private Action m_pEventMenuMining;
    private Action m_pEventMenuStorage;
    private Action m_pEventMenuMarket;
    private Action m_pEventMenuUpgrade;
    private Action m_pEventMenuMenu;

    public override void OnBeforeShow(params object[] pArgs)
    {
        m_pEventMenuMining = (Action)pArgs[0];
        m_pEventMenuStorage = (Action)pArgs[1];
        m_pEventMenuMarket = (Action)pArgs[2];
        m_pEventMenuUpgrade = (Action)pArgs[3];
        m_pEventMenuMenu = (Action)pArgs[4];
    }

    public void OnClickMining()
    {
        if (null != m_pEventMenuMining)
        {
            m_pEventMenuMining();
        }
    }

    public void OnClickStorage()
    {
        if (null != m_pEventMenuStorage)
        {
            m_pEventMenuStorage();
        }
    }

    public void OnClickMarket()
    {
        if (null != m_pEventMenuMarket)
        {
            m_pEventMenuMarket();
        }
    }

    public void OnClickUpgrade()
    {
        if (null != m_pEventMenuUpgrade)
        {
            m_pEventMenuUpgrade();
        }
    }

    public void OnClickMenu()
    {
        if (null != m_pEventMenuMenu)
        {
            m_pEventMenuMenu();
        }
    }
}
