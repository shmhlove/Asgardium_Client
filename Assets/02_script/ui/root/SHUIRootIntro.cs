using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootIntro : SHUIRoot
{
    private Action m_pEventClick;

    void Awake()
    {
        Single.UI.AddRoot(typeof(SHUIRootIntro), this);
    }

    public void Show(Action pCallback)
    {
        m_pEventClick = pCallback;
    }

    public void OnClickMoveSceneButton()
    {
        if (null == m_pEventClick)
            return;
            
        m_pEventClick();
    }
}
