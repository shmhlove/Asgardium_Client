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
    Right_Button,
    Left_Button,
    Close_Button
}

public class SHUIAlertInfo
{
    public string m_strTitle = string.Empty;
    public string m_strMessage = string.Empty;
    public string m_strOneBtnLabel = string.Empty;
    public string m_strTwoLeftBtnLabel = string.Empty;
    public string m_strTwoRightBtnLabel = string.Empty;
    public eAlertButtonType m_eButtonType = eAlertButtonType.OneButton;
    public Action<eAlertButtonAction> m_pCallback = (eSelectBtnType) => {};

    public SHUIAlertInfo() { }
    public SHUIAlertInfo(string strMessage, Action<eAlertButtonAction> pCallback = null)
    {
        m_strMessage = strMessage;
        m_pCallback = (null == pCallback) ? (eSelectBtnType) => {} : pCallback;
    }
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
    public UILabel m_pLabelOneButton;
    public UILabel m_pLabelTwoLeftButton;
    public UILabel m_pLabelTwoRightButton;

    [Header("Event")]
    private Action<eAlertButtonAction> m_pCallback;

    public override void OnBeforeShow(params object[] pArgs)
    {
        Single.Coroutine.NextUpdate(async () =>
        {
            var pStringTable = await Single.Table.GetTable<SHTableClientString>();

            var pAlertInfo = (SHUIAlertInfo)pArgs[0];
            m_pLabelTitle.text = pAlertInfo.m_strTitle;

            m_pTextListBody.Clear();
            m_pTextListBody.textLabel.pivot = UIWidget.Pivot.Center;
            m_pTextListBody.Add("[BAC4C4]" + (string)pAlertInfo.m_strMessage + "[-]");

            if (string.IsNullOrEmpty(pAlertInfo.m_strOneBtnLabel))
                m_pLabelOneButton.text = pStringTable.GetString("10010");
            else
                m_pLabelOneButton.text = pAlertInfo.m_strOneBtnLabel;

            if (string.IsNullOrEmpty(pAlertInfo.m_strTwoLeftBtnLabel))
                m_pLabelTwoLeftButton.text = pStringTable.GetString("10010");
            else
                m_pLabelTwoLeftButton.text = pAlertInfo.m_strTwoLeftBtnLabel;

            if (string.IsNullOrEmpty(pAlertInfo.m_strTwoRightBtnLabel))
                m_pLabelTwoRightButton.text = pStringTable.GetString("10011");
            else
                m_pLabelTwoRightButton.text = pAlertInfo.m_strTwoRightBtnLabel;

            switch (pAlertInfo.m_eButtonType)
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

            m_pCallback = pAlertInfo.m_pCallback;
        });
    }

    public override void OnAfterClose(params object[] pArgs)
    {
        if ((null == pArgs) || (1 > pArgs.Length))
        {
            return;
        }
        
        ((Action)pArgs[0])();
    }

    public void OnClickButtonLeft()
    {
        Action pAfterClose = () =>
        {
            m_pCallback(eAlertButtonAction.Left_Button);
        };
        Close(pAfterClose);
    }

    public void OnClickButtonRight()
    {
        Action pAfterClose = () =>
        {
            m_pCallback(eAlertButtonAction.Right_Button);
        };
        Close(pAfterClose);
    }

    public void OnClickButtonClose()
    {
        Action pAfterClose = () =>
        {
            m_pCallback(eAlertButtonAction.Close_Button);
        };
        Close(pAfterClose);
    }
}
