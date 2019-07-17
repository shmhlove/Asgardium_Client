using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHTableGridSlotForUnitData
{
    public int      m_iUnitID;
    public string   m_strIconName;
    public string   m_strUnitName;
    public int      m_iUnitValue;
    public int      m_iGoldValue;
}

public class SHUITableGridSlotForUnit : MonoBehaviour
{
    public UISprite m_pSpriteIcon;
    public UILabel m_pLabelName;
    public UILabel m_pLabelUnitValue;
    public UILabel m_pLabelGoldValue;

    private SHTableGridSlotForUnitData m_pData;
    private Action<int> m_pEventTransaction;

    public void SetData(SHTableGridSlotForUnitData pData, Action<int> pEventTransaction)
    {
        if (m_pSpriteIcon) {
            m_pSpriteIcon.spriteName = pData.m_strIconName;
        }

        if (m_pLabelName) {
            m_pLabelName.text = pData.m_strUnitName;
        }

        if (m_pLabelUnitValue) {
            m_pLabelUnitValue.text = string.Format("{0:##,##0}", pData.m_iUnitValue);
        }

        if (m_pLabelGoldValue) {
            m_pLabelGoldValue.text = string.Format("{0:##,##0}", pData.m_iGoldValue);
        }

        m_pData = pData;
        m_pEventTransaction = pEventTransaction;
    }

    public void OnClickButtonForTransaction()
    {
        if (null == m_pData) {
            return;
        }

        if (null == m_pEventTransaction) {
            return;
        }

        m_pEventTransaction(m_pData.m_iUnitID);
    }
}
