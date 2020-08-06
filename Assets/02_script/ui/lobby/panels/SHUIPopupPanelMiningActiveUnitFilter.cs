using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

/*
    - Hierarchy : 
    Panel(Open/Close 처리) - ScrollView(UI 이벤트 관리) - ScrollViewSlot(오브젝트 관리) - ScrollViewUnit(유닛표현)
*/

public class SHUIPopupPanelMiningActiveUnitFilter : SHUIPanel
{
    public SHUIPopupScrollViewForActiveFilter m_pScrollView;

    private List<SHActiveFilterUnitData> m_pDatas;
    private Action<List<SHActiveFilterUnitData>> m_pEventClose;
    
    public override void OnBeforeShow(params object[] pArgs)
    {
        if (null == pArgs)
        {
            return;
        }
        
        if (2 > pArgs.Length)
        {
            return;
        }
        
        m_pDatas = pArgs[0] as List<SHActiveFilterUnitData>;
        m_pEventClose = pArgs[1] as Action<List<SHActiveFilterUnitData>>;
        
        m_pScrollView.ResetDatas(m_pDatas);
    }

    private bool IsAllOff()
    {
        return (null == m_pDatas.Find((pData) => { return pData.m_bIsOn; }));
    }

    public async void OnClickCloseButton()
    {
        if (true == IsAllOff())
        {
            var pStringTable = await Single.Table.GetTable<SHTableClientString>();
            Single.Global.GetAlert().Show(pStringTable.GetString("10018"));
        }
        else
        {
            if (null != m_pEventClose)
            {
                m_pEventClose(m_pDatas);
            }

            Close();
        }
    }
}