using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public partial class SHBusinessLobby : MonoBehaviour
{
    private async void OnEventForMiningFilter()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
        var pPanel = await pUIRoot.GetPanel<SHUIPopupPanelMiningActiveUnitFilter>(SHUIConstant.PANEL_MINING_FILTER);

        // 필터링 대상유닛 데이터 생성
        var pUnitDatas = new List<SHActiveFilterUnitData>();
        var pUnitTable = await Single.Table.GetTable<SHTableServerGlobalUnitData>();
        foreach (var kvp in pUnitTable.m_dicDatas)
        {
            var pData = new SHActiveFilterUnitData();
            pData.m_iUnitId = kvp.Value.m_iUnitId;
            pData.m_strIconImage = kvp.Value.m_strIconImage;

            var bIsOn = SHPlayerPrefs.GetBool(kvp.Value.m_iUnitId.ToString());
            pData.m_bIsOn = (null == bIsOn) ? true : bIsOn.Value;

            pUnitDatas.Add(pData);
        }

        // 필터링 UI가 닫힐때 PlayerPreb에 셋팅하고, UI를 업데이트 한다.
        Action<List<SHActiveFilterUnitData>> pCloseEvent = (pDatas) =>
        {
            foreach (var pData in pDatas)
            {
                SHPlayerPrefs.SetBool(pData.m_iUnitId.ToString(), pData.m_bIsOn);
            }

            UpdateUIForActiveCompany(() => { });
            UpdateUIForActiveFilterbar();
        };

        // 필터링 UI Open
        pPanel.Show(pUnitDatas, pCloseEvent);
    }
}
