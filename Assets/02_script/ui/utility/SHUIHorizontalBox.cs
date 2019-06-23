using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public enum eHorizontalAlignment
{
    Left,
    Center,
    Right
}

public class SHUIHorizontalBox : MonoBehaviour
{
    public float m_fSlotSize;
    public float m_fGapSize;
    public int m_iMaxCount;
    public eHorizontalAlignment m_eAlignment;

    public void Alignment(List<GameObject> pItems)
    {
        // 시작위치 결정 : 좌측부터 차례로 쌓이는 형태
        var fStartPosition = 0.0f;
        var fHalfSlotSize = (m_fSlotSize / 2.0f);
        switch (m_eAlignment)
        {
            case eHorizontalAlignment.Left:
                {
                    var fTotalSize = (m_fSlotSize * m_iMaxCount) + (m_fGapSize * (m_iMaxCount - 1));
                    var fHalfSize = (fTotalSize * 0.5f);
                    fStartPosition = -fHalfSize;
                }
                break;
            case eHorizontalAlignment.Center:
                {
                    var fSlotTotalSize = (m_fSlotSize * pItems.Count) + (m_fGapSize * (pItems.Count - 1));
                    var fHalfSize = (fSlotTotalSize * 0.5f);
                    fStartPosition = -fHalfSize;
                }
                break;
            case eHorizontalAlignment.Right:
                {
                    var fTotalSize = (m_fSlotSize * m_iMaxCount) + (m_fGapSize * (m_iMaxCount - 1));
                    var fSlotTotalSize = (m_fSlotSize * pItems.Count) + (m_fGapSize * (pItems.Count - 1));
                    var fHalfSize = (fTotalSize * 0.5f);
                    fStartPosition = (fHalfSize - fSlotTotalSize);
                }
                break;
        }

        for (int iLoop = 0; iLoop < pItems.Count; ++iLoop)
        {
            var vPosition = pItems[iLoop].transform.localPosition;
            vPosition.x = fHalfSlotSize + (fStartPosition + ((iLoop * m_fSlotSize) + (iLoop * m_fGapSize)));
            pItems[iLoop].transform.localPosition = vPosition;
        }
    }
}
