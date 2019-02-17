using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUIPanelPassive : SHUIMassiveScrollView
{
    public override void OnEnable()
    {
        base.OnEnable();

        int iCount = 50;
        SetCellCount(iCount);
        //SetScroll(iCount - m_iMaxRow + 1, true);
        SetScroll(0, true);
    }

    protected override void OnInitialized()
    {
    }

    protected override void SetSlotData(GameObject go, int index)
    {       
    }

    protected override void Update()
    {
        base.Update();
    }
}
