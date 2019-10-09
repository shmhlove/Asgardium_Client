using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIActiveUpgrade : MonoBehaviour
{
    public UILabel m_pLabelTimeLv;
    public UILabel m_pLabelPowerLv;

    public void SetPowerLevel(string strValue)
    {
        m_pLabelPowerLv.text = strValue;
    }

    public void SetTimeLevel(string strValue)
    {
        m_pLabelTimeLv.text = strValue;
    }

    public void OnClickPowerUpButton()
    {

    }

    public void OnClickTimeUpButton()
    {

    }
}
