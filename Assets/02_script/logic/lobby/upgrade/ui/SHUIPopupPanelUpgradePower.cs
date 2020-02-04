using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class SHUIPopupPanelUpgradePower : SHUIPanel
{
    public UILabel m_pLabelCurLv;
    public UILabel m_pLabelCurMaxMP;
    public UILabel m_pLabelNextLv;
    public UILabel m_pLabelNextMaxMP;
    public UILabel m_pLabelCost;
    public UILabel m_pLabelHasGold;
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
    }

    // 어떤 데이터가 필요한지 정리해서 구조체를 만들든 하자.
    // 그 데이터를 파라미터로 전달받아 UI를 업데이트하자
    public async void UpdateUI()
    {
        var pInventory = await Single.Table.GetTable<SHTableServerUserInventory>();
        var pUpgradeInfo = await Single.Table.GetTable<SHTableServerUserUpgradeInfo>();
        var pUpgradePowerTable = await Single.Table.GetTable<SHTableServerMiningActiveMaxMP>();

        var pCurInfo = pUpgradePowerTable.GetData(pUpgradeInfo.MiningPowerLv);
        var pNextInfo = pUpgradePowerTable.GetData(pUpgradeInfo.MiningPowerLv + 1);

        // 현재 정보
        // 다음 정보
        // 코스트 정보
        // 업그레이드 버튼

        // public UILabel m_pLabelCurLv;
        // public UILabel m_pLabelCurMaxMP;
        // public UILabel m_pLabelNextLv;
        // public UILabel m_pLabelNextMaxMP;
        // public UILabel m_pLabelCost;
        // public UILabel m_pLabelHasGold
    }

    public void OnClickCloseButton()
    {
        Close();

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
