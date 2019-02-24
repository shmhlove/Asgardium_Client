using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHUIActiveInformation : MonoBehaviour
{
    public UILabel m_pLabelTimer;
    public UILabel m_pLabelPower;

    public void SetTimer(string strValue)
    {
        m_pLabelTimer.text = strValue;
    }

    public void SetMiningPower(string strValue)
    {
        m_pLabelPower.text = strValue;
    }
}
