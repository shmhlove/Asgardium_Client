using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelStorage : SHUIPanel
{
    [Header("Basic Goods")]
    public SHUITableSlotForBasicGoods m_pBasicGoods;

    [Header("Unit Goods")]
    public SHUITableSlotForUnitGoods m_pUnitGoods;

    [Header("Artifact Goods")]
    public SHUITableSlotForArtifactGoods m_pArtifactGoods;

    public void SetUnitGoods(List<SHTableGridSlotForUnitData> pDatas, Action<int> pEventTransaction)
    {
        m_pUnitGoods.SetData(pDatas, pEventTransaction);
    }
}
