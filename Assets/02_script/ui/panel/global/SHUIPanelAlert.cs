using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public enum eAlertButtonType
{
    NoButton,
    OneButton,
    TwoButton
}

public enum eAlertButtonAction
{
    Close,
    Ok,
    Cancel
}

public class SHUIPanelAlert : SHUIPanel
{
    [Header("Message")]
    public UILabel m_pLabelTitle;
    public UILabel m_pLabelBody;
    public UITextList m_pTextListBody;

    [Header("Buttons")]
    public GameObject m_pOneButton;
    public GameObject m_pTwoButton;

    [Header("Event")]
    private Action<eAlertButtonAction> m_pCallback;

    public override void OnBeforeShow(params object[] pArgs)
    {
        m_pLabelTitle.text = (string)pArgs[0];
        //m_pLabelBody.text = (string)pArgs[1];

        m_pTextListBody.Clear();
        m_pTextListBody.Add((string)pArgs[1]);

        switch((eAlertButtonType)pArgs[2])
        {
            case eAlertButtonType.NoButton:
                m_pOneButton.SetActive(false);
                m_pTwoButton.SetActive(false);
                break;
            case eAlertButtonType.OneButton:
                m_pOneButton.SetActive(true);
                m_pTwoButton.SetActive(false);
                break;
            case eAlertButtonType.TwoButton:
                m_pOneButton.SetActive(false);
                m_pTwoButton.SetActive(true);
                break;
        }
        m_pCallback = (Action<eAlertButtonAction>)pArgs[3];
    }

    public override void OnAfterClose(params object[] pArgs)
    {
        if ((null == pArgs) || (1 > pArgs.Length))
        {
            return;
        }

        var pCallback = (Action)pArgs[0];
        if (null != pCallback)
        {
            pCallback();
        }
    }

    public void OnClickButton1()
    {
        m_pCallback(eAlertButtonAction.Ok);
    }

    public void OnClickButton2()
    {
        m_pCallback(eAlertButtonAction.Cancel);
    }

    public void OnClickButtonClose()
    {
        m_pCallback(eAlertButtonAction.Close);
    }
}
