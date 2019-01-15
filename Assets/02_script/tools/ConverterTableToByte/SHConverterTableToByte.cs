using UnityEngine;

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
        ConverterTableToByte(pTableData, SHPath.GetResourceBytesTable());
        pTableData.OnFinalize();
    }

    // 인터페이스 : 바이트파일 컨버터 ( 전달된 TableData를 참조해서 전달된 저장경로에 쏟아 냄 )
    public void ConverterTableToByte(SHTableData pTableData, string strSavePath)
    {
        if (null == pTableData)
            return;

        SHUtils.ForToDic(pTableData.Tables, (pKey, pValue) =>
        {
            ConverterByteFile(pValue, strSavePath);
        });

        Debug.Log("<color=yellow>[LSH] Converter Table To Byte Finish!!!</color>");
    }

    // 인터페이스 : 바이트파일 컨버터 ( 파일 하나 )
    public void ConverterByteFile(SHBaseTable pTable, string strSavePath)
    {
        if (null == pTable)
            return;

        byte[] pBytes = pTable.GetBytesTable();
        if (null == pBytes)
            return;
        
        SHUtils.SaveByte(pBytes, string.Format("{0}/{1}{2}", strSavePath, pTable.m_strByteFileName, ".bytes"));

        Debug.Log(string.Format("[LSH] {0} To Converter Byte Files : {1}",
                    (true == pTable.IsLoadTable() ? "<color=yellow>Success</color>" : "<color=red>Fail!!</color>"),
                    pTable.m_strFileName));
    }
}