using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using LitJson;
using socket.io;

public partial class SHNetworkManager : SHSingleton<SHNetworkManager>
{
    private int m_iRetryCount = 0;
    private int m_iMaxRetryCount = 5;
    private float m_fRetryDelay = 1.5f;
    private bool m_bIsProcessingRetry = false;
    private bool m_bIsProcessingStopRetry = false;

    private SHTableClientString m_pStringTable;

    private async void StartRetryProcess()
    {
        if (true == m_bIsProcessingRetry)
            return;

        m_iRetryCount = 0;
        m_bIsProcessingRetry = true;

        if (null == m_pStringTable)
        {
            m_pStringTable = await Single.Table.GetTable<SHTableClientString>();
        }
        
        StartCoroutine("CoroutineRetryProcess");
    }

    private void StopRetryProcess()
    {
        if (true == m_bIsProcessingStopRetry)
            return;
        
        StartCoroutine(CoroutineCheckStopRetry(() => 
        {
            ClearRetryInfo();
            Single.Global.GetIndicator().Close();
            StopCoroutine("CoroutineRetryProcess");
        }));
    }

    private void ClearRetryInfo()
    {
        m_iRetryCount = 0;
        m_bIsProcessingRetry = false;
    }

    private IEnumerator CoroutineCheckStopRetry(Action pCallback)
    {
        m_bIsProcessingStopRetry = true;

        while ((false == IsWebServerConnected()) || (false == IsWebSocketConnected()))
        {
            yield return null;
        }

        m_bIsProcessingStopRetry = false;

        pCallback();
    }

    private IEnumerator CoroutineRetryProcess()
    {
        Single.Global.GetIndicator().Show();

        var strErrorMessage = string.Empty;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            strErrorMessage = m_pStringTable.GetString("1006");
        }
        else
        {
            strErrorMessage = m_pStringTable.GetString("1007");
        }

        // 자동 재시도 처리 (m_iMaxRetryCount 만큼 반복)
        while (m_iRetryCount++ < m_iMaxRetryCount)
        {
            var strRetryInfo = string.Format(m_pStringTable.GetString("1008"), m_iRetryCount, m_iMaxRetryCount);
            Single.Global.GetIndicator().UpdateMessage(string.Format("{0}\n{1}", strRetryInfo, strErrorMessage));

            StartCoroutine(CoroutineRetryWebServerProcess());
            ProcessRetryWebSocketConnect();
            
            yield return new WaitForSeconds(m_fRetryDelay);
        }
        
        // 수동 재시도 처리
        var pAlertInfo = new SHUIAlertInfo();
        pAlertInfo.m_strMessage = string.Format("{0}\n{1}", strErrorMessage, m_pStringTable.GetString("1009"));
        pAlertInfo.m_strTwoLeftBtnLabel = m_pStringTable.GetString("10012");
        pAlertInfo.m_strTwoRightBtnLabel = m_pStringTable.GetString("10013");
        pAlertInfo.m_eButtonType = eAlertButtonType.TwoButton;
        pAlertInfo.m_pCallback = (eSelectBtnType) => 
        {
            if (eAlertButtonAction.Left_Button == eSelectBtnType)
            {
                ClearRetryInfo();
                StartRetryProcess();
            }
            else
            {
                SHUtils.GameQuit();
            }
        };

        Single.Global.GetIndicator().Close();
        Single.Global.GetAlert().Show(pAlertInfo);
    }
}