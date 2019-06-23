using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

// 이 아이가 문제네 HorizontalBox랑 의존성문제 발생.
// GameObject를 이아이가 관리해야한다.
// HorizontalBox는 정렬할때 GameObjects를 받아서 정렬하도록 하고 관리에서 빼는게 좋은거 같다.

public class SHUIScrollSlotForActiveFilter : MonoBehaviour
{
    public SHUIHorizontalBox m_pHorizontalBox;
    public GameObject m_pSample;
    
    private List<GameObject> m_pUnitObjects = new List<GameObject>();
    
    private void Awake()
    {
        if (null != m_pSample)
        {
            m_pSample.SetActive(false);
        }
    }

    private void Start()
    {
        MakeSpareSlots();
    }

    public void SetData(List<SHActiveFilterUnitData> pDatas)
    {
        var pUnits = new List<GameObject>();
        for (int iLoop = 0; iLoop < m_pUnitObjects.Count; ++iLoop)
        {
            if (iLoop < pDatas.Count)
            {
                var pSlot = m_pUnitObjects[iLoop].GetComponent<SHUIScrollUnitForActiveFilter>();
                pSlot.SetUnitInfo(pDatas[iLoop], () =>
                {
                    // On/Off 처리는 누가하는게 좋을까?
                });

                NGUITools.SetActive(m_pUnitObjects[iLoop], true);
            }
            else
            {
                NGUITools.SetActive(m_pUnitObjects[iLoop], false);
            }
        }

        m_pHorizontalBox.Alignment(pUnits);
    }

    private void MakeSpareSlots()
    {
        for (int iLoop = 0; iLoop < m_pHorizontalBox.m_iMaxCount; ++iLoop)
        {
            var pSlot = Single.Resources.Instantiate<GameObject>(m_pSample);
            pSlot.transform.SetParent(gameObject.transform);
            pSlot.transform.localPosition = Vector2.zero;
            pSlot.transform.localScale = m_pSample.transform.localScale;
            pSlot.transform.localRotation = Quaternion.Euler(Vector3.zero);

            NGUITools.SetActive(pSlot, false);
            m_pUnitObjects.Add(pSlot);
        }
    }
}
