using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelUpgrade : SHUIPanel
{
    public SHUIActiveUpgrade m_pActiveUpgradeInfo;

    // 엑티브 업그레이드
    public void AddEventForActiveUpgrade(GameObject gameObject)
    {
        m_pActiveUpgradeInfo.AddEvent(gameObject);
    }

    public void SetActiveUpgradeInfo(long lPowerLv, long lTimeLv)
    {
        m_pActiveUpgradeInfo.SetPowerLevel(lPowerLv.ToString());
        m_pActiveUpgradeInfo.SetTimeLevel(lTimeLv.ToString());
    }

    // 열린 회사 업그레이드
    // 닫힌 회사 구매
}
