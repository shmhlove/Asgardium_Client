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
    public int m_iTotalCount;
    public eHorizontalAlignment m_eAlignment;

    public List<GameObject> m_pSlots = new List<GameObject>();

    public void SetSlot(GameObject pSlot)
    {
        m_pSlots.Add(pSlot);
        ResetPosition();
    }

    [FuncButton]
    public void ResetPosition()
    {
        int iSlotLen = m_pSlots.Count;
        float fTotalSize = 0;
        float fStartPosition = 0;
        switch(m_eAlignment)
        {
            case eHorizontalAlignment.Left:
                fTotalSize = ((m_iTotalCount-1) * (m_fSlotSize + m_fGapSize)) - m_fGapSize;
                fStartPosition = -(fTotalSize/2);
            break;
            case eHorizontalAlignment.Center:
                fTotalSize = ((iSlotLen-1) * (m_fSlotSize + m_fGapSize)) - m_fGapSize;
                fStartPosition = -(fTotalSize/2);
            break;
            case eHorizontalAlignment.Right:
                fTotalSize = ((iSlotLen-1) * (m_fSlotSize + m_fGapSize)) - m_fGapSize;
                fStartPosition = -(fTotalSize/2);
            break;
        }
        
        for (int iLoop = 0; iLoop < iSlotLen; ++iLoop)
        {
            var position = m_pSlots[iLoop].transform.localPosition;
            position.x = fStartPosition + ((iLoop * (m_fSlotSize + m_fGapSize)) - (m_fGapSize/2));
            m_pSlots[iLoop].transform.localPosition = position;
        }
    }
}
