using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHActiveSlotData
{
    public string m_strSlotId;

    public string m_strInstanceId;
    public string m_strCompanyName;
    public string m_strCompanyIcon;
    public string m_strResourceIcon;

    public int m_iUnitId;
    public int m_iResourceQuantity;
    public int m_iEfficiencyLevel;
    public int m_iSupplyQuantity;
    public int m_iPurchaseCost;

    public Action<string> m_pEventPurchaseButton;
    public Action<string> m_pEventShowSubItemsButton;
}

public class SHUIScrollSlotForActive : MonoBehaviour
{
    public UILabel m_pLabelCompany;
    public UISprite m_pSpriteCompany;

    public UISprite m_pSpriteResource;
    public UILabel m_pLabelResourceQuantity;

    public UILabel m_pLabelSupplyQuantity;
    public UILabel m_pLabelPurchaseCost;

    public UISprite m_pSpriteResourceLevel;
    public UILabel m_pLabelResourceLevel;

    [ReadOnlyField] public SHActiveSlotData m_pData = null;

    public void SetData(SHActiveSlotData pData)
    {
        if (m_pLabelCompany) {
            m_pLabelCompany.text = pData.m_strCompanyName;
        }

        if (m_pSpriteCompany) {
            m_pSpriteCompany.spriteName = pData.m_strCompanyIcon;
        }

        if (m_pSpriteResource) {
            m_pSpriteResource.spriteName = pData.m_strResourceIcon;
        }

        if (m_pLabelResourceQuantity) {
            m_pLabelResourceQuantity.text = pData.m_iResourceQuantity.ToString();
        }

        if (m_pLabelResourceLevel) {
            m_pLabelResourceLevel.text = pData.m_iEfficiencyLevel.ToString();
        }

        if (m_pLabelSupplyQuantity) {
            m_pLabelSupplyQuantity.text = pData.m_iSupplyQuantity.ToString();
        }

        if (m_pLabelPurchaseCost) {
            m_pLabelPurchaseCost.text = pData.m_iPurchaseCost.ToString();
        }

        if (m_pSpriteResourceLevel) {
            m_pSpriteResourceLevel.gameObject.SetActive(1 < pData.m_iEfficiencyLevel);
        }

        m_pData = pData;
    }

    public void OnClickPurchaseButton()
    {
        if (null == m_pData)
            return;

        m_pData.m_pEventPurchaseButton?.Invoke(m_pData.m_strInstanceId);
    }

    public void OnClickShowSubItemsButton()
    {
        if (null == m_pData)
            return;

        m_pData.m_pEventShowSubItemsButton?.Invoke(m_pData.m_strSlotId);
    }
}
