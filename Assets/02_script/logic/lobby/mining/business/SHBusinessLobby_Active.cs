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
        var pInventory = await Single.Table.GetTable<SHTableServerInventoryInfo>();
        var pServerGlobalConfig = await Single.Table.GetTable<SHTableServerGlobalConfig>();

        // 통신시간 갭 때문에 저장된 시간보다 1초정도 앞당겨 준다.
        var Epsilon = 1000;
        var LastMiningPowerAt = pInventory.MiningPowerAt - Epsilon;

        // 남은 시간과 파워갯수 구하기
        var pTimeSpan = (DateTime.UtcNow - SHUtils.GetUctTimeByMillisecond(LastMiningPowerAt));
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

        var pSlotDatas = new List<SHActiveSlotData>();
        foreach (var kvp in m_dicActiveCompanyData)
        {
            var pData = kvp.Value.FindAll((p) => { return 0 != p.m_iSupplyQuantity; });
            if ((null != pData) && (0 != pData.Count))
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
        (pData) =>
        {
            return new SHActiveSlotData()
            {
                // GroupID
                m_strGroupId = string.Format("{0}_{1}", pData.m_iEfficiencyLV, pData.m_iUnitId),
                // 회사 인스턴스 ID
                m_strInstanceId = pData.m_strInstanceId,
                // 회사 이름
                m_strCompanyName = pStringTable.GetString(pData.m_iNameStrid.ToString()),
                // 회사 아이콘
                m_strCompanyIcon = pData.m_strEmblemImage,
                // 유닛 아이콘
                m_strUnitIcon = pServerGlobalUnitDataTable.GetData(pData.m_iUnitId).m_strIconImage,
                // 구매시 획득 유닛 물량
                m_iUnitQuantity = pActiveMiningQuantityTable.GetData(pData.m_iEfficiencyLV).m_iQuantity,
                // 구매 가격
                m_iPurchaseCost = pServerGlobalUnitDataTable.GetData(pData.m_iUnitId).m_iWeight,
                // 유닛 ID
                m_iUnitId = pData.m_iUnitId,
                // 회사 레벨
                m_iEfficiencyLevel = pData.m_iEfficiencyLV,
                // 구매 가능한 공급 물량
                m_iSupplyQuantity = pData.m_iSupplyCount,
                // NPC 회사여부
                m_bIsNPCCompany = pData.m_bIsNPCCompany,
                // 구매버튼 이벤트
                m_pEventPurchaseButton = OnEventForPurchaseMining,
                // 서브유닛 확인 버튼
                m_pEventShowSubUnitsButton = OnEventForShowSubUnits,
            };
        };

        if (0 == m_dicActiveCompanyData.Count)
        {
            // 서버 데이터를 기준으로 클라 데이터 신규 셋팅
            foreach (var kvp in pCompanyTable.m_dicDatas)
            {
                var pData = pCreateSlotData(kvp.Value);

                if (false == m_dicActiveCompanyData.ContainsKey(pData.m_strGroupId))
                {
                    m_dicActiveCompanyData[pData.m_strGroupId] = new List<SHActiveSlotData>();
                }

                m_dicActiveCompanyData[pData.m_strGroupId].Add(pData);
            }

            // NPC 회사를 가장 뒤로 보내기
            foreach (var kvp in m_dicActiveCompanyData)
            {
                kvp.Value.Sort((x, y) =>
                {
                    // x가 기본회사이면 x를 뒤로 보내기
                    if ((true == x.m_bIsNPCCompany) && (false == y.m_bIsNPCCompany))
                        return 1;
                    // y가 기본회사이면 y를 뒤로 보내기
                    if ((false == x.m_bIsNPCCompany) && (true == y.m_bIsNPCCompany))
                        return -1;
                    // 둘다 기본회사이면 자리변경 없음
                    return 0;
                });
            }
        }
        else
        {
            // 클라 데이터를 기준으로 서버 데이터 업데이트
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
        Single.Network.SendRequestSocket(SHAPIs.SH_SOCKET_REQ_SUBSCRIBE_MINING_ACTIVE_INFO, json, (reply) => { });
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
        Single.Network.SendRequestSocket(SHAPIs.SH_SOCKET_REQ_UNSUBSCRIBE_MINING_ACTIVE_INFO, json, (reply) => { });
    }

    private async void RequestPurchaseMiningActiveCompany(string strInstanceId)
    {
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
        if (false == pUserInfo.IsLogin())
        {
            Single.BusinessGlobal.ShowAlertUI(pStringTable.GetString("1000"));
            return;
        }

        JsonData json = new JsonData
        {
            ["user_id"] = pUserInfo.UserId,
            ["active_company_instance_id"] = strInstanceId
        };
        Single.Network.POST(SHAPIs.SH_API_MINING_PURCHASE_ACTIVE, json, async (reply) =>
        {
            var pInventory = await Single.Table.GetTable<SHTableServerInventoryInfo>();
            pInventory.LoadJsonTable(reply.data);

            if (reply.isSucceed)
            {
                // 서버 소켓 데이터가 오기전 UI에 빠르게 반영해주기 위해...
                foreach (var kvp in m_dicActiveCompanyData)
                {
                    var pData = kvp.Value.Find((p) => { return strInstanceId == p.m_strInstanceId; });
                    if (null != pData)
                    {
                        pData.m_iSupplyQuantity = Math.Max(pData.m_iSupplyQuantity - 1, 0);
                        
                        if (m_pUIPanelMiningSubActiveCompany.IsActive())
                        {
                            m_pUIPanelMiningSubActiveCompany.Show(m_dicActiveCompanyData[pData.m_strGroupId]);
                        }
                        break;
                    }
                }

                UpdateUIForActiveCompany(() => { });
            }
            else
            {
                switch (reply.errorCode)
                {
                    case eErrorCode.Server_Mining_ZeroSupplyQuantity:
                        Single.BusinessGlobal.ShowAlertUI(pStringTable.GetString("1010"));
                        break;
                    case eErrorCode.Server_Mining_NotEnoughMiningPower:
                        Single.BusinessGlobal.ShowAlertUI(pStringTable.GetString("1011"));
                        break;
                    default:
                        Single.BusinessGlobal.ShowAlertUI(reply);
                        break;
                }
            }
        });
    }

    public void OnEventForChangeMiningTab(eMiningTabType eType)
    {
        SetChangeMiningTab(eType);
    }

    public void OnEventForPurchaseMining(string strInstanceId)
    {
        RequestPurchaseMiningActiveCompany(strInstanceId);
    }

    public void OnEventForShowSubUnits(string strGroupId)
    {
        if (true == m_dicActiveCompanyData.ContainsKey(strGroupId))
        {
            m_pUIPanelMiningSubActiveCompany.Show(m_dicActiveCompanyData[strGroupId]);
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
        var pInventoryInfo = await Single.Table.GetTable<SHTableServerInventoryInfo>();

        JsonData json = new JsonData
        {
            ["user_id"] = pUserInfo.UserId
        };
        Single.Network.POST(SHAPIs.SH_API_TEST_RESET_POWER, json, (reply) => 
        {
            if (reply.isSucceed)
            {
                pInventoryInfo.LoadJsonTable(reply.data);
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
        var pInventoryInfo = await Single.Table.GetTable<SHTableServerInventoryInfo>();

        JsonData json = new JsonData
        {
            ["user_id"] = pUserInfo.UserId
        };

        Single.Network.POST(SHAPIs.SH_API_TEST_USE_POWER, json, (reply) => 
        {
            if (reply.isSucceed)
            {
                pInventoryInfo.LoadJsonTable(reply.data);
            }
            else
            {
                Single.BusinessGlobal.ShowAlertUI(reply);
            }
        });
    }
}
