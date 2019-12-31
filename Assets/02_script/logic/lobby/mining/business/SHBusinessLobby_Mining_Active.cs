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

    private void EnableMiningActiveTab()
    {
        RequestSubscribeMiningActiveInfo();

        m_dicActiveCompanyData.Clear();
        UpdateDataForActiveCompany();
        UpdateUIForActiveFilterbar();

        StopCoroutine("CoroutineForUpdateUIForActiveCompany");
        StartCoroutine("CoroutineForUpdateUIForActiveCompany");
    }

    private void DisableMiningActiveTab()
    {
        RequestUnsubscribeMiningActiveInfo();

        StopCoroutine("CoroutineForUpdateUIForActiveCompany");
    }

    private async void UpdateUIForActiveInformation(Action pCallback)
    {
        var pInventory = await Single.Table.GetTable<SHTableServerUserInventory>();
        var pUpgrade = await Single.Table.GetTable<SHTableServerUserUpgradeInfo>();
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

    private async void UpdateUIForActiveFilterbar()
    {
        var bIsAllOn = true;
        var pSlotDatas = new List<SHActiveFilterUnitData>();
        var pUnitTable = await Single.Table.GetTable<SHTableServerGlobalUnit>();

        foreach (var kvp in pUnitTable.m_dicDatas)
        {
            var bIsOn = SHPlayerPrefs.GetBool(kvp.Value.m_iUnitId.ToString());
            bIsOn = (null == bIsOn) ? true : bIsOn.Value;
            if (false == bIsOn)
            {
                bIsAllOn = false;
                continue;
            }

            var pData = new SHActiveFilterUnitData();
            pData.m_iUnitId = kvp.Value.m_iUnitId;
            pData.m_strIconImage = kvp.Value.m_strIconImage;

            pSlotDatas.Add(pData);
        }

        m_pUIPanelMining.SetActiveFilterbarScrollview(pSlotDatas, bIsAllOn);
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
            // 유닛 필터링 체크
            var keySplit = kvp.Key.Split('_');
            var bIsOn = SHPlayerPrefs.GetBool(keySplit[1]);
            if (false == ((null == bIsOn) ? true : bIsOn.Value))
            {
                continue;
            }

            // 공급량 체크
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

        var m_pSortFilters = new List<Func<SHActiveSlotData, SHActiveSlotData, bool?>>
        {
            SortConditionForEfficiencyLevel,
            SortConditionForUnitID
        };
        pSlotDatas.Sort((x, y) =>
        {
            foreach (var pFilter in m_pSortFilters)
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
        var pServerGlobalUnitDataTable = await Single.Table.GetTable<SHTableServerGlobalUnit>();
        var pActiveMiningQuantityTable = await Single.Table.GetTable<SHTableServerMiningActiveQuantity>();

        Func<SHTableServerInstanceMiningActiveCompanyData, SHActiveSlotData> pCreateSlotData =
        (pData) =>
        {
            return new SHActiveSlotData()
            {
                m_strGroupId = string.Format("{0}_{1}", pData.m_iEfficiencyLV, pData.m_iUnitId),
                m_strInstanceId = pData.m_strInstanceId,
                m_strCompanyName = pStringTable.GetString(pData.m_iNameStrid.ToString()),
                m_strCompanyIcon = pData.m_strEmblemImage,
                m_strUnitIcon = pServerGlobalUnitDataTable.GetData(pData.m_iUnitId).m_strIconImage,
                m_iUnitId = pData.m_iUnitId,
                m_iUnitQuantity = pActiveMiningQuantityTable.GetData(pData.m_iEfficiencyLV).m_iQuantity,
                m_iPurchaseCost = pServerGlobalUnitDataTable.GetData(pData.m_iUnitId).m_iWeight,
                m_iEfficiencyLevel = pData.m_iEfficiencyLV,
                m_iSupplyQuantity = pData.m_iSupplyCount,
                m_bIsNPCCompany = pData.m_bIsNPCCompany,
                
                m_pEventPurchaseButton = OnEventForPurchaseMining,
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
    
    private void RequestSubscribeMiningActiveInfo()
    {
        Single.Network.SendRequestSocket(SHAPIs.SH_SOCKET_REQ_SUBSCRIBE_MINING_ACTIVE_INFO, null, (reply) => { });
    }

    private void RequestUnsubscribeMiningActiveInfo()
    {
        Single.Network.SendRequestSocket(SHAPIs.SH_SOCKET_REQ_UNSUBSCRIBE_MINING_ACTIVE_INFO, null, (reply) => { });
    }

    private void RequestPurchaseMiningActiveCompany(string strInstanceId)
    {
        JsonData json = new JsonData
        {
            ["active_company_instance_id"] = strInstanceId
        };
        Single.Network.POST(SHAPIs.SH_API_MINING_PURCHASE_ACTIVE, json, async (reply) =>
        {
            var pInventory = await Single.Table.GetTable<SHTableServerUserInventory>();
            pInventory.LoadJsonTable(reply.data);

            if (reply.isSucceed)
            {
                // 갱신된 소켓 데이터가 오기전 UI에 빠르게 선반영해주기 위해 미리 UI에 반영해준다.
                // 부작용으로 실제 소켓 데이터가 도착하면 UI상 임시 데이터가 순간 변할 수 있다.
                var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
                var pPanel = await pUIRoot.GetPanel<SHUIPopupPanelMiningSubActiveCompany>(SHUIConstant.PANEL_MINING_SUB_ACTIVE_COMPANY);

                foreach (var kvp in m_dicActiveCompanyData)
                {
                    var pData = kvp.Value.Find((p) => { return strInstanceId == p.m_strInstanceId; });
                    if (null != pData)
                    {
                        pData.m_iSupplyQuantity = Math.Max(pData.m_iSupplyQuantity - 1, 0);
                        
                        if (pPanel.IsActive())
                        {
                            pPanel.Show(m_dicActiveCompanyData[pData.m_strGroupId]);
                        }
                        break;
                    }
                }

                UpdateUIForActiveCompany(() => { });
            }
            else
            {
                Single.BusinessGlobal.ShowAlertUI(reply);
            }
        });
    }

    public void OnEventForPurchaseMining(string strInstanceId)
    {
        RequestPurchaseMiningActiveCompany(strInstanceId);
    }

    public async void OnEventForShowSubUnits(string strGroupId)
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pPanel = await pUIRoot.GetPanel<SHUIPopupPanelMiningSubActiveCompany>(SHUIConstant.PANEL_MINING_SUB_ACTIVE_COMPANY);

        if (true == m_dicActiveCompanyData.ContainsKey(strGroupId))
        {
            pPanel.Show(m_dicActiveCompanyData[strGroupId]);
        }
        else
        {
            pPanel.Show(new List<SHActiveSlotData>());
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
        var pInventoryInfo = await Single.Table.GetTable<SHTableServerUserInventory>();

        Single.Network.POST(SHAPIs.SH_API_TEST_RESET_POWER, null, (reply) => 
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
        var pInventoryInfo = await Single.Table.GetTable<SHTableServerUserInventory>();

        Single.Network.POST(SHAPIs.SH_API_TEST_USE_POWER, null, (reply) => 
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
