using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
        var pUnitTable = await Single.Table.GetTable<SHTableServerGlobalUnitData>();
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();

        // 엑티브 업그레이드
        pUpgrade.RequestGetUserUpgradeInfo(pUserInfo.UserId, (reply) =>
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

    // 닫힌 회사 구매
    // 마이닝파워 업그레이드 이벤트
    private void OnEventForUpgradePowerupBtn()
    {
        Debug.Log("파워 이벤트 옴");
    }

    // 충전시간 업그레이드 이벤트
    private void OnEventForUpgradeTimeupBtn()
    {
        Debug.Log("타임 이벤트 옴");
    }

    // 회사 설립 이벤트
    // 회사 업그레이드 이벤트
}
