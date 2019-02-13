using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelLogin : SHUIPanel 
{
    [Header("User Information")]
    public UIInput inputEmail;
    public UIInput inputPass;
    public UIToggle toggleSave;
    
    private Action<string, string, bool> m_pEventLogin;
    private Action<string, string> m_pEventSignup;

    public override void OnBeforeShow(params object[] pArgs)
    {
        if ((null == pArgs) || (2 > pArgs.Length))
            return;
        
        m_pEventLogin  = ((Action<string, string, bool>)pArgs[0]);
        m_pEventSignup = ((Action<string, string>)pArgs[1]);
        inputEmail.value = (string)pArgs[2];
        inputPass.value = (string)pArgs[3];
        
        if (null != (bool?)pArgs[4])
        {
            toggleSave.value = ((bool?)pArgs[4]).Value;
        }
    }

	public void OnClickLoginButton()
	{
        if (null == m_pEventLogin)
        {
            return;
        }

        m_pEventLogin(inputEmail.value.Trim(), inputPass.value, toggleSave.value);
	}
    
    public void OnClickSigninButton()
	{
        if (null == m_pEventSignup)
        {
            return;
        }

        m_pEventSignup(inputEmail.value.Trim(), inputPass.value);
	}

    public void OnClickGETWebTest()
    {
        Single.Network.GET(SHAPIs.SH_API_TEST, null, (reply) => 
        {
            Debug.LogError(reply.rawResponse.ToJson());
        });
    }

    public void OnClickPOSTWebTest()
    {
        Single.Network.POST(SHAPIs.SH_API_TEST, null, (reply) => 
        {
            Debug.LogError(reply.rawResponse.ToJson());
        });
    }
}