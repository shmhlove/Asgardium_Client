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
        var iCurPowerCount = (int)(pTimeSpan.TotalMilliseconds / (double)m_pServerConfig.m_iBasicChargeTime);
        var fCurLeftTime = (pTimeSpan.TotalMilliseconds % (double)m_pServerConfig.m_iBasicChargeTime);

        // 파워갯수 출력형태로 구성
        iCurPowerCount = Math.Min(iCurPowerCount, m_pServerConfig.m_iBasicMiningPowerCount);
        string strCountInfo = string.Format("{0}/{1}", iCurPowerCount, m_pServerConfig.m_iBasicMiningPowerCount);

        // 남은 시간 출력형태로 구성
        var pLeftTime = TimeSpan.FromMilliseconds(m_pServerConfig.m_iBasicChargeTime - fCurLeftTime);
        var iLeftMinutes = (int)(pLeftTime.TotalSeconds / 60);
        var iLeftSecond = (int)(pLeftTime.TotalSeconds % 60);
        string strTimer = (iCurPowerCount < m_pServerConfig.m_iBasicMiningPowerCount) ? 
            string.Format("{0:00}:{1:00}", iLeftMinutes, iLeftSecond) : "--:--";

        // UI 업데이트
        m_pUIPanelMining.SetActiveInformation(strCountInfo, strTimer);
    }

    private async void UpdateActiveScrollview()
    {
        var pCompanyTable = await Single.Table.GetTable<SHTableServerCompanyForMining>();
        //pCompanyTable.LoadServerTable(async (errorCode) => 
        //{
            // if (eErrorCode.Succeed != errorCode)
            //     return;

            var pStringTable = await Single.Table.GetTable<SHTableClientString>();
            var pAsgardiumResourceTable = await Single.Table.GetTable<SHTableServerAsgardiumResource>();
            var pActiveMiningQuantityTable = await Single.Table.GetTable<SHTableServerAactiveMiningQuantity>();

            List<SHActiveSlotData> pSlotDatas = new List<SHActiveSlotData>();
            foreach (var kvp in pCompanyTable.m_dicDatas)
            {
                var pData = new SHActiveSlotData();
                pData.m_strInstanceId = kvp.Value.m_strInstanceId;
                pData.m_strCompanyName = pStringTable.GetString(kvp.Value.m_iNameStrid.ToString());
                pData.m_strCompanyIcon = kvp.Value.m_strEmblemImage;
                pData.m_strResourceIcon = pAsgardiumResourceTable.GetData(kvp.Value.m_iResourceId).m_strIconImage;
                pData.m_iResourceQuantity = pActiveMiningQuantityTable.GetData(kvp.Value.m_iEfficiencyLV).m_iQuantity;
                pData.m_iResourceLevel = kvp.Value.m_iEfficiencyLV;
                pData.m_iSupplyQuantity = kvp.Value.m_iSupplyCount;
                pData.m_iPurchaseCost = pAsgardiumResourceTable.GetData(kvp.Value.m_iResourceId).m_iWorth;
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
    public async void ShowAlert(string strMessage)
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        await pUIRoot.ShowAlert(strMessage);
    }

    public void OnEventOfPurchaseMining(string strInstanceId)
    {
        ShowAlert(string.Format("채굴 요청 : {0}", strInstanceId));
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
