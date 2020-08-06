using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using LitJson;

public partial class SHBusinessLobby : MonoBehaviour
{
    private bool? SortConditionForEfficiencyLevel(SHActiveSlotData pValue1, SHActiveSlotData pValue2)
    {
        if (pValue1.m_iEfficiencyLevel == pValue2.m_iEfficiencyLevel)
            return null;

        return pValue1.m_iEfficiencyLevel < pValue2.m_iEfficiencyLevel;
    }

    private bool? SortConditionForUnitID(SHActiveSlotData pValue1, SHActiveSlotData pValue2)
    {
        if (pValue1.m_iUnitId == pValue2.m_iUnitId)
            return null;

        return pValue1.m_iUnitId > pValue2.m_iUnitId;
    }

    private bool? SortConditionForSupplyQuantity(SHActiveSlotData pValue1, SHActiveSlotData pValue2)
    {
        if (pValue1.m_iSupplyQuantity == pValue2.m_iSupplyQuantity)
            return null;

        return pValue1.m_iSupplyQuantity < pValue2.m_iSupplyQuantity;
    }
}
