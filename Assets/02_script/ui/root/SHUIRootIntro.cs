using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIRootIntro : SHUIRoot
{
    private Action m_pCallback;

    public void Show(Action pCallback)
    {
        m_pCallback = pCallback;
    }

    public void OnClickButton()
    {
        m_pCallback();
    }
}
