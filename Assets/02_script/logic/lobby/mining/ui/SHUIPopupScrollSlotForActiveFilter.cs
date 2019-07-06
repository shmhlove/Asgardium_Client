using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

// 이 아이가 문제네 HorizontalBox랑 의존성문제 발생.
// GameObject를 이아이가 관리해야한다.
// HorizontalBox는 정렬할때 GameObjects를 받아서 정렬하도록 하고 관리에서 빼는게 좋은거 같다.

public class SHUIPopupScrollSlotForActiveFilter : MonoBehaviour
{
    public float m_fSlotSize;
    public float m_fGapSize;
    public int m_iMaxCount;
    public eHorizontalAlignment m_eAlignment;

    public GameObject m_pUnitSample;
    
    private List<GameObject> m_pUnitObjects = new List<GameObject>();
    
    private void Awake()
    {
        if (null != m_pUnitSample)
        {
            m_pUnitSample.SetActive(false);
        }
    }

    private void Start()
    {
        MakeSpareSlots();
    }

    public void SetData(List<SHActiveFilterUnitData> pDatas, Action<int, bool> pEventUnitToggle)
    {
        if (0 == m_pUnitObjects.Count)
        {
            MakeSpareSlots();
        }

        var pUnits = new List<GameObject>();
        for (int iLoop = 0; iLoop < m_pUnitObjects.Count; ++iLoop)
        {
            if (iLoop < pDatas.Count)
            {
                NGUITools.SetActive(m_pUnitObjects[iLoop], true);
                
                var pSlot = m_pUnitObjects[iLoop].GetComponent<SHUIPopupScrollUnitForActiveFilter>();
                pSlot.SetUnitInfo(pDatas[iLoop], pEventUnitToggle);
                pUnits.Add(m_pUnitObjects[iLoop]);
            }
            else
            {
                NGUITools.SetActive(m_pUnitObjects[iLoop], false);
            }
        }

        SHUtils.HorizontalAlignment(m_fSlotSize, m_fGapSize, m_iMaxCount, m_eAlignment, pUnits);
    }

    private void MakeSpareSlots()
    {
        if (0 != m_pUnitObjects.Count)
            return;

        for (int iLoop = 0; iLoop < m_iMaxCount; ++iLoop)
        {
            var pSlot = Single.Resources.Instantiate<GameObject>(m_pUnitSample);
            pSlot.transform.SetParent(gameObject.transform);
            pSlot.transform.localPosition = Vector2.zero;
            pSlot.transform.localScale = m_pUnitSample.transform.localScale;
            pSlot.transform.localRotation = Quaternion.Euler(Vector3.zero);

            NGUITools.SetActive(pSlot, false);
            m_pUnitObjects.Add(pSlot);
        }

        NGUITools.SetActive(m_pUnitSample, false);
    }
}
