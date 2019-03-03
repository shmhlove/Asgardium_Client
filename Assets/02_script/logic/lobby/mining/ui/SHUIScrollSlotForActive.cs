using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHActiveSlotData
{
    public string m_strActiveUID;

    public string m_strCompanyName;
    public string m_strCompanyIcon;
    public string m_iResourceIcon;
    public int m_iResourceQuantity;
    public int m_iSupplyQuantity;
    public int m_iPurchaseCost;

    public Action<string> m_pEventPurchaseButton;
}

public class SHUIScrollSlotForActive : MonoBehaviour
{
    [ReadOnlyField] public string m_strActiveUID;

    public UILabel m_pLabelCompany;
    public UISprite m_pSpriteCompany;

    public UISprite m_pSpriteResource;
    public UILabel m_pLabelResourceQuantity;

    public UILabel m_pLabelSupplyQuantity;
    public UILabel m_pLabelPurchaseCost;

    [ReadOnlyField] public Action<string> m_pEventPurchaseButton;

    public void SetData(SHActiveSlotData pData)
    {
        m_pLabelCompany.text = pData.m_strCompanyName;
        m_pSpriteCompany.spriteName = pData.m_strCompanyIcon;
        m_pSpriteResource.spriteName = pData.m_iResourceIcon;
        m_pLabelResourceQuantity.text = pData.m_iResourceQuantity.ToString();
        m_pLabelSupplyQuantity.text = pData.m_iSupplyQuantity.ToString();
        m_pLabelPurchaseCost.text = pData.m_iPurchaseCost.ToString();

        m_strActiveUID = pData.m_strActiveUID;
        m_pEventPurchaseButton = pData.m_pEventPurchaseButton;
    }

    public void OnClickButton()
    {
        m_pEventPurchaseButton?.Invoke(m_strActiveUID);
    }
}
