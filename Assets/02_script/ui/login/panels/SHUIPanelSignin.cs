using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelSignin : SHUIPanel 
{
    [Header("User Information")]
    public UIInput inputEmail;
    public UIInput inputPass;
    public UIToggle toggleSave;
    
    private Action<string, string, bool> m_pEventSignin;
    private Action<string, string> m_pEventSignup;

    public override void OnBeforeShow(params object[] pArgs)
    {
        if ((null == pArgs) || (2 > pArgs.Length))
            return;

        m_pEventSignin = ((Action<string, string, bool>)pArgs[0]);
        m_pEventSignup = ((Action<string, string>)pArgs[1]);
        inputEmail.value = (string)pArgs[2];
        inputPass.value = (string)pArgs[3];
        
        if (null != (bool?)pArgs[4])
        {
            toggleSave.value = ((bool?)pArgs[4]).Value;
        }
    }

	public void OnClickSigninButton()
	{
        if (null == m_pEventSignin)
        {
            return;
        }

        m_pEventSignin(inputEmail.value.Trim(), inputPass.value, toggleSave.value);
	}
    
    public void OnClickSignupButton()
	{
        if (null == m_pEventSignup)
        {
            return;
        }

        m_pEventSignup(inputEmail.value.Trim(), inputPass.value);
	}
}