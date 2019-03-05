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

    private async void ReloadUserInfo()
    {
        m_pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();

        if (false == m_pUserInfo.m_bIsLoaded)
        {
            m_pUserInfo = null;
        }
    }

    private async void ReloadServerConfig()
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
        var iCurPowerCount = (int)(pTimeSpan.TotalMilliseconds / (double)m_pServerConfig.BasicChargeTime);
        var fCurLeftTime = (pTimeSpan.TotalMilliseconds % (double)m_pServerConfig.BasicChargeTime);

        // 파워갯수 출력형태로 구성
        iCurPowerCount = Math.Min(iCurPowerCount, m_pServerConfig.BasicMiningPowerCount);
        string strCountInfo = string.Format("{0}/{1}", iCurPowerCount, m_pServerConfig.BasicMiningPowerCount);

        // 남은 시간 출력형태로 구성
        var pLeftTime = TimeSpan.FromMilliseconds(m_pServerConfig.BasicChargeTime - fCurLeftTime);
        var iLeftMinutes = (int)(pLeftTime.TotalSeconds / 60);
        var iLeftSecond = (int)(pLeftTime.TotalSeconds % 60);
        string strTimer = (iCurPowerCount < m_pServerConfig.BasicMiningPowerCount) ? 
            string.Format("{0:00}:{1:00}", iLeftMinutes, iLeftSecond) : "--:--";

        // UI 업데이트
        m_pUIPanelMining.SetActiveInformation(strCountInfo, strTimer);
    }

    private void UpdateActiveScrollview()
    {
        // 참조 해야할 테이블리스트
        // 1. 인스턴스 마이닝 액티브 테이블 oracle_company_am과 동일한 구조
        /*
            resource_id -> asgardium_resource_data참조
            ==>> name_strid -> 회사이름
            emblem_id -> 회사앰블럼인데 현재 무시옵션
            ==>> efficiency_lv -> 광물 레벨
            ==>> supply_lv -> 공급량 레벨
        */
        // 2. asgardium_resource_data 테이블
        /*
            name_strid -> 무시
            ==>> icon_name -> 광물 아이콘 파일이름
            ==>> value -> 채굴시 번개 소모량
            rid_fuel1 -> 무시
            rid_fuel2 -> 무시
        */

        // 여기서 데이터 조립해서 Mining Panel UI에 던져주자.
        // public class SHActiveSlotData
        // {
        //     public string m_strActiveUID; 인스턴스 마이닝 액티브 테이블의 UID

        //     public string m_strCompanyName; 인스턴스 마이닝 액티브 테이블의 name_strid 필드
        //     public string m_strCompanyIcon; 인스턴스 마이닝 액티브 테이블의 emblem_id 필드
        //     public string m_iResourceIcon; 인스턴스 마이닝 액티브 테이블의 resource_id필드를 이용해서 asgardium_resource_data 테이블의 icon_name 필드 참고
        //     public int m_iResourceQuantity; 인스턴스 마이닝 액티브 테이블의 Efficiency_lv을 이용해서 active_mining_quantity에서 quantity 필드
        //     public int m_iResourceLevel; 인스턴스 마이닝 액티브 테이블의 Efficiency_lv 필드
        //     public int m_iSupplyQuantity; 인스턴스 마이닝 액티브 테이블의 supply_lv를 참조하여 active_mining_supply 테이블에 찾기(단, 기본 회사일 경우 ServerConfig테이블의 basic_active_mining_supply 필드 사용)
        //     public int m_iPurchaseCost; asgardium_resource_data 테이블의 value 필드

        //     public Action<string> m_pEventPurchaseButton;
        // }

        // dictionary로 관리하고, 정렬 후 List로 뽑아서 던져주기
        // List로 뽑을때는 Efficiency_lv기준으로 정렬하고, 레벨이 같을때는 공급량을 기준으로 정렬(다중정렬)
        // SHActiveSlotData
        // m_pUIPanelMining.SetActiveScrollview();

        // 2.
        // 스크롤뷰 구성 - 출력해야할 리스트 서버로 부터 받아 슬롯구성할 수 있도록 스크롤뷰클래스에 전달

        // 1.
        // 서버가 켜질때 마이닝셋 테이블에 기본 회사를 넣어준다.
        // 일단 GET 으로 마이닝셋 테이블을 가져갈 수 있도록 추가
        // 클라는 코루틴 돌면서 마이닝셋 테이블을 Web통신으로 가져가자
        // 정상동작 확인되면 웹소켓으로 변경해보자.

        // 3.
        // 필터동작시 이벤트 받아 스크롤뷰 재구성, 필터내용은 저장하여 켜질때 그대로 셋팅될 수 있도록

    }

    public void OnEventOfChangeMiningStage(eMiningStageType eType)
    {
        // 맞을 때
        if (eType == eMiningStageType.Active)
        {
            StartCoroutine(CoroutineForMiningActiveScrollview());
        }

        // 아닐 때
        if (eType != eMiningStageType.Active)
        {
            StopCoroutine(CoroutineForMiningActiveScrollview());
        }
    }
    
    public void OnEventOfPurchaseMining(string strActiveUID)
    {

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
                ReloadUserInfo();
                yield return new WaitForSeconds(0.5f);
            }

            if (null == m_pServerConfig)
            {
                ReloadServerConfig();
                yield return new WaitForSeconds(0.5f);
            }

            UpdateActiveInformation();

            yield return null;
        }
    }

    [Obsolete("This Coroutine is Deprecated When run socket.io")]
    private IEnumerator CoroutineForMiningActiveScrollview()
    {
        while (true)
        {
            if (null == m_pUIPanelMining)
            {
                yield return null;
            }

            UpdateActiveScrollview();

            yield return new WaitForSeconds(1.0f);
        }
    }

    //////////////////////////////////////////////////////////////////////
    // 테스트 코드
    //////////////////////////////////////////////////////////////////////
    public async void OnClickDebugReset()
    {
        if (null == m_pUserInfo)
        {
            var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
            var pTable = await Single.Table.GetTable<SHTableClientString>();
            await pUIRoot.ShowAlert(pTable.GetString("1000"));
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
                await pUIRoot.ShowAlert(reply.ToString());
            }
        });
    }

    public async void OnClickDebugUsePower()
    {
        if (null == m_pUserInfo)
        {
            var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
            var pTable = await Single.Table.GetTable<SHTableClientString>();
            await pUIRoot.ShowAlert(pTable.GetString("1000"));
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
                await pUIRoot.ShowAlert(reply.ToString());
            }
        });
    }
}
