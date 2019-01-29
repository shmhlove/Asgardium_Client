using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelLogin : SHUIPanel 
{
    [Header("User Information")]
    public UIInput inputId;
    public UIInput inputPass;
    
    private Action<string, string> m_pEventLogin;
    private Action m_pEventSignup;

    public override void OnBeforeShow(params object[] pArgs)
    {
        if ((null == pArgs) || (2 > pArgs.Length))
            return;
        
        m_pEventLogin  = ((Action<string, string>)pArgs[0]);
        m_pEventSignup = ((Action)pArgs[1]);
    }

	public void OnClickLoginButton()
	{
        if (null == m_pEventLogin)
        {
            return;
        }

        m_pEventLogin(inputId.value, inputPass.value);
	}
    
    public void OnClickSigninButton()
	{
        if (null == m_pEventSignup)
        {
            return;
        }

        m_pEventSignup();
	}
}
