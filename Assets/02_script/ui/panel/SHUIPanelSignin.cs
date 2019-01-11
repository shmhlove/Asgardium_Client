using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUIPanelSignin : SHUIPanel 
{
    public UIInput inputId;
    public UIInput inputPass;
    
	public void OnClickSigninButton()
	{
		Debug.Log("OnClickSigninButton");
	}

    public void OnClickCloseButton()
    {
        Debug.Log("OnClickCloseButton");
    }
}
