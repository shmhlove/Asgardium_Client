using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public partial class SHBusinessLobby : MonoBehaviour
{
    private void StartUpgrade()
    {
        AddEnableDelegate(eLobbyMenuType.Upgrade, EnableUpgradeMenu);
        AddDisableDelegate(eLobbyMenuType.Upgrade, DisableUpgradeMenu);
    }

    private async void EnableUpgradeMenu()
    {
        var pUpgrade = await Single.Table.GetTable<SHTableServerUserUpgradeInfo>();

        // 엑티브 업그레이드 갱신
        pUpgrade.RequestGetUserUpgradeInfo((reply) =>
        {
            if (false == reply.isSucceed)
            {
                return;
            }

            pUpgrade.LoadJsonTable(reply.data);
            m_pUIPanelUpgrade.SetActiveUpgradeInfo(pUpgrade.MiningPowerLv, pUpgrade.ChargeTimeLv);
        });

        // 열린 회사 업그레이드
    }

    private void DisableUpgradeMenu()
    {
    }

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
    private async void OnEventForUpgradePowerupBtn()
    {
        // 데이터 업데이트를 먼저하자.
        var pInventory = await Single.Table.GetTable<SHTableServerUserInventory>();
        var pUpgradeInfo = await Single.Table.GetTable<SHTableServerUserUpgradeInfo>();
        var pUpgradePowerTable = await Single.Table.GetTable<SHTableServerMiningActiveMaxMP>();

        // 예외처리 필요하다.
        var pCurInfo = pUpgradePowerTable.GetData(pUpgradeInfo.MiningPowerLv);
        var pNextInfo = pUpgradePowerTable.GetData(pUpgradeInfo.MiningPowerLv + 1);

        // 자.. 테이블이 없을때 어떻게 처리할까?
        // 비지니스에서 처리한 뒤에 UI를 띄워줘야겠다.
        // 1. 현재 정보가 없을 때 -> 파라미터 혹은 에러로 인해 없을 때
        // 2. 다음 정보가 없을 때 -> 파라미터 혹은 에러로 인해 없을 때, Max레벨이라 없을 때


        // 이 후 업그레이드 팝업을 띄우고, 업그레이드 액션에 대한 처리를 한다.
        
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pPopupPanel = await pUIRoot.GetPanel<SHUIPopupPanelUpgradePower>(SHUIConstant.PANEL_UPGRADE_POWER);

        Action<bool> pUpgradeAction = (bClickedUpgradeBtn) => 
        {
            if (false == bClickedUpgradeBtn) {
                EnableUpgradeMenu();
                return;
            }
            else {
                RequestUpgradePowerLv((reply) => 
                {
                    if (false == reply.isSucceed) {
                        Single.BusinessGlobal.ShowAlertUI(reply);
                    }
                    else {
                        pPopupPanel.UpdateUI();
                    }
                });
            }
        };

        pPopupPanel.UpdateUI();
        pPopupPanel.Show(pUpgradeAction);
    }

    // 충전시간 업그레이드 버튼 이벤트
    private void OnEventForUpgradeTimeupBtn()
    {
        RequestUpgradeTimeLv((reply) => 
        {
            if (false == reply.isSucceed) {
                Single.BusinessGlobal.ShowAlertUI(reply);
            }
            else {
                EnableUpgradeMenu();
            }
        });
    }

    // 회사 설립 이벤트
    // 회사 업그레이드 이벤트
}
