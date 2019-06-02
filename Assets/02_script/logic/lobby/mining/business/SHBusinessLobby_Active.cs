using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public partial class SHBusinessLobby : MonoBehaviour
{
    // Key : "Level_UnitId", Value : SlotData in the same group as Level and UnitId
    private Dictionary<string, List<SHActiveSlotData>> m_dicActiveCompanyData = new Dictionary<string, List<SHActiveSlotData>>();

    private void SetChangeMiningTab(eMiningTabType eType)
    {
        // On
        if ((eMiningTabType.Active == eType)
            && (eMiningTabType.Active != m_eCurrentMiningTabType))
        {
            RequestSubscribeMiningActiveInfo();

            m_dicActiveCompanyData.Clear();
            UpdateDataForActiveCompany();

            StopCoroutine("CoroutineForUpdateUIForActiveCompany");
            StartCoroutine("CoroutineForUpdateUIForActiveCompany");
        }

        // Off
        if ((eMiningTabType.Active != eType)
            && (eMiningTabType.Active == m_eCurrentMiningTabType))
        {
            RequestUnsubscribeMiningActiveInfo();

            StopCoroutine("CoroutineForUpdateUIForActiveCompany");
        }

        m_eCurrentMiningTabType = eType;
    }

    private async void UpdateUIForActiveInformation(Action pCallback)
    {
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
        var pServerGlobalConfig = await Single.Table.GetTable<SHTableServerGlobalConfig>();

        // 통신시간 갭 때문에 저장된 시간보다 3초정도 앞당겨 준다.
        var Epsilon = 3000;
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

    private void UpdateUIForActiveCompany(Action pCallback)
    {
        if (0 == m_dicActiveCompanyData.Count)
        {
            pCallback();
            return;
        }

        List<SHActiveSlotData> pSlotDatas = new List<SHActiveSlotData>();
        foreach (var kvp in m_dicActiveCompanyData)
        {
            var pData = kvp.Value.FindAll((p) => { return 0 != p.m_iSupplyQuantity; });
            if (null != pData)
            {
                pData[0].m_bIsSubItems = (1 < pData.Count);
                pSlotDatas.Add(pData[0]);
            }
            else
            {
                kvp.Value[0].m_bIsSubItems = false;
                pSlotDatas.Add(kvp.Value[0]);
            }
        }

        var m_pFilters = new List<Func<SHActiveSlotData, SHActiveSlotData, bool?>>
        {
            SortConditionForEfficiencyLevel,
            SortConditionForUnitID
        };
        pSlotDatas.Sort((x, y) =>
        {
            foreach (var pFilter in m_pFilters)
            {
                bool? result = pFilter(x, y);
                if (null != result)
                {
                    return result.Value ? 1 : -1;
                }
            }

            return 1;
        });

        m_pUIPanelMining.SetActiveScrollview(pSlotDatas);
        
        pCallback();
    }

    private async void UpdateDataForActiveCompany()
    {
        var pCompanyTable = await Single.Table.GetTable<SHTableServerInstanceMiningActiveCompany>();
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();
        var pServerGlobalUnitDataTable = await Single.Table.GetTable<SHTableServerGlobalUnitData>();
        var pActiveMiningQuantityTable = await Single.Table.GetTable<SHTableServerMiningActiveQuantity>();

        Func<SHTableServerInstanceMiningActiveCompanyData, SHActiveSlotData> pCreateSlotData = 
            (SHTableServerInstanceMiningActiveCompanyData pData) =>
            {
                return new SHActiveSlotData()
                {
                    m_strSlotId = string.Format("{0}_{1}", pData.m_iEfficiencyLV, pData.m_iUnitId),
                    m_strInstanceId = pData.m_strInstanceId,

                    m_strCompanyName = pStringTable.GetString(pData.m_iNameStrid.ToString()),
                    m_strCompanyIcon = pData.m_strEmblemImage,
                    m_strResourceIcon = pServerGlobalUnitDataTable.GetData(pData.m_iUnitId).m_strIconImage,
                    m_iResourceQuantity = pActiveMiningQuantityTable.GetData(pData.m_iEfficiencyLV).m_iQuantity,
                    m_iPurchaseCost = pServerGlobalUnitDataTable.GetData(pData.m_iUnitId).m_iWeight,

                    m_iUnitId = pData.m_iUnitId,
                    m_iEfficiencyLevel = pData.m_iEfficiencyLV,
                    m_iSupplyQuantity = pData.m_iSupplyCount,

                    m_pEventPurchaseButton = OnEventForPurchaseMining,
                    m_pEventShowSubItemsButton = OnEventForShowSubItems,
                };
            };

        if (0 == m_dicActiveCompanyData.Count)
        {
            foreach (var kvp in pCompanyTable.m_dicDatas)
            {
                var pData = pCreateSlotData(kvp.Value);

                if (false == m_dicActiveCompanyData.ContainsKey(pData.m_strSlotId))
                {
                    m_dicActiveCompanyData[pData.m_strSlotId] = new List<SHActiveSlotData>();
                }

                m_dicActiveCompanyData[pData.m_strSlotId].Add(pData);
            }
        }
        else
        {
            foreach (var kvp in m_dicActiveCompanyData)
            {
                foreach (var pSlotData in kvp.Value)
                {
                    var pTableData = pCompanyTable.GetData(pSlotData.m_strInstanceId);
                    if (null == pTableData)
                    {
                        pSlotData.m_iSupplyQuantity = 0;
                    }
                    else
                    {
                        var pData = pCreateSlotData(pTableData);
                        pSlotData.CopyFrom(pData);
                    }
                }
            }
        }
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

    public void OnEventForShowSubItems(string strSlotId)
    {
        if (null == m_pUIPanelMiningSubActiveCompany)
            return;

        if (true == m_dicActiveCompanyData.ContainsKey(strSlotId))
        {
            m_pUIPanelMiningSubActiveCompany.Show(m_dicActiveCompanyData[strSlotId]);
        }
        else
        {
            m_pUIPanelMiningSubActiveCompany.Show(new List<SHActiveSlotData>());
        }
    }

    private async void OnEventForSocketPollingMiningActiveInfo(SHReply pReply)
    {
        var pCompanyTable = await Single.Table.GetTable<SHTableServerInstanceMiningActiveCompany>();
        pCompanyTable.LoadJsonTable(pReply.data);
        UpdateDataForActiveCompany();
    }
    
    private IEnumerator CoroutineForUpdateUIForActiveInformation()
    {
        while (true)
        {
            if (m_pUIPanelMining)
            {
                bool isDone = false;
                UpdateUIForActiveInformation(() => isDone = true);

                while (false == isDone)
                    yield return null;
            }
            
            yield return null;
        }
    }

    private IEnumerator CoroutineForUpdateUIForActiveCompany()
    {
        while (true)
        {
            if (m_pUIPanelMining)
            {
                bool isDone = false;
                UpdateUIForActiveCompany(() => isDone = true);
                
                while (false == isDone)
                    yield return null;
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
