using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Timers;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public class SHBusinessLobby_Mining_Active : SHBusinessPresenter
{
    private readonly Dictionary<string, List<SHActiveSlotData>> m_dicActiveCompanyData = new Dictionary<string, List<SHActiveSlotData>>();
    private readonly Timer m_pTimerUpdateActiveCompany = new Timer(1000);
    
    public override void OnInitialize()
    {
        m_pTimerUpdateActiveCompany.Elapsed += OnUpdateTimer;
        m_pTimerUpdateActiveCompany.AutoReset = true;
        m_pTimerUpdateActiveCompany.Enabled = true;

        // 웹소켓 이벤트 바인딩
        Single.Network.AddEventObserver(SHAPIs.SH_SOCKET_POLLING_MINING_ACTIVE_INFO, OnEventForSocketPollingMiningActiveInfo);
    }

    public override void OnEnter()
    {
        // 마이닝 엑티브 정보를 주기적으로 소켓을 통해 받기위한 구독 등록
        RequestSubscribeMiningActiveInfo();

        // 마이닝 엑티브 회사정보 업데이트
        m_dicActiveCompanyData.Clear();
        UpdateDataForActiveCompany();

        // 필터바 업데이트
        UpdateUIForActiveFilterbar();

        // 마이닝 업데이트 회사 정보 주기적으로 반영
        m_pTimerUpdateActiveCompany.Start();
    }

    public override void OnLeave()
    {
        // 마이닝 엑티브 정보를 주기적으로 소켓을 통해 받기위한 구독 해제
        RequestUnsubscribeMiningActiveInfo();
        
        m_pTimerUpdateActiveCompany.Stop();
    }

    public override void OnFinalize()
    {
        m_pTimerUpdateActiveCompany.Stop();
        m_pTimerUpdateActiveCompany.Dispose();
    }

    private async void UpdateUIForActiveInformation(Action pCallback)
    {
        var pInventoryInfo = await Single.Table.GetTable<SHTableServerUserInventory>();
        var pUpgradeInfo = await Single.Table.GetTable<SHTableServerUserUpgradeInfo>();
        var pServerConfig = await Single.Table.GetTable<SHTableServerGlobalConfig>();
        
        // 통신시간 갭 때문에 저장된 시간보다 1초정도 앞당겨 준다.
        var Epsilon = 1000;
        var LastMiningPowerAt = pInventoryInfo.MiningPowerAt - Epsilon;

        // 남은 시간과 파워갯수 구하기
        var pTimeSpan = (DateTime.UtcNow - SHUtils.GetUctTimeByMillisecond(LastMiningPowerAt));
        var iCurPowerCount = (int)(pTimeSpan.TotalMilliseconds / (double)pServerConfig.m_iBasicChargeTime);
        var fCurLeftTime = (pTimeSpan.TotalMilliseconds % (double)pServerConfig.m_iBasicChargeTime);

        // 파워갯수 출력형태로 구성
        iCurPowerCount = Math.Min(iCurPowerCount, pServerConfig.m_iBasicMiningPowerCount);
        string strCountInfo = string.Format("{0}/{1}", iCurPowerCount, pServerConfig.m_iBasicMiningPowerCount);

        // 남은 시간 출력형태로 구성
        var pLeftTime = TimeSpan.FromMilliseconds(pServerConfig.m_iBasicChargeTime - fCurLeftTime);
        var iLeftMinutes = (int)(pLeftTime.TotalSeconds / 60);
        var iLeftSecond = (int)(pLeftTime.TotalSeconds % 60);
        string strTimer = (iCurPowerCount < pServerConfig.m_iBasicMiningPowerCount) ? 
            string.Format("{0:00}:{1:00}", iLeftMinutes, iLeftSecond) : "--:--";

        // UI 업데이트
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pUIPanel = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        pUIPanel.SetActiveInformation(strCountInfo, strTimer);

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

            pSlotDatas.Add(new SHActiveFilterUnitData
            {
                m_iUnitId = kvp.Value.m_iUnitId,
                m_strIconImage = kvp.Value.m_strIconImage
            });
        }

        // UI 업데이트
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pUIPanel = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        pUIPanel.SetActiveFilterbarScrollview(pSlotDatas, bIsAllOn);
    }

    private async void UpdateUIForActiveCompany(Action pCallback)
    {
        if (0 == m_dicActiveCompanyData.Count)
        {
            pCallback();
            return;
        }

        var pSlotDatas = new List<SHActiveSlotData>();
        foreach (var kvp in m_dicActiveCompanyData)
        {
            // 유닛 필터링 체크 (UnitId를 분리해내서 해당 UnitId가 필터바에서 체크해제 되었는지 확인)
            var keySplit = kvp.Key.Split('_');
            var bIsOn = SHPlayerPrefs.GetBool(keySplit[1]);
            if (false == ((null == bIsOn) ? true : bIsOn.Value))
            {
                continue;
            }

            // 공급량 체크 & 서브 아이템 처리
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

        // UI 업데이트
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pUIPanel = await pUIRoot.GetPanel<SHUIPanelMining>(SHUIConstant.PANEL_MINING);
        pUIPanel.SetActiveScrollview(pSlotDatas);

        pCallback();
    }

    private bool? SortConditionForEfficiencyLevel(SHActiveSlotData pValue1, SHActiveSlotData pValue2)
    {
        if (pValue1.m_iEfficiencyLevel == pValue2.m_iEfficiencyLevel)
            return null;

        return pValue1.m_iEfficiencyLevel < pValue2.m_iEfficiencyLevel;
    }

    private bool? SortConditionForUnitID(SHActiveSlotData pValue1, SHActiveSlotData pValue2)
    {
        if (pValue1.m_iUnitId == pValue2.m_iUnitId)
            return null;

        return pValue1.m_iUnitId > pValue2.m_iUnitId;
    }

    private bool? SortConditionForSupplyQuantity(SHActiveSlotData pValue1, SHActiveSlotData pValue2)
    {
        if (pValue1.m_iSupplyQuantity == pValue2.m_iSupplyQuantity)
            return null;

        return pValue1.m_iSupplyQuantity < pValue2.m_iSupplyQuantity;
    }

    private async void UpdateDataForActiveCompany()
    {
        var pCompanyInfo = await Single.Table.GetTable<SHTableServerInstanceMiningActiveCompany>();
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();
        var pUnitTable = await Single.Table.GetTable<SHTableServerGlobalUnit>();
        var pQuantityTable = await Single.Table.GetTable<SHTableServerMiningActiveQuantity>();

        Func<SHTableServerInstanceMiningActiveCompanyData, SHActiveSlotData> pCreateUIData =
        (pServerData) =>
        {
            return new SHActiveSlotData()
            {
                m_strGroupId        = string.Format("{0}_{1}", pServerData.m_iEfficiencyLV, pServerData.m_iUnitId),
                m_strInstanceId     = pServerData.m_strInstanceId,
                m_strCompanyName    = pStringTable.GetString(pServerData.m_iNameStrid.ToString()),
                m_strCompanyIcon    = pServerData.m_strEmblemImage,
                m_strUnitIcon       = pUnitTable.GetData(pServerData.m_iUnitId).m_strIconImage,
                m_iUnitId           = pServerData.m_iUnitId,
                m_iUnitQuantity     = pQuantityTable.GetData(pServerData.m_iEfficiencyLV).m_iQuantity,
                m_iPurchaseCost     = pUnitTable.GetData(pServerData.m_iUnitId).m_iWeight,
                m_iEfficiencyLevel  = pServerData.m_iEfficiencyLV,
                m_iSupplyQuantity   = pServerData.m_iSupplyCount,
                m_bIsNPCCompany     = pServerData.m_bIsNPCCompany,
                
                m_pEventPurchaseButton = OnUIEventForPurchaseMining,
                m_pEventShowSubUnitsButton = OnUIEventForShowSubUnits,
            };
        };

        // 공급량이 0이 되더라도 UI 리스트에서 제거하지 않기 위해(싱크를 위함)
        // 회사정보가 없으면 서버 데이터를 기준으로 UI 데이터 생성
        // 회사정보가 있으면 UI 데이터를 기준으로 서버데이터 참고해서 데이터 갱신
        if (0 == m_dicActiveCompanyData.Count)
        {
            // 서버 데이터를 기준으로 UI 데이터 생성
            foreach (var kvp in pCompanyInfo.m_dicDatas)
            {
                var pUIData = pCreateUIData(kvp.Value);

                if (false == m_dicActiveCompanyData.ContainsKey(pUIData.m_strGroupId))
                {
                    m_dicActiveCompanyData[pUIData.m_strGroupId] = new List<SHActiveSlotData>();
                }

                m_dicActiveCompanyData[pUIData.m_strGroupId].Add(pUIData);
            }

            // NPC 회사를 가장 뒤로 정렬
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
                    var pTableData = pCompanyInfo.GetData(pSlotData.m_strInstanceId);
                    if (null == pTableData)
                    {
                        pSlotData.m_iSupplyQuantity = 0;
                    }
                    else
                    {
                        var pData = pCreateUIData(pTableData);
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

                // 서브 회사 팝업 UI 업데이트
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

                // 메인 회사 UI 업데이트
                UpdateUIForActiveCompany(() => { });
            }
            else
            {
                Single.Global.GetAlert().Show(reply);
            }
        });
    }

    public void OnUIEventForPurchaseMining(string strInstanceId)
    {
        RequestPurchaseMiningActiveCompany(strInstanceId);
    }

    public async void OnUIEventForShowSubUnits(string strGroupId)
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
            // if (m_pUIPanelMining)
            // {
            //     bool isDone = false;
            //     UpdateUIForActiveInformation(() => isDone = true);

            //     while (false == isDone)
            //         yield return null;
            // }
            
            yield return null;
        }
    }

    private void OnUpdateTimer(System.Object source, ElapsedEventArgs e)
    {
        //if (m_pUIPanelMining)
        // {
        //     bool isDone = false;
        //     UpdateUIForActiveCompany(() => isDone = true);
            
        //     while (false == isDone)
        //         yield return null;
        // }
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
                Single.Global.GetAlert().Show(reply);
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
                Single.Global.GetAlert().Show(reply);
            }
        });
    }
}