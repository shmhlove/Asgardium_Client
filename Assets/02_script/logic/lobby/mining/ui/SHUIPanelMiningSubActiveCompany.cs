using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelMiningSubActiveCompany : SHUIPanel
{
    public SHUIScrollViewForActive m_pActiveScrollView;

    public override void OnBeforeShow(params object[] pArgs)
    {
        if ((null == pArgs) || (0 == pArgs.Length))
        {
            return;
        }
        
        var pDatas = pArgs[0] as List<SHActiveSlotData>;
        m_pActiveScrollView.ResetDatas(pDatas);
        m_pActiveScrollView.SetSlotCount(pDatas.Count);
    }

    public void OnClickCloseButton()
    {
        Close();
    }
}