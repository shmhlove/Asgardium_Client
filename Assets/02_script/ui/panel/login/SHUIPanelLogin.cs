using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelLogin : SHUIPanel 
{
    [Header("User Information")]
    public UIInput inputEmail;
    public UIInput inputPass;
    
    private Action<string, string> m_pEventLogin;
    private Action<string, string> m_pEventSignup;

    public override void OnBeforeShow(params object[] pArgs)
    {
        if ((null == pArgs) || (2 > pArgs.Length))
            return;
        
        m_pEventLogin  = ((Action<string, string>)pArgs[0]);
        m_pEventSignup = ((Action<string, string>)pArgs[1]);

        if (3 <= pArgs.Length)
        {
            inputEmail.value = (string)pArgs[2];
        }

        inputPass.value = "";
    }

	public void OnClickLoginButton()
	{
        if (null == m_pEventLogin)
        {
            return;
        }

        m_pEventLogin(inputEmail.value, inputPass.value);
	}
    
    public void OnClickSigninButton()
	{
        if (null == m_pEventSignup)
        {
            return;
        }

        m_pEventSignup(inputEmail.value, inputPass.value);
	}
}
