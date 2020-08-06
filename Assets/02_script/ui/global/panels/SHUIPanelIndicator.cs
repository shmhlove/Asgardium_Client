using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelIndicator : SHUIPanel
{
    public UILabel m_pMessageLabel;
    public UISprite m_pMessageBG;
    
    public override void OnBeforeShow(params object[] pArgs)
    {
        SetMessage(m_pMessageLabel.text);
    }
    
    public override void OnAfterClose(params object[] pArgs)
    {
        SetMessage(string.Empty);
    }

    public void SetMessage(string strMessage)
    {
        m_pMessageLabel.text = strMessage;
        m_pMessageBG.gameObject.SetActive(false == string.IsNullOrEmpty(strMessage));
    }
}
