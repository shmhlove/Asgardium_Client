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
        // 아래 코드를 한순간에 한번만 실행 될 수 있도록 세마포어로 대기시켜준다.
        // 비동기 실행시 똑같은 리소스를 여러번 동적로드 할 수 있다.
        await m_pSemaphoreSlim.WaitAsync();

        // Single.Resources.GetComponentByObject에 의한 비동기 동적로드가 있을 수 있으므로
        // Promise 개념으로 Async-Await 처리를 한다.
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
            m_pSemaphoreSlim.Release(1);
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