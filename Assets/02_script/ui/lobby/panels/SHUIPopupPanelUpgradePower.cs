using UnityEngine;
using UnityEngine.Assertions;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

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

    public async void UpdateUI(string jsonString)
    {
        Assert.IsFalse(string.IsNullOrEmpty(jsonString));
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();
        
        var pJson = JsonMapper.ToObject(jsonString);
        m_pLabelCurLv.text = string.Format(pStringTable.GetString("1120002"), (int)pJson["curLv"]);
        m_pLabelCurMaxMP.text = string.Format(pStringTable.GetString("1120005"), (int)pJson["curMP"]);
        m_pLabelNextLv.text = string.Format(pStringTable.GetString("1120002"), (int)pJson["nextLv"]);
        m_pLabelNextMaxMP.text = string.Format(pStringTable.GetString("1120005"), (int)pJson["nextMP"]);
        m_pLabelCost.text = string.Format("{0:#,###}", (int)pJson["upgradeCost"]);
        m_pLabelHasGold.text = string.Format(pStringTable.GetString("1120003"), string.Format("{0:#,###}", (int)pJson["hasGold"]));
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
