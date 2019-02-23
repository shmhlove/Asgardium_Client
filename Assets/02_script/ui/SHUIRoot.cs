using UnityEngine;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class SHUIRoot : MonoBehaviour
{
    [Header("Basic Info")]
    public List<SHUIPanel> panels;

    [Header("Internal Info")]
    private Dictionary<string, SHUIPanel> m_dicPanels;
    private SemaphoreSlim m_pSemaphoreSlim = new SemaphoreSlim(1,1);

    public virtual void Awake()
    {
        Single.UI.AddRoot(gameObject.name, this);
        
        m_dicPanels = new Dictionary<string, SHUIPanel>();
        panels.ForEach((pPanel) =>
        {
            pPanel.OnInitialized();
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
    
    public async Task<T> GetPanel<T>(string strName) where T : SHUIPanel
    {
        await m_pSemaphoreSlim.WaitAsync();

        var pPromise = new TaskCompletionSource<T>();
        try
        {
            if (true == m_dicPanels.ContainsKey(strName))
            {
                pPromise.TrySetResult(m_dicPanels[strName] as T);
            }
            else
            {
                var pPanel = await Single.Resources.GetComponentByObject<T>(typeof(T).ToString());
                AddUIPanel(strName, pPanel);
                pPromise.TrySetResult(pPanel);
            }
        }
        finally
        {
            m_pSemaphoreSlim.Release();
        }

        return await pPromise.Task;
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