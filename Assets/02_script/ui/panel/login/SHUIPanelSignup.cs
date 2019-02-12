using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelSignup : SHUIPanel 
{
    [Header("User Information")]
    public UIInput inputEmail;
    public UIInput inputName;
    public UIInput inputPass;

    private Action<string, string, string> m_pEventRegistration;
    private Action<string, string, string> m_pEventGoBackLogin;

    public override void OnBeforeShow(params object[] pArgs)
    {
        if ((null == pArgs) || (2 > pArgs.Length))
            return;
        
        m_pEventRegistration  = ((Action<string, string, string>)pArgs[0]);
        m_pEventGoBackLogin = ((Action<string, string, string>)pArgs[1]);
        inputEmail.value = (string)pArgs[2];
        inputName.value = "";
        inputPass.value = "";
    }

	public void OnClickRegistrationButton()
	{
		if (null == m_pEventRegistration)
        {
            return;
        }

        m_pEventRegistration(inputEmail.value.Trim(), inputName.value.Trim(), inputPass.value);
	}

	public void OnClickGoBackLoginButton()
	{
		if (null == m_pEventGoBackLogin)
        {
            return;
        }

        m_pEventGoBackLogin(inputEmail.value.Trim(), inputName.value.Trim(), inputPass.value);
	}
}
