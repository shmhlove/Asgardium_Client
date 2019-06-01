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

    public bool m_bIsSubItems;

    public Action<string> m_pEventPurchaseButton;
    public Action<string> m_pEventShowSubItemsButton;

    public void CopyFrom(SHActiveSlotData pData)
    {
        this.m_strSlotId = pData.m_strSlotId;

        this.m_strInstanceId = pData.m_strInstanceId;
        this.m_strCompanyName = pData.m_strCompanyName;
        this.m_strCompanyIcon = pData.m_strCompanyIcon;
        this.m_strResourceIcon = pData.m_strResourceIcon;

        this.m_iUnitId = pData.m_iUnitId;
        this.m_iResourceQuantity = pData.m_iResourceQuantity;
        this.m_iEfficiencyLevel = pData.m_iEfficiencyLevel;
        this.m_iSupplyQuantity = pData.m_iSupplyQuantity;
        this.m_iPurchaseCost = pData.m_iPurchaseCost;

        this.m_bIsSubItems = pData.m_bIsSubItems;

        this.m_pEventPurchaseButton = pData.m_pEventPurchaseButton;
        this.m_pEventShowSubItemsButton = pData.m_pEventShowSubItemsButton;
    }
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

    public GameObject m_pDimmed;
    public GameObject m_pSubItems;

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

        if (m_pDimmed) {
            if (0 == pData.m_iSupplyQuantity) {
                m_pDimmed.SetActive(true);
            }
            else {
                m_pDimmed.SetActive(false);
            }
        }

        //if (m_pSubItems) {
        //    m_pSubItems.SetActive(pData.m_bIsSubItems);
        //}

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
