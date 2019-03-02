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

        var Epsilon = 500;
        var LastMiningPowerAt = m_pUserInfo.MiningPowerAt - Epsilon;

        var timeSpan = (DateTime.UtcNow - SHUtils.GetUCTTimeByMillisecond(LastMiningPowerAt));
        var curPowerCount = (int)(timeSpan.TotalMilliseconds / m_pServerConfig.BasicChargeTime);
        var curLeftTime = (timeSpan.TotalMilliseconds % m_pServerConfig.BasicChargeTime);

        curPowerCount = Math.Min(curPowerCount, m_pServerConfig.BasicMiningPowerCount);
        string strCountInfo = string.Format("{0}/{1}", curPowerCount, m_pServerConfig.BasicMiningPowerCount);

        var pLeftTime = TimeSpan.FromMilliseconds(m_pServerConfig.BasicChargeTime - curLeftTime);
        
        var iLeftMinutes = (int)(pLeftTime.TotalSeconds / 60);
        var iLeftSecond = (int)(pLeftTime.TotalSeconds % 60);
        string strTimer = (curPowerCount < m_pServerConfig.BasicMiningPowerCount) ? string.Format("{0:00}:{1:00}", iLeftMinutes, iLeftSecond) : "--:--";
        m_pUIPanelMining.SetActiveInformation(strCountInfo, strTimer);
    }

    public async void OnClickDebugReset()
    {
        if (null == m_pUserInfo)
        {
            var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
            var pTable = await Single.Table.GetTable<SHTableClientString>();
            pUIRoot.ShowAlert(pTable.GetString("1000"));
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
                // 시간차 문제가 있어 1초 뒤 갱신해주도록 해보았다.
                //Single.Coroutine.WaitTime(1.0f, () => 
                //{
                    m_pUserInfo.LoadJsonTable(reply.data);
                //});
            }
            else
            {
                var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
                pUIRoot.ShowAlert(reply.ToString());
            }
        });
    }

    public async void OnClickDebugUsePower()
    {
        if (null == m_pUserInfo)
        {
            var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
            var pTable = await Single.Table.GetTable<SHTableClientString>();
            pUIRoot.ShowAlert(pTable.GetString("1000"));
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
                // 시간차 문제가 있어 1초 뒤 갱신해주도록 해보았다.
                //Single.Coroutine.WaitTime(1.0f, () => 
                //{
                    m_pUserInfo.LoadJsonTable(reply.data);
                //});
            }
            else
            {
                var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
                pUIRoot.ShowAlert(reply.ToString());
            }
        });
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
                yield return new WaitForSeconds(3.0f);
            }

            if (null == m_pServerConfig)
            {
                ResetServerConfig();
                yield return new WaitForSeconds(3.0f);
            }

            UpdateActiveInformation();

            yield return null;
        }
    }
}
