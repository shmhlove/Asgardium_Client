using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUIScrollViewForActive : SHUIMassiveScrollView
{
    protected override void OnInitialized()
    {
        int iCount = 50;
        SetSlotCount(iCount);
        SetFocus(0, true);
    }

    protected override void SetSlotData(GameObject go, int index)
    {
    }

    protected override void Update()
    {
        base.Update();
    }
}
