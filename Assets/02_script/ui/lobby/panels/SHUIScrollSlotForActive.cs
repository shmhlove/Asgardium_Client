using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHActiveSlotData
{
    public string m_strGroupId;         // GroupID

    public string m_strInstanceId;      // 회사 인스턴스 ID
    public string m_strCompanyName;     // 회사 이름
    public string m_strCompanyIcon;     // 회사 아이콘
    public string m_strUnitIcon;        // 유닛 아이콘

    public int m_iUnitId;               // 유닛 ID
    public int m_iUnitQuantity;         // 구매시 획득 유닛 물량
    public int m_iEfficiencyLevel;      // 회사 레벨
    public int m_iSupplyQuantity;       // 구매 가능한 공급 물량
    public int m_iPurchaseCost;         // 구매 가격

    public bool m_bIsSubItems;
    public bool m_bIsNPCCompany;        // NPC 회사여부

    public Action<string> m_pEventPurchaseButton;
    public Action<string> m_pEventShowSubUnitsButton;

    public void CopyFrom(SHActiveSlotData pData)
    {
        this.m_strGroupId = pData.m_strGroupId;

        this.m_strInstanceId = pData.m_strInstanceId;
        this.m_strCompanyName = pData.m_strCompanyName;
        this.m_strCompanyIcon = pData.m_strCompanyIcon;
        this.m_strUnitIcon = pData.m_strUnitIcon;

        this.m_iUnitId = pData.m_iUnitId;
        this.m_iUnitQuantity = pData.m_iUnitQuantity;
        this.m_iEfficiencyLevel = pData.m_iEfficiencyLevel;
        this.m_iSupplyQuantity = pData.m_iSupplyQuantity;
        this.m_iPurchaseCost = pData.m_iPurchaseCost;

        this.m_bIsSubItems = pData.m_bIsSubItems;
        this.m_bIsNPCCompany = pData.m_bIsNPCCompany;

        this.m_pEventPurchaseButton = pData.m_pEventPurchaseButton;
        this.m_pEventShowSubUnitsButton = pData.m_pEventShowSubUnitsButton;
    }
}

public class SHUIScrollSlotForActive : MonoBehaviour
{
    public UILabel m_pLabelCompany;
    public UISprite m_pSpriteCompany;

    public UISprite m_pSpriteUnit;
    public UILabel m_pLabelUnitQuantity;

    public UILabel m_pLabelSupplyQuantity;
    public UILabel m_pLabelPurchaseCost;

    public UISprite m_pSpriteUnitLevel;
    public UILabel m_pLabelUnitLevel;

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

        if (m_pSpriteUnit) {
            m_pSpriteUnit.spriteName = pData.m_strUnitIcon;
        }

        if (m_pLabelUnitQuantity) {
            m_pLabelUnitQuantity.text = pData.m_iUnitQuantity.ToString();
        }

        if (m_pLabelUnitLevel) {
            m_pLabelUnitLevel.text = pData.m_iEfficiencyLevel.ToString();
        }

        if (m_pLabelSupplyQuantity) {
            m_pLabelSupplyQuantity.text = pData.m_iSupplyQuantity.ToString();
        }

        if (m_pLabelPurchaseCost) {
            m_pLabelPurchaseCost.text = pData.m_iPurchaseCost.ToString();
        }

        if (m_pSpriteUnitLevel) {
            m_pSpriteUnitLevel.gameObject.SetActive(1 < pData.m_iEfficiencyLevel);
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

    public void OnClickShowSubUnitsButton()
    {
        if (null == m_pData)
            return;

        m_pData.m_pEventShowSubUnitsButton?.Invoke(m_pData.m_strGroupId);
    }
}
