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
    
    public SHXML(string strFileName, Action<SHXML> pCallback)
    {
        if (true == string.IsNullOrEmpty(strFileName))
        {
            pCallback(this);
            return;
        }

        strFileName = Path.GetFileNameWithoutExtension(strFileName);

        // 1차 : PersistentDataPath에 데이터가 있으면 그걸 로드하도록 한다.
        // 2차 : 없으면 패키지에서 로드하도록 한다.

        string strSavePath = string.Format("{0}/{1}.xml", SHPath.GetPersistentDataXML(), strFileName);
        if (true == File.Exists(strSavePath))
        {
            LoadByPersistent(strSavePath, (strXML) => 
            {
                SetXMLData(strXML);
                pCallback(this);
            });
        }
        else
        {
            LoadByPackage(strFileName, (strXML) => 
            {
                SetXMLData(strXML);
                pCallback(this);
            });
        }
    }
    
    public bool CheckXML()
    {
        return (false == string.IsNullOrEmpty(m_strXMLData));
    }
    
    public XmlNodeList GetNodeList(string strTagName)
    {
        return GetNodeList(GetNodeToTag(GetDocument(), strTagName, 0));
    }

    private void SetXMLData(string strBuff)
    {
        if (false == string.IsNullOrEmpty(strBuff))
            return;

        var pStream = new MemoryStream(Encoding.UTF8.GetBytes(strBuff));
        var pReader = new StreamReader(pStream, true);
        m_strXMLData = pReader.ReadToEnd();
        pReader.Close();
        pStream.Close();
    }
    
    private void LoadByPersistent(string strFilePath, Action<string> pCallback)
    {
        var pBuff = File.ReadAllText(strFilePath);
        if (null == pBuff)
        {
            Debug.LogError(string.Format("[LSH] XML(*.xml)������ �д� �� �����߻�!!(Path:{0})", strFilePath));
        }

        pCallback(pBuff);
    }

    private void LoadByPackage(string strFileName, Action<string> pCallback)
    {
        Single.Resources.GetTextAsset(Path.GetFileNameWithoutExtension(strFileName), (pTextAsset) => 
        {
            if (null == pTextAsset)
            {
                pCallback(string.Empty);
            }
            else
            {
                pCallback(pTextAsset.text);
            }
        });
    }

    private XmlDocument GetDocument()
    {
        if (true == string.IsNullOrEmpty(m_strXMLData))
            return null;

        var pDocument = new XmlDocument();
        pDocument.LoadXml(m_strXMLData);
        return pDocument;
    }
    
    private XmlNode GetNodeToTag(XmlDocument pDoc, string strTag, int iItemIndex)
    {
        if (null == pDoc)
            pDoc = GetDocument();

        if (null == pDoc)
            return null;

        return pDoc.GetElementsByTagName(strTag).Item(iItemIndex);
    }

    public XmlNodeList GetNodeList(XmlNode pNode)
    {
        if (null == pNode)
            return null;

        return pNode.ChildNodes;
    }
}