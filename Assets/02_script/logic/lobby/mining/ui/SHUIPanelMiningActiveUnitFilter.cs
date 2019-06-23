using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

/*
    - 필터링 정보는 SHPlayerPrefs를 통해 관리
    - SHPlayerPrefs는 창이 열릴때 정보 얻어오고, 창이 닫힐때 갱신 처리하는게 좋겠다.
    - SHPlayerPrefs의 key는 unitId이며 value는 bool(On/Off) 정보
    - Hierarchy : Panel - ScrollView - ScrollViewSlot - ScrollViewUnit
*/

public class SHUIPanelMiningActiveUnitFilter : SHUIPanel
{
    private SHUIScrollViewForActiveFilter m_pScrollView;
    private List<SHActiveFilterUnitData> m_pDatas;

    public async override void OnBeforeShow(params object[] pArgs)
    {
        var pTable = await Single.Table.GetTable<SHTableServerGlobalUnitData>();

        m_pDatas = new List<SHActiveFilterUnitData>();
        foreach (var kvp in pTable.m_dicDatas)
        {
            var pData = new SHActiveFilterUnitData();
            pData.m_iUnitId = kvp.Value.m_iUnitId;
            pData.m_strIconImage = kvp.Value.m_strIconImage;
            var bIsOn = SHPlayerPrefs.GetBool(kvp.Value.m_iUnitId.ToString());
            pData.m_bIsOn = (null == bIsOn) ? true : bIsOn.Value;

            m_pDatas.Add(pData);
        }

        m_pScrollView.ResetDatas(m_pDatas);
    }

    private bool SaveFilterInfo()
    {
        bool bIsNotAllOff = false;
        foreach (var pData in m_pDatas)
        {
            SHPlayerPrefs.SetBool(pData.m_iUnitId.ToString(), pData.m_bIsOn);

            if (pData.m_bIsOn)
            {
                bIsNotAllOff = true;
            }
        }

        return bIsNotAllOff;
    }

    public async void OnClickCloseButton()
    {
        if (true == SaveFilterInfo())
        {
            Close();
        }
        else
        {
            var pStringTable = await Single.Table.GetTable<SHTableClientString>();
            Single.BusinessGlobal.ShowAlertUI(pStringTable.GetString("10018"));
        }
    }
}