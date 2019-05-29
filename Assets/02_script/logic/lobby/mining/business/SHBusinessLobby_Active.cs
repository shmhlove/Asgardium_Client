using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public partial class SHBusinessLobby : MonoBehaviour
{
    private void SetChangeMiningTab(eMiningTabType eType)
    {
        // Initialize
        StopCoroutine("CoroutineForMiningActiveCompany");

        // On
        if ((eMiningTabType.Active == eType)
            && (eMiningTabType.Active != m_eCurrentMiningTabType))
        {
            RequestSubscribeMiningActiveInfo();
            StartCoroutine("CoroutineForMiningActiveCompany");
        }

        // Off
        if ((eMiningTabType.Active != eType)
            && (eMiningTabType.Active == m_eCurrentMiningTabType))
        {
            RequestUnsubscribeMiningActiveInfo();
        }

        m_eCurrentMiningTabType = eType;
    }

    private async void UpdateActiveInformation(Action pCallback)
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

        pCallback();
    }

    private async void UpdateActiveCompany(Action pCallback)
    {
        var pCompanyTable = await Single.Table.GetTable<SHTableServerInstanceMiningActiveCompany>();
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();
        var pServerGlobalUnitData = await Single.Table.GetTable<SHTableServerGlobalUnitData>();
        var pActiveMiningQuantityTable = await Single.Table.GetTable<SHTableServerMiningActiveQuantity>();

        var pSlotDatas = new List<SHActiveSlotData>();
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
            pData.m_pEventPurchaseButton = OnEventForPurchaseMining;
            pSlotDatas.Add(pData);
        }

        pSlotDatas.Sort((x, y) =>
        {
            return (x.m_iResourceLevel == y.m_iResourceLevel) ? 
                ((x.m_iSupplyQuantity < y.m_iSupplyQuantity) ? 1 : -1) : ((x.m_iResourceLevel < y.m_iResourceLevel) ? 1 : -1);
        });

        // UI 업데이트
        m_pUIPanelMining.SetActiveScrollview(pSlotDatas);
        pCallback();
    }

    private async void RequestSubscribeMiningActiveInfo()
    {
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
        if (false == pUserInfo.IsLogin()) {
            var pStringTable = await Single.Table.GetTable<SHTableClientString>();
            Single.BusinessGlobal.ShowAlertUI(pStringTable.GetString("1000"));
            return;
        }

        JsonData json = new JsonData
        {
            ["user_id"] = pUserInfo.UserId
        };
        Single.Network.SendRequestSocket(SHAPIs.SH_SOCKET_REQ_SUBSCRIBE_MINING_ACTIVE_INFO, json, (reply) =>
        {
            if (reply.isSucceed)
            {
                //pUserInfo.LoadJsonTable(reply.data);
            }
            else
            {
                Single.BusinessGlobal.ShowAlertUI(reply);
            }
        });
    }

    private async void RequestUnsubscribeMiningActiveInfo()
    {
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
        if (false == pUserInfo.IsLogin()) {
            var pStringTable = await Single.Table.GetTable<SHTableClientString>();
            Single.BusinessGlobal.ShowAlertUI(pStringTable.GetString("1000"));
            return;
        }
        
        JsonData json = new JsonData
        {
            ["user_id"] = pUserInfo.UserId
        };
        Single.Network.SendRequestSocket(SHAPIs.SH_SOCKET_REQ_UNSUBSCRIBE_MINING_ACTIVE_INFO, json, (reply) =>
        {
            if (reply.isSucceed)
            {
                //pUserInfo.LoadJsonTable(reply.data);
            }
            else
            {
                Single.BusinessGlobal.ShowAlertUI(reply);
            }
        });
    }

    public void OnEventForChangeMiningTab(eMiningTabType eType)
    {
        SetChangeMiningTab(eType);
    }

    public void OnEventForPurchaseMining(string strInstanceId)
    {
        Single.BusinessGlobal.ShowAlertUI(string.Format("채굴 요청 : {0}", strInstanceId));
    }

    private async void OnEventForSocketPollingMiningActiveInfo(SHReply pReply)
    {
        var pCompanyTable = await Single.Table.GetTable<SHTableServerInstanceMiningActiveCompany>();
        pCompanyTable.LoadJsonTable(pReply.data);
    }
    
    private IEnumerator CoroutineForMiningActiveInformation()
    {
        while (true)
        {
            if (m_pUIPanelMining)
            {
                bool isDone = false;
                UpdateActiveInformation(() => isDone = true);

                while (false == isDone)
                    yield return null;
            }
            
            yield return null;
        }
    }

    private IEnumerator CoroutineForMiningActiveCompany()
    {
        while (true)
        {
            if (m_pUIPanelMining)
            {
                bool isDone = false;
                UpdateActiveCompany(() => isDone = true);
                
                while (false == isDone)
                    yield return null;
            }
            
            yield return null;
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
