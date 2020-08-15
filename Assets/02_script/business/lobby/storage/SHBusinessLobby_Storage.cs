using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHBusinessLobby_Storage : SHBusinessPresenter
{
    public override void OnInitialize() { }

    public async override void OnEnter()
    {
        var pInventory   = await Single.Table.GetTable<SHTableServerUserInventory>();
        var pUnitTable   = await Single.Table.GetTable<SHTableServerGlobalUnit>();
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();

        // Basic Goods 셋팅
        //m_pUIPanelStorage
        
        // Unit Goods 셋팅
        pInventory.RequestGetUserInventory(async (reply) => 
        {
            var pUnitData = new List<SHTableGridSlotForUnitData>();
            foreach (var kvp in pInventory.HasUnits)
            {
                var pData = new SHTableGridSlotForUnitData();
                var pUnit = pUnitTable.GetData(kvp.Key);

                pData.m_iUnitID     = pUnit.m_iUnitId;
                pData.m_strIconName = pUnit.m_strIconImage;
                pData.m_strUnitName = pStringTable.GetString(pUnit.m_iNameStrId.ToString());
                pData.m_iUnitValue  = kvp.Value;
                pData.m_iGoldValue  = (int)(kvp.Value * 1.5f);

                pUnitData.Add(pData);
            }
            
            // UI 이벤트 바인딩
            var pUIRoot = await Single.UI.GetRoot<SHUIRootLobby>(SHUIConstant.ROOT_LOBBY);
            var pUIPanel = await pUIRoot.GetPanel<SHUIPanelStorage>(SHUIConstant.PANEL_STORAGE);
            pUIPanel.SetUnitGoods(pUnitData, (int iUnitId) =>
            {
                Single.Global.GetAlert().Show(string.Format("업데이트 예정(UnitID : {0})", iUnitId));
            });
        });

        // Artifact Goods 셋팅
        //m_pUIPanelStorage
    }

    public override void OnLeave() { }

    public override void OnUpdate() { }

    public override void OnFinalize() { }
}
