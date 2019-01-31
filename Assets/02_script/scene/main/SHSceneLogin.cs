using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHSceneLogin : MonoBehaviour
{
    private void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    private void Start()
    {
        ShowLoginPanel();
    }

    private void ShowLoginPanel(string strEmail = "")
    {
        var pUIRoot = Single.UI.GetRoot<SHUIRootLogin>(SHUIConstant.ROOT_LOGIN);
        pUIRoot.CloseSignupPanel();
        pUIRoot.ShowLoginPanel(OnClickLogin, OnClickSignup, strEmail);
    }

    private void ShowSignupPanel(string strEmail = "", string strName = "")
    {
        var pUIRoot = Single.UI.GetRoot<SHUIRootLogin>(SHUIConstant.ROOT_LOGIN);
        pUIRoot.CloseLoginPanel();
        pUIRoot.ShowSignupPanel(OnClickRegistrationUser, OnClickGoBackLogin, strEmail, strName);
    }

    private void OnClickLogin(string strEmail, string strPassword)
    {
        if (false == SHUtils.IsValidEmail(strEmail))
        {
            Single.UI.GetGlobalRoot().ShowAlert("올바른 이메일 형식이 아닙니다.");
            return;
        }

        JsonData json = new JsonData();
        json["email"] = strEmail;
        json["password"] = strPassword;
        Single.Network.POST(SHAPIs.SH_API_LOGIN, json, (reply) =>
        {
            Single.UI.GetGlobalRoot().ShowAlert(reply.ToString(), () => 
            {
                if (reply.isSucceed)
                {
                    Single.Scene.LoadScene(eSceneType.Lobby, bIsUseFade:true);
                }
            });
        });
    }

    private void OnClickSignup(string strEmail, string strPassword)
    {
        ShowSignupPanel(strEmail);
    }

    private void OnClickRegistrationUser(string strEmail, string strName, string strPassword)
    {
        if (false == SHUtils.IsValidEmail(strEmail))
        {
            Single.UI.GetGlobalRoot().ShowAlert("올바른 이메일 형식이 아닙니다.");
            return;
        }

        JsonData json = new JsonData();
        json["email"] = strEmail;
        json["name"] = strName;
        json["password"] = strPassword;
        Single.Network.POST(SHAPIs.SH_API_SIGNUP, json, (reply) =>
        {
            Single.UI.GetGlobalRoot().ShowAlert(reply.ToString(), () => 
            {
                if (reply.isSucceed)
                {
                    ShowLoginPanel(strEmail);
                }
            });
        });
    }

    private void OnClickGoBackLogin(string strEmail, string strName, string strPassword)
    {
        ShowLoginPanel(strEmail);
    }
}