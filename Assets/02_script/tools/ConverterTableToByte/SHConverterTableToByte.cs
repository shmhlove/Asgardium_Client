﻿using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SHConverterTableToByte
{
    // 인터페이스 : 에디터 클래스 전용 ( Resources폴더내에 컨버팅된 Byte파일을 쏟아 냄 )
    public void RunEditorToConverter()
    {
        var pTableData = new SHTableData();
        pTableData.OnInitialize();
        ConverterTableToByte(pTableData, SHPath.GetResourceByteTable());
        pTableData.OnFinalize();
    }

    // 인터페이스 : 바이트파일 컨버터 ( 전달된 TableData를 참조해서 전달된 저장경로에 쏟아 냄 )
    public void ConverterTableToByte(SHTableData pTableData, string strSavePath)
    {
        if (null == pTableData)
            return;

        int iSucceedCnt = 0;
        foreach (var kvp in pTableData.Tables)
        {
            if (ConverterByteFile(kvp.Value, strSavePath))
            {
                ++iSucceedCnt;
            }
        }

        Debug.LogFormat("<color=yellow>[LSH] Converter Table To Byte Finish!!!(SucceedCnt : {0})</color>", iSucceedCnt);
    }

    // 인터페이스 : 바이트파일 컨버터 ( 파일 하나 )
    public bool ConverterByteFile(SHBaseTable pTable, string strSavePath)
    {
        if (null == pTable)
            return false;

        byte[] pBytes = pTable.GetBytesTable();
        if (null == pBytes)
            return false;
        
        SHUtils.SaveByte(pBytes, string.Format("{0}/{1}{2}", strSavePath, pTable.GetFileName(eTableLoadType.Byte), ".bytes"));

        Debug.Log(string.Format("[LSH] {0} To Converter Byte Files : {1}",
                    (true == pTable.IsLoadTable() ? "<color=yellow>Success</color>" : "<color=red>Fail!!</color>"),
                    pTable.GetFileName(eTableLoadType.Byte)));

        return true;
    }
}