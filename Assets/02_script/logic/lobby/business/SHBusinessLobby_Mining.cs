using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public partial class SHBusinessLobby : MonoBehaviour
{
    private SHTableUserInfo m_pUserInfo = null;
    private SHTableServerConfig m_pServerConfig = null;

    // 액티브
    // 필터동작시 이벤트 받아 스크롤뷰 재구성, 필터내용은 저장하여 켜질때 그대로 셋팅될 수 있도록
    // 스크롤뷰 구성 - 출력해야할 리스트 서버로 부터 받아 슬롯구성할 수 있도록 스크롤뷰클래스에 전달

    // 패시브

    // 컴퍼니

    private async void ResetUserInfo()
    {
        m_pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();

        if (false == m_pUserInfo.m_bIsLoaded)
        {
            m_pUserInfo = null;
        }
    }

    private async void ResetServerConfig()
    {
        m_pServerConfig = await Single.Table.GetTable<SHTableServerConfig>();

        if (false == m_pServerConfig.m_bIsLoaded)
        {
            m_pServerConfig = null;
        }
    }

    private void UpdateActiveInformation()
    {
        if ((null == m_pUserInfo) || (null == m_pServerConfig))
            return;

        var LastMiningPowerAt = m_pUserInfo.MiningPowerAt;

        var timeSpan = (DateTime.UtcNow - SHUtils.GetUCTTimeByMillisecond(LastMiningPowerAt));
        var curPowerCount = (int)(timeSpan.TotalMilliseconds / m_pServerConfig.BasicChargeTime);
        var curPowerremindTime = (timeSpan.TotalMilliseconds % m_pServerConfig.BasicChargeTime);

        curPowerCount = Math.Min(curPowerCount, m_pServerConfig.BasicMiningPower);
        string strCountInfo = string.Format("{0}/{1}", curPowerCount, m_pServerConfig.BasicMiningPower);

        var pRemindTime = TimeSpan.FromMilliseconds(m_pServerConfig.BasicChargeTime - curPowerremindTime);
        
        var iRemindMinutes = (int)(pRemindTime.TotalSeconds / 60);
        var iRemindSecond = (int)(pRemindTime.TotalSeconds % 60);
        string strTimer = (curPowerCount < m_pServerConfig.BasicMiningPower) ? string.Format("{0:00}:{1:00}", iRemindMinutes, iRemindSecond) : "--:--";
        m_pUIPanelMining.SetActiveInformation(strCountInfo, strTimer);
    }

    public async void OnClickDebugReset()
    {
        if (null == m_pUserInfo)
        {
            var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
            pUIRoot.ShowAlert("로그인이 안되어 있어서 지금 통신못함.");
            return;
        }

        JsonData json = new JsonData
        {
            ["user_id"] = m_pUserInfo.UserId
        };
        Single.Network.POST(SHAPIs.SH_API_TEST_RESET_POWER, json, async (reply) => 
        {
            if (reply.isSucceed)
            {
                m_pUserInfo.LoadJsonTable(reply.data);
            }
            else
            {
                var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
                pUIRoot.ShowAlert(reply.ToString());
            }
        });
        
        // var milliSeconds = SHUtils.GetTotalMillisecondsForUTC() - (m_pServerConfig.BasicChargeTime * m_pServerConfig.BasicMiningPower);
        // m_pUserInfo.MiningPowerAt = milliSeconds;
    }

    public async void OnClickDebugUsePower()
    {
        if (null == m_pUserInfo)
        {
            var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
            pUIRoot.ShowAlert("로그인이 안되어 있어서 지금 통신못함.");
            return;
        }

        JsonData json = new JsonData
        {
            ["user_id"] = m_pUserInfo.UserId
        };
        Single.Network.POST(SHAPIs.SH_API_TEST_USE_POWER, json, async (reply) => 
        {
            if (reply.isSucceed)
            {
                m_pUserInfo.LoadJsonTable(reply.data);
            }
            else
            {
                var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
                pUIRoot.ShowAlert(reply.ToString());
            }
        });
        
        //m_pUserInfo.MiningPowerAt = m_pUserInfo.MiningPowerAt + m_pServerConfig.BasicChargeTime;
    }

    private IEnumerator CoroutineForMiningActive()
    {
        while (true)
        {
            if (null == m_pUIPanelMining)
            {
                yield return null;
            }

            if (null == m_pUserInfo)
            {
                ResetUserInfo();
                yield return null;
            }

            if (null == m_pServerConfig)
            {
                ResetServerConfig();
                yield return null;
            }

            UpdateActiveInformation();

            yield return null;
        }
    }
}
