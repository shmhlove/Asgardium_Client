using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUIScrollViewForActiveFilterbar : SHUIMassiveScrollView
{
    [Header("Filter Objects")]
    public UISprite m_pSpriteAllOn;

    private List<SHActiveFilterUnitData> m_pDatas = new List<SHActiveFilterUnitData>();

    protected override void SetSlotData(GameObject go, int index)
    {
        if (null == go)
        {
            return;
        }

        if (0 > index || index >= m_pDatas.Count)
        {
            return;
        }

        go.GetComponent<SHUIScrollSlotForActiveFilterbar>().SetData(m_pDatas[index]);
    }

    public void ResetDatas(List<SHActiveFilterUnitData> pDatas, bool bIsAllOn)
    {
        if (null != m_pSpriteAllOn)
        {
            NGUITools.SetActive(m_pSpriteAllOn.gameObject, bIsAllOn);
        }

        if (m_pDatas.Count != pDatas.Count)
        {
            m_pDatas = pDatas;
            SetSlotCount(m_pDatas.Count);
        }
        else
        {
            m_pDatas = pDatas;
            SetDirty();
            Paint();
        }
    }
}
