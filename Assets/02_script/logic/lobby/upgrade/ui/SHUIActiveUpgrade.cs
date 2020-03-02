using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIActiveUpgrade : MonoBehaviour
{
    public UILabel m_pLabelTimeLv;
    public UILabel m_pLabelPowerLv;
    private List<GameObject> m_pEventObjects = new List<GameObject>();

    public void SetPowerLevel(string strValue)
    {
        m_pLabelPowerLv.text = strValue;
    }

    public void SetTimeLevel(string strValue)
    {
        m_pLabelTimeLv.text = strValue;
    }

    public void AddEvent(GameObject gameObject)
    {
        if (false == m_pEventObjects.Contains(gameObject))
        {
            m_pEventObjects.Add(gameObject);
        }
    }

    public void OnClickPowerUpButton()
    {
        foreach(var item in m_pEventObjects)
        {
            item.SendMessage("OnEventForUpgradePowerupBtn", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void OnClickTimeUpButton()
    {
        foreach(var item in m_pEventObjects)
        {
            item.SendMessage("OnEventForUpgradeTimeupBtn", SendMessageOptions.DontRequireReceiver);
        }
    }
}
