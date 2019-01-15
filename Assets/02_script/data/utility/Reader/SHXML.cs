using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using System.Xml;

public class SHXML
{
    public string m_strXMLData = string.Empty;
    
    public SHXML() { }
    public SHXML(string strFileName)
    {
        if (true == string.IsNullOrEmpty(strFileName))
            return;

        strFileName = Path.GetFileNameWithoutExtension(strFileName);

        // 1�� : PersistentData�� XML �����Ͱ� ������ �װ� �ε��ϵ��� �Ѵ�.
        // 2�� : ������ ��Ű������ �ε��ϵ��� �Ѵ�.

        string strSavePath = string.Format("{0}/{1}.xml", SHPath.GetPersistentDataXML(), strFileName);
        if (true == File.Exists(strSavePath))
            SetXMLData(LoadLocal(strSavePath));
        else
            SetXMLData(LoadPackage(strFileName));
    }
    
    public bool CheckXML()
    {
        return (false == string.IsNullOrEmpty(m_strXMLData));
    }
    
    public void SetXMLData(string strBuff)
    {
        if (false == string.IsNullOrEmpty(strBuff))
            return;

        var pStream = new MemoryStream(Encoding.UTF8.GetBytes(strBuff));
        var pReader = new StreamReader(pStream, true);
        m_strXMLData = pReader.ReadToEnd();
        pReader.Close();
        pStream.Close();
    }
    
    public string GetXMLData()
    {
        return m_strXMLData;
    }
    
    public XmlDocument GetDocument()
    {
        var strData = GetXMLData();
        if (true == string.IsNullOrEmpty(strData))
            return null;

        var pDocument = new XmlDocument();
        pDocument.LoadXml(strData);
        return pDocument;
    }
    
    public XmlNode GetNodeToTag(XmlDocument pDoc, string strTag, int iItemIndex)
    {
        if (null == pDoc)
            pDoc = GetDocument();

        if (null == pDoc)
            return null;

        return pDoc.GetElementsByTagName(strTag).Item(iItemIndex);
    }
    
    public XmlNodeList GetNodeList(string strTagName)
    {
        return GetNodeList(GetNodeToTag(GetDocument(), strTagName, 0));
    }

    public XmlNodeList GetNodeList(XmlNode pNode)
    {
        if (null == pNode)
            return null;

        return pNode.ChildNodes;
    }

    string LoadLocal(string strFilePath)
    {
        var pBuff = File.ReadAllText(strFilePath);
        if (null == pBuff)
        {
            Debug.LogError(string.Format("[LSH] XML(*.xml)������ �д� �� �����߻�!!(Path:{0})", strFilePath));
            return null;
        }

        return pBuff;
    }

    string LoadPackage(string strFileName)
    {
        var pTextAsset = Single.Resources.GetTextAsset(Path.GetFileNameWithoutExtension(strFileName));
        if (null == pTextAsset)
            return null;

        return pTextAsset.text;
    }
}