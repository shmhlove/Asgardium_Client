using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUITableSlotForBasicGoods : MonoBehaviour
{
    public UILabel m_pLabelAsgariumValue;

    public UILabel m_pLabelGoldValue;
    
    public void SetData(int iAsgariumValue, int iGoldValue)
    {
        if (m_pLabelAsgariumValue) {
            m_pLabelAsgariumValue.text = string.Format("{0:##,##0}",iAsgariumValue);
        }
        if (m_pLabelGoldValue) {
            m_pLabelGoldValue.text = string.Format("{0:##,##0}",iGoldValue);
        }
    }
}
