using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelIndicator : SHUIPanel
{
    public UILabel m_pMessageLabel;
    
    public override void OnAfterClose(params object[] pArgs)
    {
        SetMessage(string.Empty);
    }

    public void SetMessage(string strMessage)
    {
        m_pMessageLabel.text = strMessage;
    }
}
