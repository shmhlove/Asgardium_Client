using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public static partial class SHUtils
{
    public static void HorizontalAlignment(
        float fSlotSize, 
        float fGapSize, 
        int iMaxCount, 
        eHorizontalAlignment eAlignment,
        List<GameObject> pItems)
    {
        // 시작위치 결정 : 좌측부터 차례로 쌓이는 형태
        var fStartPosition = 0.0f;
        var fHalfSlotSize = (fSlotSize / 2.0f);
        switch (eAlignment)
        {
            case eHorizontalAlignment.Left:
                {
                    var fTotalSize = (fSlotSize * iMaxCount) + (fGapSize * (iMaxCount - 1));
                    var fHalfSize = (fTotalSize * 0.5f);
                    fStartPosition = -fHalfSize;
                }
                break;
            case eHorizontalAlignment.Center:
                {
                    var fSlotTotalSize = (fSlotSize * pItems.Count) + (fGapSize * (pItems.Count - 1));
                    var fHalfSize = (fSlotTotalSize * 0.5f);
                    fStartPosition = -fHalfSize;
                }
                break;
            case eHorizontalAlignment.Right:
                {
                    var fTotalSize = (fSlotSize * iMaxCount) + (fGapSize * (iMaxCount - 1));
                    var fSlotTotalSize = (fSlotSize * pItems.Count) + (fGapSize * (pItems.Count - 1));
                    var fHalfSize = (fTotalSize * 0.5f);
                    fStartPosition = (fHalfSize - fSlotTotalSize);
                }
                break;
        }

        for (int iLoop = 0; iLoop < pItems.Count; ++iLoop)
        {
            var vPosition = pItems[iLoop].transform.localPosition;
            vPosition.x = fHalfSlotSize + (fStartPosition + ((iLoop * fSlotSize) + (iLoop * fGapSize)));
            pItems[iLoop].transform.localPosition = vPosition;
        }
    }
}