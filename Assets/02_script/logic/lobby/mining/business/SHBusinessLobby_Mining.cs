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

    // 여기서 데이터 조립해서 UI에 던져주자.
    // SHActiveSlotData

    // 서버에서 아이디값으로 통신할 것이므로 필요한것
    // 기본 광물정보
    // 기본 회사정보

    // 액티브

    // 2.
    // 스크롤뷰 구성 - 출력해야할 리스트 서버로 부터 받아 슬롯구성할 수 있도록 스크롤뷰클래스에 전달

    // 1.
    // 서버가 켜질때 마이닝셋 테이블에 기본 회사를 넣어준다.
    // 일단 GET 으로 마이닝셋 테이블을 가져갈 수 있도록 추가
    // 클라는 코루틴 돌면서 마이닝셋 테이블을 Web통신으로 가져가자
    // 정상동작 확인되면 웹소켓으로 변경해보자.

    // 3.
    // 필터동작시 이벤트 받아 스크롤뷰 재구성, 필터내용은 저장하여 켜질때 그대로 셋팅될 수 있도록

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

        // 남은 시간과 파워갯수 구하기
        var pTimeSpan = (DateTime.UtcNow - SHUtils.GetUCTTimeByMillisecond(LastMiningPowerAt));
        var iCurPowerCount = (int)(pTimeSpan.TotalMilliseconds / m_pServerConfig.BasicChargeTime);
        var iCurLeftTime = (pTimeSpan.TotalMilliseconds % m_pServerConfig.BasicChargeTime);

        // 파워갯수 출력형태로 구성
        iCurPowerCount = Math.Min(iCurPowerCount, m_pServerConfig.BasicMiningPowerCount);
        string strCountInfo = string.Format("{0}/{1}", iCurPowerCount, m_pServerConfig.BasicMiningPowerCount);

        // 남은 시간 출력형태로 구성
        var pLeftTime = TimeSpan.FromMilliseconds(m_pServerConfig.BasicChargeTime - iCurLeftTime);
        var iLeftMinutes = (int)(pLeftTime.TotalSeconds / 60);
        var iLeftSecond = (int)(pLeftTime.TotalSeconds % 60);
        string strTimer = (iCurPowerCount < m_pServerConfig.BasicMiningPowerCount) ? string.Format("{0:00}:{1:00}", iLeftMinutes, iLeftSecond) : "--:--";

        // UI 업데이트
        m_pUIPanelMining.SetActiveInformation(strCountInfo, strTimer);
    }

    private void UpdateActiveScrollview()
    {
        // 소켓통신 전 테스트용으로 3초 간격으로 코루틴 돌자.
        // dictionary로 관리하고, 정렬 후 List로 뽑아서 던져주기
        // SHActiveSlotData
        // m_pUIPanelMining.SetActiveScrollview();
    }

    public void OnEventOfPurchaseMining(string strActiveUID)
    {

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

    private IEnumerator CoroutineForMiningActiveInformation()
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
                yield return new WaitForSeconds(0.5f);
            }

            if (null == m_pServerConfig)
            {
                ResetServerConfig();
                yield return new WaitForSeconds(0.5f);
            }

            UpdateActiveInformation();

            yield return null;
        }
    }

    private IEnumerator CoroutineForMiningActiveScrollview()
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
                yield return new WaitForSeconds(0.5f);
            }

            if (null == m_pServerConfig)
            {
                ResetServerConfig();
                yield return new WaitForSeconds(0.5f);
            }

            UpdateActiveInformation();

            yield return null;
        }
    }
}
