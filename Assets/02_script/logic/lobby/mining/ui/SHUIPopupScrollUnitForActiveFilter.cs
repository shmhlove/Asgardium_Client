using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHActiveFilterUnitData
{
    public int m_iUnitId;
    public string m_strIconImage;
    public bool m_bIsOn;
}

public class SHUIPopupScrollUnitForActiveFilter : MonoBehaviour
{
    public UISprite m_pSpriteUnit;
    public UIToggle m_pToggle;

    public Action<int, bool> m_pEventUnitToggle;
    public SHActiveFilterUnitData m_pData;

    public void SetUnitInfo(SHActiveFilterUnitData pData, Action<int, bool> pEventUnitToggle)
    {
        if (m_pSpriteUnit)
        {
            m_pSpriteUnit.spriteName = pData.m_strIconImage;
        }

        if (m_pToggle)
        {
            m_pToggle.Set(pData.m_bIsOn, false);
        }
        
        m_pData = pData;
        m_pEventUnitToggle = pEventUnitToggle;
    }

    public void OnClickToggle(bool isOn)
    {
        if (null == m_pData)
            return;

        if (null == m_pEventUnitToggle)
            return;

        m_pEventUnitToggle(m_pData.m_iUnitId, isOn);
    }
}
