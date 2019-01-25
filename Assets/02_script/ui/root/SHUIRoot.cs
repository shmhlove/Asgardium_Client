using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRoot : MonoBehaviour
{
	public List<SHUIPanel> m_listPanels;
    private Dictionary<string, SHUIPanel> m_dicPanels;

    public virtual void Awake()
    {
        Single.UI.AddRoot(this.GetType(), this);

        m_dicPanels = new Dictionary<string, SHUIPanel>();
        m_listPanels.ForEach((pPanel) => 
        {
            m_dicPanels.Add(pPanel.gameObject.name, pPanel);
        });
    }

    public void GetPanel(string strName, Action<SHUIPanel> pCallback)
    {
        GetPanel<SHUIPanel>(strName, pCallback);
    }

    public void GetPanel<T>(string strName, Action<T> pCallback) where T : SHUIPanel
    {
        if (true == m_dicPanels.ContainsKey(strName))
        {
            pCallback(m_dicPanels[strName] as T);
        }
        else
        {
            Single.Resources.GetComponentByObject<T>(strName, (pPanel) => 
            {
                if (null == pPanel)
                    pCallback(default(T));
                else
                {
                    AddUIPanel(strName, pPanel);
                    pCallback(pPanel);
                }
            });
        }
    }

    public void SetEnableAllPanels(bool isActive)
    {
        foreach(var kvp in m_dicPanels)
        {
            kvp.Value.SetActive(isActive);
        }
    }

    private void AddUIPanel(string strName, SHUIPanel pPanel)
    {
        if (false == m_dicPanels.ContainsKey(strName))
        {
            m_dicPanels.Add(strName, pPanel);
        }
        else
        {
            m_dicPanels[strName] = pPanel;
        }
    }
}