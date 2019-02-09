using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRoot : MonoBehaviour
{
    [Header("Basic Info")]
    public List<SHUIPanel> panels;

    [Header("Internal Info")]
    private Dictionary<string, SHUIPanel> m_dicPanels;

    public virtual void Awake()
    {
        Single.UI.AddRoot(gameObject.name, this);
        
        m_dicPanels = new Dictionary<string, SHUIPanel>();
        panels.ForEach((pPanel) =>
        {
            m_dicPanels.Add(pPanel.gameObject.name, pPanel);
        });
    }

    public virtual void OnDestroy()
    {
        if (true == SHUIManager.IsExists)
        {
            Single.UI.DelRoot(gameObject.name);
        }
    }

    public void GetPanel(string strName, Action<SHUIPanel> pCallback)
    {
        GetPanel<SHUIPanel>(strName, pCallback);
    }

    public async void GetPanel<T>(string strName, Action<T> pCallback) where T : SHUIPanel
    {
        if (true == m_dicPanels.ContainsKey(strName))
        {
            pCallback(m_dicPanels[strName] as T);
        }
        else
        {
            var pPanel = await Single.Resources.GetComponentByObject<T>(typeof(T).ToString());
            AddUIPanel(strName, pPanel);
            pCallback(pPanel);
        }
    }
    
    private void AddUIPanel(string strName, SHUIPanel pPanel)
    {
        if (null == pPanel)
        {
            return;
        }

        if (false == m_dicPanels.ContainsKey(strName))
        {
            m_dicPanels.Add(strName, pPanel);
        }
        else
        {
            m_dicPanels[strName] = pPanel;
        }

        pPanel.transform.SetParent(transform);
    }
}