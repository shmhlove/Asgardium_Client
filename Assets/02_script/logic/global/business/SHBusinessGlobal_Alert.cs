using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public partial class SHBusinessGlobal : SHSingleton<SHBusinessGlobal>
{
    private bool m_bIsRunningCoroutineShowAlert = false;
    private List<SHUIAlertInfo> m_pAlertInfoPool = new List<SHUIAlertInfo>();

    public void ShowAlertUI(string strMessage, Action<eAlertButtonAction> pCallback = null)
    {
        ShowAlertUI(new SHUIAlertInfo(strMessage, pCallback));
    }

    public async void ShowAlertUI(SHUIAlertInfo pAlertInfo)
    {
        m_pAlertInfoPool.Add(pAlertInfo);

        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);

        if (false == m_bIsRunningCoroutineShowAlert)
        {
            StartCoroutine(CoroutineShowAlertUI(pUIRoot));
        }
    }

    public async void ShowAlertUI(SHReply pReply, Action<eAlertButtonAction> pCallback = null)
    {
        var errorMessage = string.Empty;
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();
        
        if (false == pReply.isSucceed)
        {
            switch(pReply.error.code)
            {
                case eErrorCode.Net_Common_HTTP:
                    // Network Module 에서 처리...
                    return;
                case eErrorCode.Net_Common_InvalidParameter:
                case eErrorCode.Net_Common_FailedGetCollection:
                case eErrorCode.Net_Common_FailedFindCollection:
                case eErrorCode.Net_Common_FailedWriteDocument:
                case eErrorCode.Net_Common_InvalidResponseData:
                case eErrorCode.Net_Common_JsonParse:
                case eErrorCode.Net_Common_EmptyCollection:
                case eErrorCode.Net_Common_InvalidHeader:
                    errorMessage = pStringTable.GetString("1002");
                    break;
                case eErrorCode.Net_Auth_AlreadySignupUser:
                    errorMessage = pStringTable.GetString("1003");
                    break;
                case eErrorCode.Net_Auth_NoSignupUser:
                    errorMessage = pStringTable.GetString("1004");
                    break;
                case eErrorCode.Net_Auth_NoMatchPassword:
                    errorMessage = pStringTable.GetString("1005");
                    break;
                case eErrorCode.Auth_InvalidEmail:
                    errorMessage = pStringTable.GetString("1001");
                    break;
            }
        }

        if (string.IsNullOrEmpty(errorMessage))
        {
            ShowAlertUI(new SHUIAlertInfo(pReply.ToString(), pCallback));
        }
        else
        {
            ShowAlertUI(new SHUIAlertInfo(string.Format("{0}({1})", errorMessage, pReply.error.code), pCallback));
        }
    }

    public IEnumerator CoroutineShowAlertUI(SHUIRootGlobal pUIRoot)
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
            pUIRoot.ShowAlert(pAlertInfo);

            while (false == isDone)
                yield return null;
        }

        m_bIsRunningCoroutineShowAlert = false;
    }
}