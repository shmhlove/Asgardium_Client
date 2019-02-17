using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUIPanelStoragePassive : SHUIMassiveScrollView
{
    protected override void OnInitialized()
    {
        int iCount = 2;
        SetCellCount(iCount);
        SetScroll(0, true);
    }

    protected override void SetSlotData(GameObject go, int index)
    {       
    }

    protected override void Update()
    {
        base.Update();
    }
}
