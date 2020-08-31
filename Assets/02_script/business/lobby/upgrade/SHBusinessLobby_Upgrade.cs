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
        pUIPanel.AddEventForPowerUpUpgrade(OnEventForUpgradePowerBtn);
        pUIPanel.AddEventForTimeUpUpgrade(OnEventForUpgradeTimeBtn);
    }

    public async override void OnEnter()
    {
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
            pUIPanel.SetUpgradeInfo(pUpgrade.MiningPowerLv, pUpgrade.ChargeTimeLv);
        });
        
        // @@ Active 회사 업그레이드 데이터 갱신
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

    // 마이닝파워 업그레이드 버튼 이벤트
    private async void OnEventForUpgradePowerBtn()
    {
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();
        var pInventory = await Single.Table.GetTable<SHTableServerUserInventory>();
        var pUpgradeInfo = await Single.Table.GetTable<SHTableServerUserUpgradeInfo>();
        var pPowerTable = await Single.Table.GetTable<SHTableServerMiningActiveMaxMP>();
        
        Func<SHTableServerUserInventory, SHTableServerUserUpgradeInfo, SHTableServerMiningActiveMaxMP, SHTableClientString, string> pMakeData = 
        (_pInventory, _pUpgradeInfo, _pPowerTable, _pStringTable) =>
        {
            var pCurTable = _pPowerTable.GetData(_pUpgradeInfo.MiningPowerLv);
            var pNextTable = _pPowerTable.GetData(_pUpgradeInfo.MiningPowerLv + 1);

            if (null == pCurTable)
            {
                Single.Global.GetAlert().Show(_pStringTable.GetString("1002"));
                return string.Empty;
            }
            pNextTable = pNextTable??pCurTable;
            
            JsonData pJson = new JsonData();
            pJson["curLv"] = pCurTable.m_iLevel;
            pJson["curMP"] = pCurTable.m_iMaxMP;
            pJson["nextLv"] = pNextTable.m_iLevel;
            pJson["nextMP"] = pNextTable.m_iMaxMP;
            pJson["upgradeCost"] = pNextTable.m_iCostGold;
            pJson["hasGold"] = _pInventory.Gold;
            return JsonMapper.ToJson(pJson);
        };
        
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pPopupPanel = await pUIRoot.GetPanel<SHUIPopupPanelUpgradePower>(SHUIConstant.PANEL_UPGRADE_POWER);

        Action<bool> pUpgradeAction = (bClickedUpgradeBtn) => 
        {
            if (false == bClickedUpgradeBtn) {
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
                        pPopupPanel.UpdateUI(pMakeData(pInventory, pUpgradeInfo, pPowerTable, pStringTable));
                    }

                    OnEnter();
                });
            }
        };

        pPopupPanel.Show(pUpgradeAction);
        pPopupPanel.UpdateUI(pMakeData(pInventory, pUpgradeInfo, pPowerTable, pStringTable));
    }

    // 충전시간 업그레이드 버튼 이벤트 (UI에서 SendMessage로 보내주고 있음..)
    private void OnEventForUpgradeTimeBtn()
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
