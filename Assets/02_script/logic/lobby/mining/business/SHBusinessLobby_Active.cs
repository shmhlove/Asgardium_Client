using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public partial class SHBusinessLobby : MonoBehaviour
{
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

    private async void UpdateDataForActiveCompany()
    {
        var pCompanyTable = await Single.Table.GetTable<SHTableServerInstanceMiningActiveCompany>();
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();
        var pServerGlobalUnitDataTable = await Single.Table.GetTable<SHTableServerGlobalUnitData>();
        var pActiveMiningQuantityTable = await Single.Table.GetTable<SHTableServerMiningActiveQuantity>();

        // m_dicActiveCompanyData이 데이터는 탭이 전환될때만 초기화되어야한다.
        // 탭이 전환되기 전에는 업데이트만 쳐야하고,
        // 없는 유닛에 대해서는 Supply를 0으로 처리해야한다.
        // Supply 0인 유닛에 대해서는 UI에서 딤드처리를 해서 인터렉션을 받지 않도록 해야한다.

        // 업데이트
        // 삽입
        // 삭제

        foreach (var kvp in pCompanyTable.m_dicDatas)
        {
            var pData = new SHActiveSlotData
            {
                m_strSlotId = string.Format("{0}_{1}", kvp.Value.m_iEfficiencyLV, kvp.Value.m_iUnitId),
                m_strInstanceId = kvp.Value.m_strInstanceId,
                
                m_strCompanyName = pStringTable.GetString(kvp.Value.m_iNameStrid.ToString()),
                m_strCompanyIcon = kvp.Value.m_strEmblemImage,
                m_strResourceIcon = pServerGlobalUnitDataTable.GetData(kvp.Value.m_iUnitId).m_strIconImage,
                m_iResourceQuantity = pActiveMiningQuantityTable.GetData(kvp.Value.m_iEfficiencyLV).m_iQuantity,
                m_iPurchaseCost = pServerGlobalUnitDataTable.GetData(kvp.Value.m_iUnitId).m_iWeight,
                
                m_iUnitId = kvp.Value.m_iUnitId,
                m_iEfficiencyLevel = kvp.Value.m_iEfficiencyLV,
                m_iSupplyQuantity = kvp.Value.m_iSupplyCount,
                
                m_pEventPurchaseButton = OnEventForPurchaseMining,
                m_pEventShowSubItemsButton = OnEventForShowSubItems,
            };

            if (false == m_dicActiveCompanyData.ContainsKey(pData.m_strSlotId))
            {
                m_dicActiveCompanyData[pData.m_strSlotId] = new List<SHActiveSlotData>();
            }

            var pValue = m_dicActiveCompanyData[pData.m_strSlotId].Find((p) => 
            {
                return p.m_strInstanceId.Equals(pData.m_strInstanceId);
            });
            if (null != pValue) {
                pValue.CopyFrom(pData);
            }
            else {
                m_dicActiveCompanyData[pData.m_strSlotId].Add(pData);
            }
        }
    }

    private void UpdateUIForActiveCompany(Action pCallback)
    {
        if (0 == m_dicActiveCompanyData.Count)
        {
            pCallback();
            return;
        }

        // 출력할 데이터 선별
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

        // 출력순서 정렬
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
