using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public partial class SHBusinessLobby : MonoBehaviour
{
    private void SetChangeMiningStage(eMiningStageType eType)
    {
        // 맞을 때
        if (eType == eMiningStageType.Active)
        {
            // 서버에게 마이닝 회사 정보 업데이트 승인 요청
            StartCoroutine("CoroutineForMiningActiveScrollview");
        }

        // 아닐 때
        if (eType != eMiningStageType.Active)
        {
            // 서버에게 마이닝 회사 정보 업데이트 취소 요청
            StopCoroutine("CoroutineForMiningActiveScrollview");
        }
    }

    private async void UpdateActiveInformation()
    {
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
        var pServerGlobalConfig = await Single.Table.GetTable<SHTableServerGlobalConfig>();

        var Epsilon = 500;
        var LastMiningPowerAt = pUserInfo.MiningPowerAt - Epsilon;

        // 남은 시간과 파워갯수 구하기
        var pTimeSpan = (DateTime.UtcNow - SHUtils.GetUCTTimeByMillisecond(LastMiningPowerAt));
        var iCurPowerCount = (int)(pTimeSpan.TotalMilliseconds / (double)pServerGlobalConfig.m_iBasicChargeTime);
        var fCurLeftTime = (pTimeSpan.TotalMilliseconds % (double)pServerGlobalConfig.m_iBasicChargeTime);

        // 파워갯수 출력형태로 구성
        iCurPowerCount = Math.Min(iCurPowerCount, pServerGlobalConfig.m_iBasicMiningPowerCount);
        string strCountInfo = string.Format("{0}/{1}", iCurPowerCount, pServerGlobalConfig.m_iBasicMiningPowerCount);

        // 남은 시간 출력형태로 구성
        var pLeftTime = TimeSpan.FromMilliseconds(pServerGlobalConfig.m_iBasicChargeTime - fCurLeftTime);
        var iLeftMinutes = (int)(pLeftTime.TotalSeconds / 60);
        var iLeftSecond = (int)(pLeftTime.TotalSeconds % 60);
        string strTimer = (iCurPowerCount < pServerGlobalConfig.m_iBasicMiningPowerCount) ? 
            string.Format("{0:00}:{1:00}", iLeftMinutes, iLeftSecond) : "--:--";

        // UI 업데이트
        m_pUIPanelMining.SetActiveInformation(strCountInfo, strTimer);
    }

    private async void UpdateActiveScrollview()
    {
        var pCompanyTable = await Single.Table.GetTable<SHTableServerInstanceMiningActiveCompany>();
        //pCompanyTable.LoadServerTable(async (errorCode) => 
        //{
            // if (eErrorCode.Succeed != errorCode)
            //     return;

            var pStringTable = await Single.Table.GetTable<SHTableClientString>();
            var pServerGlobalUnitData = await Single.Table.GetTable<SHTableServerGlobalUnitData>();
            var pActiveMiningQuantityTable = await Single.Table.GetTable<SHTableServerMiningActiveQuantity>();

            List<SHActiveSlotData> pSlotDatas = new List<SHActiveSlotData>();
            foreach (var kvp in pCompanyTable.m_dicDatas)
            {
                var pData = new SHActiveSlotData();
                pData.m_strInstanceId = kvp.Value.m_strInstanceId;
                pData.m_strCompanyName = pStringTable.GetString(kvp.Value.m_iNameStrid.ToString());
                pData.m_strCompanyIcon = kvp.Value.m_strEmblemImage;
                pData.m_strResourceIcon = pServerGlobalUnitData.GetData(kvp.Value.m_iUnitId).m_strIconImage;
                pData.m_iResourceQuantity = pActiveMiningQuantityTable.GetData(kvp.Value.m_iEfficiencyLV).m_iQuantity;
                pData.m_iResourceLevel = kvp.Value.m_iEfficiencyLV;
                pData.m_iSupplyQuantity = kvp.Value.m_iSupplyCount;
                pData.m_iPurchaseCost = pServerGlobalUnitData.GetData(kvp.Value.m_iUnitId).m_iWeight;
                pData.m_pEventPurchaseButton = OnEventOfPurchaseMining;
                pSlotDatas.Add(pData);
            }

            pSlotDatas.Sort((x, y) =>
            {
                return (x.m_iResourceLevel == y.m_iResourceLevel) ? 
                    ((x.m_iSupplyQuantity < y.m_iSupplyQuantity) ? 1 : -1) : ((x.m_iResourceLevel < y.m_iResourceLevel) ? 1 : -1);
            });

            m_pUIPanelMining.SetActiveScrollview(pSlotDatas);
        //});
    }

    public void OnEventOfChangeMiningStage(eMiningStageType eType)
    {
        SetChangeMiningStage(eType);
    }

    public void OnEventOfPurchaseMining(string strInstanceId)
    {
        Single.BusinessGlobal.ShowAlertUI(string.Format("채굴 요청 : {0}", strInstanceId));
    }

    private IEnumerator CoroutineForMiningActiveInformation()
    {
        while (true)
        {
            if (m_pUIPanelMining)
            {
                UpdateActiveInformation();
            }
            
            yield return null;
        }
    }
    
    [Obsolete("This Coroutine is Deprecated When run socket.io")]
    private IEnumerator CoroutineForMiningActiveScrollview()
    {
        while (true)
        {
            if (m_pUIPanelMining)
            {
                UpdateActiveScrollview();
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    //////////////////////////////////////////////////////////////////////
    // 테스트 코드
    //////////////////////////////////////////////////////////////////////
    public async void OnClickDebugReset()
    {
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();

        JsonData json = new JsonData
        {
            ["user_id"] = pUserInfo.UserId
        };
        Single.Network.POST(SHAPIs.SH_API_TEST_RESET_POWER, json, (reply) => 
        {
            if (reply.isSucceed)
            {
                pUserInfo.LoadJsonTable(reply.data);
            }
            else
            {
                Single.BusinessGlobal.ShowAlertUI(reply);
            }
        });
    }

    public async void OnClickDebugUsePower()
    {
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();

        JsonData json = new JsonData
        {
            ["user_id"] = pUserInfo.UserId
        };

        Single.Network.POST(SHAPIs.SH_API_TEST_USE_POWER, json, (reply) => 
        {
            if (reply.isSucceed)
            {
                pUserInfo.LoadJsonTable(reply.data);
            }
            else
            {
                Single.BusinessGlobal.ShowAlertUI(reply);
            }
        });
    }
}
