using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelSignup : SHUIPanel 
{
    [Header("User Information")]
    public UIInput inputId;
    public UIInput inputPass;

    private Action<string, string> m_pEventRegistration;
    private Action m_pEventGoBackLogin;

    public override void OnBeforeShow(params object[] pArgs)
    {
        if ((null == pArgs) || (2 > pArgs.Length))
            return;
        
        m_pEventRegistration  = ((Action<string, string>)pArgs[0]);
        m_pEventGoBackLogin = ((Action)pArgs[1]);
    }

	public void OnClickRegistrationButton()
	{
		if (null == m_pEventRegistration)
        {
            return;
        }

        m_pEventRegistration(inputId.value, inputPass.value);
	}

	public void OnClickGoBackLoginButton()
	{
		if (null == m_pEventGoBackLogin)
        {
            return;
        }

        m_pEventGoBackLogin();
	}
}
