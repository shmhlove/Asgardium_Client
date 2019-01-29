using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHSceneLogin : MonoBehaviour
{
    private SHUIRootLogin m_pUIRoot = null;

    private void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    private void Start()
    {
        SetUIRoot(() => 
        {
            ShowLoginPanel();
        });
    }

    private void SetUIRoot(Action pCallback)
    {
        Single.UI.GetRoot<SHUIRootLogin>(SHUIConstant.ROOT_LOGIN, (pUIRoot) => 
        {
            m_pUIRoot = pUIRoot;
            pCallback();
        });
    }

    private void ShowLoginPanel()
    {
        if (null == m_pUIRoot)
        {
            Debug.LogError("[LSH] not set ui-root object in LoginScene");
            return;
        }

        m_pUIRoot.CloseSignupPanel();
        m_pUIRoot.ShowLoginPanel(OnClickLogin, OnClickSignup);
    }

    private void ShowSignupPanel()
    {
        if (null == m_pUIRoot)
        {
            Debug.LogError("[LSH] not set ui-root object in LoginScene");
            return;
        }

        m_pUIRoot.CloseLoginPanel();
        m_pUIRoot.ShowSignupPanel(OnClickRegistrationUser, OnClickGoBackLogin);
    }

    private void OnClickLogin(string strEmail, string strPassword)
    {
        JsonData json = new JsonData();
        json["name"] = strEmail;
        json["pass"] = strPassword;
        Single.Network.POST(SHAPIs.SH_API_LOGIN, json, (reply) =>
        {
            if (reply.isSucceed)
            {
            }
            else
            {
            }
        });
    }

    private void OnClickSignup()
    {
        ShowSignupPanel();
    }

    private void OnClickRegistrationUser(string strEmail, string strPassword)
    {

    }

    private void OnClickGoBackLogin()
    {
        ShowLoginPanel();
    }
}