using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRoot : MonoBehaviour
{
    [Header("Basic Info")]
    public List<SHUIPanel> panels;

    [Header("Internal Info")]
    private Dictionary<Type, SHUIPanel> m_dicPanels;

    public virtual void Awake()
    {
        Single.UI.AddRoot(this.GetType(), this);
        
        m_dicPanels = new Dictionary<Type, SHUIPanel>();
        panels.ForEach((pPanel) =>
        {
            m_dicPanels.Add(pPanel.GetType(), pPanel);
        });
    }

    public virtual void OnDestroy()
    {
        if (true == SHUIManager.IsExists)
        {
            Single.UI.DelRoot(this.GetType());
        }
    }

    public void GetPanel(Action<SHUIPanel> pCallback)
    {
        GetPanel<SHUIPanel>(pCallback);
    }

    public void GetPanel<T>(Action<T> pCallback) where T : SHUIPanel
    {
        if (true == m_dicPanels.ContainsKey(typeof(T)))
        {
            pCallback(m_dicPanels[typeof(T)] as T);
        }
        else
        {
            Single.Resources.GetComponentByObject<T>(typeof(T).ToString(), (pPanel) => 
            {
                if (null == pPanel)
                    pCallback(default(T));
                else
                {
                    AddUIPanel(typeof(T), pPanel);
                    pCallback(pPanel);
                }
            });
        }
    }
    
    private void AddUIPanel(Type type, SHUIPanel pPanel)
    {
        if (false == m_dicPanels.ContainsKey(type))
        {
            m_dicPanels.Add(type, pPanel);
        }
        else
        {
            m_dicPanels[type] = pPanel;
        }

        pPanel.transform.SetParent(transform);
    }
}