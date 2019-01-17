using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelLogin : SHUIPanel 
{
    public UIInput inputId;
    public UIInput inputPass;
    
    public Action<string, string> OnLoginAction;
    public Action<string, string> OnSigninAction;
    public Action OnloseAction;

	public void OnClickLoginButton()
	{
        if (null != OnLoginAction)
        {
            OnLoginAction(inputId.value, inputPass.value);
        }
	}
    
    public void OnClickSigninButton()
	{
        if (null != OnSigninAction)
        {
            OnSigninAction(inputId.value, inputPass.value);
        }
	}

    public void OnClickCloseButton()
    {
        if (null != OnloseAction)
        {
            OnloseAction();
        }
    }
}
