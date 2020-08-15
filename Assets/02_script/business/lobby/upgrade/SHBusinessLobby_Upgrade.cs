using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public partial class SHBusinessLobby_Upgrade : SHBusinessPresenter
{
    public async override void OnInitialize()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pUIPanel = await pUIRoot.GetPanel<SHUIPanelUpgrade>(SHUIConstant.PANEL_UPGRADE);
        pUIPanel.AddEventForPowerUpUpgrade(OnEventForUpgradePowerupBtn);
        pUIPanel.AddEventForTimeUpUpgrade(OnEventForUpgradeTimeupBtn);
    }

    public async override void OnEnter()
    {
        // 엑티브 업그레이드 데이터 갱신
        var pUpgrade = await Single.Table.GetTable<SHTableServerUserUpgradeInfo>();
        pUpgrade.RequestGetUserUpgradeInfo(async (reply) =>
        {
            if (false == reply.isSucceed)
            {
                return;
            }

            pUpgrade.LoadJsonTable(reply.data);

           // UI 업데이트
            var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
            var pUIPanel = await pUIRoot.GetPanel<SHUIPanelUpgrade>(SHUIConstant.PANEL_UPGRADE);
            pUIPanel.SetActiveUpgradeInfo(pUpgrade.MiningPowerLv, pUpgrade.ChargeTimeLv);
        });
        
        // 열린 회사 업그레이드 데이터 갱신
    }

    public override void OnLeave() { }

    public override void OnUpdate() { }

    public override void OnFinalize() { }

    // Web요청 : 파워레벨 업그레이드
    private void RequestUpgradePowerLv(Action<SHReply> callback)
    {
        Single.Network.POST(SHAPIs.SH_API_USER_UPGRADE_ACTIVE_POWER, null, callback);
    }

    // Web요청 : 시간레벨 업그레이드
    private void RequestUpgradeTimeLv(Action<SHReply> callback)
    {
        Single.Network.POST(SHAPIs.SH_API_USER_UPGRADE_ACTIVE_TIME, null, callback);
    }

    // 마이닝파워 업그레이드 버튼 이벤트 (UI에서 SendMessage로 보내주고 있음..)
    private async void OnEventForUpgradePowerupBtn()
    {
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();
        var pInventory = await Single.Table.GetTable<SHTableServerUserInventory>();
        var pUpgradeInfo = await Single.Table.GetTable<SHTableServerUserUpgradeInfo>();
        var pUpgradePowerTable = await Single.Table.GetTable<SHTableServerMiningActiveMaxMP>();
        
        Func<SHTableServerUserInventory, SHTableServerUserUpgradeInfo, SHTableServerMiningActiveMaxMP, SHTableClientString, string> pMakeData = 
        (_pInventory, _pUpgradeInfo, _pUpgradePowerTable, _pStringTable) =>
        {
            var pCurInfo = _pUpgradePowerTable.GetData(_pUpgradeInfo.MiningPowerLv);
            var pNextInfo = _pUpgradePowerTable.GetData(_pUpgradeInfo.MiningPowerLv + 1);

            if (null == pCurInfo)
            {
                Single.Global.GetAlert().Show(_pStringTable.GetString("1002"));
                return string.Empty;
            }
            pNextInfo = pNextInfo??pCurInfo;
            
            var pCurTable = _pUpgradePowerTable.GetData(pCurInfo.m_iLevel);
            var pNextTable = _pUpgradePowerTable.GetData(pNextInfo.m_iLevel);

            JsonData pJson = new JsonData();
            pJson["curLv"] = pCurTable.m_iLevel;
            pJson["curMP"] = pCurTable.m_iMaxMP;
            pJson["nextLv"] = pNextTable.m_iLevel;
            pJson["nextMP"] = pNextTable.m_iMaxMP;
            pJson["upgradeCost"] = pNextTable.m_iCostGold;
            pJson["hasGold"] = _pInventory.Gold;
            return JsonMapper.ToJson(pJson) ;
        };
        
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pPopupPanel = await pUIRoot.GetPanel<SHUIPopupPanelUpgradePower>(SHUIConstant.PANEL_UPGRADE_POWER);

        Action<bool> pUpgradeAction = (bClickedUpgradeBtn) => 
        {
            if (false == bClickedUpgradeBtn) {
                OnEnter();
                return;
            }
            else {
                RequestUpgradePowerLv((reply) => 
                {
                    if (false == reply.isSucceed) {
                        Single.Global.GetAlert().Show(reply);
                    }
                    else {
                        pUpgradeInfo.LoadJsonTable(reply.data["upgrade_info"]);
                        pInventory.LoadJsonTable(reply.data["inventory_info"]);
                        pPopupPanel.UpdateUI(pMakeData(pInventory, pUpgradeInfo, pUpgradePowerTable, pStringTable));
                        // @@ 엑티브 업그레이드 갱신 : 함수로 빼서 호출하도록 하자.
                    }
                });
            }
        };

        pPopupPanel.Show(pUpgradeAction);
        pPopupPanel.UpdateUI(pMakeData(pInventory, pUpgradeInfo, pUpgradePowerTable, pStringTable));
    }

    // 충전시간 업그레이드 버튼 이벤트 (UI에서 SendMessage로 보내주고 있음..)
    private void OnEventForUpgradeTimeupBtn()
    {
        RequestUpgradeTimeLv((reply) => 
        {
            if (false == reply.isSucceed) {
                Single.Global.GetAlert().Show(reply);
            }
            else {
                OnEnter();
            }
        });
    }

    // 회사 설립 이벤트
    // 회사 업그레이드 이벤트
}
