using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public partial class SHBusinessLobby : MonoBehaviour
{
    private void StartStorage()
    {
        AddEnableDelegate(eLobbyMenuType.Storage, EnableStorageMenu);
        AddDisableDelegate(eLobbyMenuType.Storage, DisableStorageMenu);
    }

    private async void EnableStorageMenu()
    {
        var pInventory   = await Single.Table.GetTable<SHTableServerUserInventory>();
        var pUnitTable   = await Single.Table.GetTable<SHTableServerGlobalUnit>();
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();

        // Basic Goods 셋팅
        //m_pUIPanelStorage
        
        // Unit Goods 셋팅
        pInventory.RequestGetUserInventory((reply) => 
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

            m_pUIPanelStorage.SetUnitGoods(pUnitData, OnEventForTransaction);
        });

        // Artifact Goods 셋팅
        //m_pUIPanelStorage
    }

    private void DisableStorageMenu()
    {
    }

    public void OnEventForTransaction(int iUnitId)
    {
        Single.BusinessGlobal.ShowAlertUI(string.Format("업데이트 예정(UnitID : {0})", iUnitId));
    }
}
