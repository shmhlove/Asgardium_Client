using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelUpgrade : SHUIPanel
{
    public SHUIActiveUpgrade m_pActiveUpgrade;

    // 엑티브 업그레이드
    public void SetUpgradeInfo(long lPowerLv, long lTimeLv)
    {
        m_pActiveUpgrade.SetPowerLevel(lPowerLv.ToString());
        m_pActiveUpgrade.SetTimeLevel(lTimeLv.ToString());
    }
    public void AddEventForPowerUpUpgrade(Action pCallback)
    {
        m_pActiveUpgrade.AddEventToPowerUp(pCallback);
    }
    public void AddEventForTimeUpUpgrade(Action pCallback)
    {
        m_pActiveUpgrade.AddEventToTimeUp(pCallback);
    }
    
    // 열린 회사 업그레이드
    // 닫힌 회사 구매
}
