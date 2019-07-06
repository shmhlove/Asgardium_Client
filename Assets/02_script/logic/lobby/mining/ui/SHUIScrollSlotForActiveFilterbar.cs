using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIScrollSlotForActiveFilterbar : MonoBehaviour
{
    public UISprite m_pSpriteUnit;

    public void SetData(SHActiveFilterUnitData pData)
    {
        if (m_pSpriteUnit)
        {
            m_pSpriteUnit.spriteName = pData.m_strIconImage;
        }
    }
}
