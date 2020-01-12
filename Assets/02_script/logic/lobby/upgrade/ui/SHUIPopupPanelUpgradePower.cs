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
        ResetUpgradeInfo();
    }

    public async void ResetUpgradeInfo()
    {
        var pInventory = await Single.Table.GetTable<SHTableServerUserInventory>();
        var pUpgradeInfo = await Single.Table.GetTable<SHTableServerUserUpgradeInfo>();
        var pUpgradePowerTable = await Single.Table.GetTable<SHTableServerMiningActiveMaxMP>();

        var pCurInfo = pUpgradePowerTable.GetData(pUpgradeInfo.MiningPowerLv);
        var pNextInfo = pUpgradePowerTable.GetData(pUpgradeInfo.MiningPowerLv + 1);

        // 자.. 테이블이 없을때 어떻게 처리할까?
        // 비지니스에서 처리한 뒤에 UI를 띄워줘야겠다.
        // 1. 현재 정보가 없을 때 -> 파라미터 혹은 에러로 인해 없을 때
        // 2. 다음 정보가 없을 때 -> 파라미터 혹은 에러로 인해 없을 때, Max레벨이라 없을 때

        if (null != pCurInfo)
        {
            
        }
        else
        {

        }

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
