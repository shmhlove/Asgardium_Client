using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class SHUIPopupPanelUpgradePower : SHUIPanel
{
    private Action<bool> m_pEventForClickBtn;

    public override void OnBeforeShow(params object[] pArgs)
    {
        m_pEventForClickBtn = null;

        if (0 >= pArgs.Length || 1 < pArgs.Length)
        {
            OnClickCloseButton();
            Debug.LogError("SHUIPopupPanelUpgradePower need only 1 parameters.");
            return;
        }

        m_pEventForClickBtn = (Action<bool>)pArgs[0];
        ResetUpgradeInfo();
    }

    public async void ResetUpgradeInfo()
    {
        var pUpgradeInfo = await Single.Table.GetTable<SHTableServerUserUpgradeInfo>();
        var pUpgradePower = await Single.Table.GetTable<SHTableServerMiningActiveMaxMP>();

        // 현재 정보
        // 다음 정보
        // 코스트 정보
        // 업그레이드 버튼
    }

    public void OnClickCloseButton()
    {
        if (null != m_pEventForClickBtn)
        {
            m_pEventForClickBtn(false);
        }
    }

    public void OnClickUpgradeButton()
    {
        if (null != m_pEventForClickBtn)
        {
            m_pEventForClickBtn(true);
        }
    }
}
