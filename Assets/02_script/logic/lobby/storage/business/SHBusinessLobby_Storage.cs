using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public partial class SHBusinessLobby : MonoBehaviour
{
    private async void SetChangeStorageTab()
    {
        var pInventory = await Single.Table.GetTable<SHTableServerInventoryInfo>();
        var pUnitTable = await Single.Table.GetTable<SHTableServerGlobalUnitData>();
        var pStringTable = await Single.Table.GetTable<SHTableClientString>();

        // Basic Goods 셋팅
        //m_pUIPanelStorage
        
        // Unit Goods 셋팅
        var pUserInfo = await Single.Table.GetTable<SHTableUserInfo>();
        pInventory.RequestGetInventoryInfo(pUserInfo.UserId, (reply) => 
        {
            pUserInfo.LoadJsonTable(reply.data);

            var pUnitInfos = new List<SHTableGridSlotForUnitData>();
            foreach (var kvp in pInventory.HasUnits)
            {
                var pUnit = new SHTableGridSlotForUnitData();
                var pUnitData = pUnitTable.GetData(kvp.Key);

                pUnit.m_iUnitID = pUnitData.m_iUnitId;
                pUnit.m_strIconName = pUnitData.m_strIconImage;
                pUnit.m_strUnitName = pStringTable.GetString(pUnitData.m_iNameStrId.ToString());
                pUnit.m_iUnitValue = kvp.Value;
                pUnit.m_iGoldValue = (int)(kvp.Value * 1.5f);

                pUnitInfos.Add(pUnit);
            }

            m_pUIPanelStorage.SetUnitGoods(pUnitInfos, OnEventForTransaction);
        });

        // Artifact Goods 셋팅
        //m_pUIPanelStorage
    }

    public void OnEventForTransaction(int iUnitId)
    {
        Single.BusinessGlobal.ShowAlertUI(string.Format("업데이트 예정(UnitID : {0})", iUnitId));
    }
}
