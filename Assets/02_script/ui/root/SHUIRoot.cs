using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRoot : MonoBehaviour
{
	public List<SHUIPanel> panels;

    public virtual void Awake()
    {
        Single.UI.AddRoot(this.GetType(), this);
    }

    public T GetPanel<T>() where T : SHUIPanel
    {
        foreach(var panel in panels)
        {
            if (panel.GetType().Equals(typeof(T)))
              return panel as T;
        }

        // 리소스 동적로드 처리 (싱크방식)

        return default(T);
    }

    public void SetEnableAllPanels(bool isActive)
    {
        foreach(var panel in panels)
        {
            panel.SetActive(isActive);
        }
    }
}
