using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHBusinessLogin : MonoBehaviour
{
    [Header("UI Objects")]
    private SHUIPanelSignin m_pUIPanelSignin = null;
    private SHUIPanelSignup m_pUIPanelSignup = null;

    void Awake()
    {
        Single.AppInfo.CreateSingleton();
    }

    async void Start()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLogin>(SHUIConstant.ROOT_LOGIN);
        m_pUIPanelSignin = await pUIRoot.GetPanel<SHUIPanelSignin>(SHUIConstant.PANEL_SIGNIN);
        m_pUIPanelSignup = await pUIRoot.GetPanel<SHUIPanelSignup>(SHUIConstant.PANEL_SIGNUP);

        ShowSigninPanel(SHPlayerPrefs.GetString("auth_email"), 
                        SHPlayerPrefs.GetString("auth_password"),
                        SHPlayerPrefs.GetBool("auth_is_save"));
    }

    private void ShowSigninPanel(string strEmail, string strPass, bool? bIsSave)
    {
        m_pUIPanelSignup.Close();

        Action<string, string, bool> EventLogin = OnClickLogin;
        Action<string, string> EventSignup = OnClickSignup;

        m_pUIPanelSignin.Show(EventLogin, EventSignup, strEmail, strPass, bIsSave);
    }

    private void ShowSignupPanel(string strEmail)
    {
        m_pUIPanelSignin.Close();

        Action<string, string, string> EventRegistrationUser = OnClickRegistrationUser;
        Action<string, string, string> EventGoBackLogin = OnClickGoBackLogin;

        m_pUIPanelSignup.Show(EventRegistrationUser, EventGoBackLogin, strEmail);
    }

    private async void OnClickLogin(string strEmail, string strPassword, bool bIsSave)
    {
        if (false == SHUtils.IsValidEmail(strEmail))
        {
            var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
            var pTable = await Single.Table.GetTable<SHTableClientString>();
            await pUIRoot.ShowAlert(pTable.GetString("1001"));
            return;
        }

        JsonData json = new JsonData
        {
            ["email"] = strEmail,
            ["password"] = strPassword
        };
        Single.Network.POST(SHAPIs.SH_API_SIGNIN, json, async (reply) =>
        {
            if (reply.isSucceed)
            {
                var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
                pUserInfo.LoadJsonTable(reply.data);
                
                SHPlayerPrefs.SetString("auth_email", bIsSave ? pUserInfo.UserEmail : string.Empty);
                SHPlayerPrefs.SetString("auth_password", bIsSave ? pUserInfo.Password : string.Empty);
                SHPlayerPrefs.SetBool("auth_is_save", bIsSave);
                SHPlayerPrefs.Save();
            }
            
            var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
            await pUIRoot.ShowAlert(reply.ToString(), async () => 
            {
                if (reply.isSucceed)
                {
                    await Single.Scene.LoadScene(eSceneType.Lobby, bIsUseFade: true);
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
            var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
            var pTable = await Single.Table.GetTable<SHTableClientString>();
            await pUIRoot.ShowAlert(pTable.GetString("1001"));
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
            var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
            await pUIRoot.ShowAlert(reply.ToString(), () => 
            {
                if (reply.isSucceed)
                {
                    ShowSigninPanel(strEmail, "", null);
                }
            });
        });
    }

    private void OnClickGoBackLogin(string strEmail, string strName, string strPassword)
    {
        ShowSigninPanel(strEmail, "", null);
    }
}