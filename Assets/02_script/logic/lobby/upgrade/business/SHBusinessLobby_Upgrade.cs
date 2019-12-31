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

        // 엑티브 업그레이드
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

    // 닫힌 회사 구매
    
    // 마이닝파워 업그레이드 이벤트
    private async void OnEventForUpgradePowerupBtn()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pPanel = await pUIRoot.GetPanel<SHUIPopupPanelUpgradePower>(SHUIConstant.PANEL_UPGRADE_POWER);

        Action<bool> pUpgradeAction = (bClickedUpgradeBtn) => 
        {
            if (false == bClickedUpgradeBtn) {
                EnableUpgradeMenu();
                pPanel.Close();
                return;
            }
            else {
                RequestUpgradePowerLv((reply) => 
                {
                    if (false == reply.isSucceed) {
                        Single.BusinessGlobal.ShowAlertUI(reply);
                    }
                    else {
                        pPanel.ResetUpgradeInfo();
                    }
                });
            }
        };
        pPanel.Show(pUpgradeAction);
    }

    // 충전시간 업그레이드 이벤트
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
