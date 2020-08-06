using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class SHBusinessGlobal_Alert : SHBusinessPresenter
{
    private bool m_bIsRunningCoroutineShowAlert = false;
    private List<SHUIAlertInfo> m_pAlertInfoPool = new List<SHUIAlertInfo>();

    public void Show(string strMessage, Action<eAlertButtonAction> pCallback = null)
    {
        Show(new SHUIAlertInfo(strMessage, pCallback));
    }

    public async void Show(SHUIAlertInfo pAlertInfo)
    {
        m_pAlertInfoPool.Add(pAlertInfo);

        if (false == m_bIsRunningCoroutineShowAlert)
        {
            var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
            Single.Coroutine.Coroutine(CoroutineShowAlertUI(await pUIRoot.GetAlert()), ()=>{});
        }
    }

    public async void Show(SHReply pReply, Action<eAlertButtonAction> pCallback = null)
    {
        var errorMessage = string.Empty;
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();
        
        if (false == pReply.isSucceed)
        {
            switch(pReply.error.code)
            {
                case (int)eErrorCode.Net_Common_HTTP:
                    // Network Module 에서 처리...
                    return;
                case (int)eErrorCode.Common_Need_login:
                case (int)eErrorCode.Server_Common_InvalidAccessToken:
                    errorMessage = pStringTable.GetString("1000");
                    break;
                case (int)eErrorCode.Server_Auth_AlreadySignupUser:
                    errorMessage = pStringTable.GetString("1003");
                    break;
                case (int)eErrorCode.Server_Auth_NoSignupUser:
                    errorMessage = pStringTable.GetString("1004");
                    break;
                case (int)eErrorCode.Server_Auth_NoMatchPassword:
                    errorMessage = pStringTable.GetString("1005");
                    break;
                case (int)eErrorCode.Auth_InvalidEmail:
                    errorMessage = pStringTable.GetString("1001");
                    break;
                case (int)eErrorCode.Server_Mining_ZeroSupplyQuantity:
                    errorMessage = pStringTable.GetString("1010");
                    break;
                case (int)eErrorCode.Server_Mining_NotEnoughMiningPower:
                    errorMessage = pStringTable.GetString("1011");
                    break;
                case (int)eErrorCode.Server_Upgrade_MaxLevel:
                    errorMessage = pStringTable.GetString("1012");
                    break;
                case (int)eErrorCode.Server_Upgrade_NotEnoughGold:
                    errorMessage = pStringTable.GetString("1013");
                    break;
                default:
                    errorMessage = pStringTable.GetString("1002");
                    break;
            }
        }

        if (string.IsNullOrEmpty(errorMessage))
        {
            Show(new SHUIAlertInfo(pReply.ToString(), pCallback));
        }
        else
        {
            Show(new SHUIAlertInfo(string.Format("{0}({1})", errorMessage, pReply.error.code), pCallback));
        }
    }

    public IEnumerator CoroutineShowAlertUI(SHUIPanelAlert pUIAlert)
    {
        m_bIsRunningCoroutineShowAlert = true;

        while(0 < m_pAlertInfoPool.Count)
        {
            var isDone = false;
            var pAlertInfo = m_pAlertInfoPool[0];
            var pCopyCallback = pAlertInfo.m_pCallback;
            pAlertInfo.m_pCallback = (eSelectBtnType) =>
            {
                pCopyCallback(eSelectBtnType);
                m_pAlertInfoPool.Remove(pAlertInfo);
                isDone = true;
            };

            pUIAlert.Show(pAlertInfo);

            while (false == isDone)
                yield return null;
        }

        m_bIsRunningCoroutineShowAlert = false;
    }
}