using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHBusinessLogin : MonoBehaviour
{
    private void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    private void Start()
    {
        ShowLoginPanel(
            SHPlayerPrefs.GetString("auth_email"), 
            SHPlayerPrefs.GetString("auth_password"),
            (0 == SHPlayerPrefs.GetInt("auth_is_save")) ? (bool?)null : (1 == SHPlayerPrefs.GetInt("auth_is_save")));
    }

    private async void ShowLoginPanel(string strEmail, string strPass, bool? bIsSave)
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLogin>(SHUIConstant.ROOT_LOGIN);
        pUIRoot.CloseSignupPanel();

        Action<string, string, bool> EventLogin = OnClickLogin;
        Action<string, string> EventSignup = OnClickSignup;

        pUIRoot.ShowLoginPanel(EventLogin, EventSignup, strEmail, strPass, bIsSave);
    }

    private async void ShowSignupPanel(string strEmail)
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLogin>(SHUIConstant.ROOT_LOGIN);
        pUIRoot.CloseLoginPanel();

        Action<string, string, string> EventRegistrationUser = OnClickRegistrationUser;
        Action<string, string, string> EventGoBackLogin = OnClickGoBackLogin;

        pUIRoot.ShowSignupPanel(EventRegistrationUser, EventGoBackLogin, strEmail);
    }

    private async void OnClickLogin(string strEmail, string strPassword, bool bIsSave)
    {
        if (false == SHUtils.IsValidEmail(strEmail))
        {
            var pUIRoot = await Single.UI.GetGlobalRoot();
            pUIRoot.ShowAlert("올바른 이메일 형식이 아닙니다.");
            return;
        }

        JsonData json = new JsonData
        {
            ["email"] = strEmail,
            ["password"] = strPassword
        };
        Single.Network.POST(SHAPIs.SH_API_LOGIN, json, async (reply) =>
        {
            if (reply.isSucceed)
            {
                var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
                pUserInfo.LoadJsonTable(reply.data);
                
                SHPlayerPrefs.SetString("auth_email", bIsSave ? pUserInfo.UserEmail : string.Empty);
                SHPlayerPrefs.SetString("auth_password", bIsSave ? pUserInfo.Password : string.Empty);
                SHPlayerPrefs.SetInt("auth_is_save", bIsSave ? 1 : 2);
                SHPlayerPrefs.Save();
            }
            
            var pUIRoot = await Single.UI.GetGlobalRoot();
            pUIRoot.ShowAlert(reply.ToString(), () => 
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

    private async void OnClickRegistrationUser(string strEmail, string strName, string strPassword)
    {
        if (false == SHUtils.IsValidEmail(strEmail))
        {
            var pUIRoot = await Single.UI.GetGlobalRoot();
            pUIRoot.ShowAlert("올바른 이메일 형식이 아닙니다.");
            return;
        }

        JsonData json = new JsonData
        {
            ["email"] = strEmail,
            ["name"] = strName,
            ["password"] = strPassword
        };
        Single.Network.POST(SHAPIs.SH_API_SIGNUP, json, async (reply) =>
        {
            var pUIRoot = await Single.UI.GetGlobalRoot();
            pUIRoot.ShowAlert(reply.ToString(), () => 
            {
                if (reply.isSucceed)
                {
                    ShowLoginPanel(strEmail, "", null);
                }
            });
        });
    }

    private void OnClickGoBackLogin(string strEmail, string strName, string strPassword)
    {
        ShowLoginPanel(strEmail, "", null);
    }
}