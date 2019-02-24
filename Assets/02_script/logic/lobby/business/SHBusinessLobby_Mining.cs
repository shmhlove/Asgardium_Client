using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public partial class SHBusinessLobby : MonoBehaviour
{
    //[Header("UI Mining Objects")]
    //private SHUIPanelMining m_pUIPanelMining = null;

    // 액티브
    // 필터동작시 이벤트 받아 스크롤뷰 재구성, 필터내용은 저장하여 켜질때 그대로 셋팅될 수 있도록
    // 스크롤뷰 구성 - 출력해야할 리스트 서버로 부터 받아 슬롯구성할 수 있도록 스크롤뷰클래스에 전달

    // 패시브

    // 컴퍼니

    private async Task<SHTableUserInfo> GetUserInfo()
    {
        return await Single.Table.GetTable<SHTableUserInfo>();
    }

    private async Task<SHTableServerConfig> GetServerConfig()
    {
        return await Single.Table.GetTable<SHTableServerConfig>();
    }

    private IEnumerator CoroutineForMiningActive()
    {
        while (true)
        {
            if (null == m_pUIPanelMining)
            {
                yield return null;
            }

            var pUserInfo = GetUserInfo();
            var pConfigInfo = GetServerConfig();

            // pConfigInfo.BasicMiningPower
            // pConfigInfo.BasicChargeTime
            // pUserInfo.MiningPowerAt

            m_pUIPanelMining.SetActiveInformation("4/5", "--:--");
        }
    }
}
