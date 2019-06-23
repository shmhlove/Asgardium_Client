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

public class SHUIScrollUnitForActiveFilter : MonoBehaviour
{
    public UISprite m_pSpriteUnit;
    public UIToggle m_pToggle;

    public Action<int, bool> m_pEventToToggle;
    public SHTableServerGlobalUnitDataData m_pData;

    public void SetUnitInfo(SHActiveFilterUnitData pData, Action<int, bool> pEventToToggle)
    {
        if (m_pSpriteUnit)
        {
            m_pSpriteUnit.spriteName = pData.m_strIconImage;
        }

        if (m_pToggle)
        {
            bool? bIsOn = SHPlayerPrefs.GetBool(pData.m_iUnitId.ToString());
            m_pToggle.Set((null == bIsOn || bIsOn.Value), false);
        }

        m_pData = pData;
        m_pEventToToggle = pEventToToggle;
    }

    public void OnClickToggle(bool isOn)
    {
        if (null == m_pData)
            return;

        if (null == m_pEventToToggle)
            return;

        m_pEventToToggle(m_pData.m_iUnitId, isOn);
    }
}
