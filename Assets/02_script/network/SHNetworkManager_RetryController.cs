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

    private bool StopRetryProcess()
    {
        // 웹서버와 웹소켓 모두 연결중일때
        if ((false == IsWebServerConnected()) || (false == IsWebSocketConnected())) {
                // 대기타야 함
            }

        ClearRetryInfo();
        StopCoroutine("CoroutineRetryProcess");
        return true;
    }

    private void ClearRetryInfo()
    {
        m_iRetryCount = 0;
        m_bIsProcessingRetry = false;
    }

    private IEnumerator CoroutineRetryProcess()
    {
        Single.BusinessGlobal.ShowIndicator();

        var strErrorMessage = string.Empty;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            strErrorMessage = m_pStringTable.GetString("1006");
        }
        else
        {
            strErrorMessage = m_pStringTable.GetString("1007");
        }

        while (m_iRetryCount++ < m_iMaxRetryCount)
        {
            // 자동 재시도 처리
            var strRetryInfo = string.Format(m_pStringTable.GetString("1008"), m_iRetryCount, m_iMaxRetryCount);
            Single.BusinessGlobal.UpdateIndicatorMessage(string.Format("{0}\n{1}", strRetryInfo, strErrorMessage));

            StartCoroutine(CoroutineRetryWebServerProcess());
            StartCoroutine(CoroutineRetryWebSocketProcess());

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

        Single.BusinessGlobal.CloseIndicator();
        Single.BusinessGlobal.ShowAlertUI(pAlertInfo);
    }
}