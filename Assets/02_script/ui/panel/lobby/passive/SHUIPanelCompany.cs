using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHUIPanelCompany : MonoBehaviour
{
    CUIListView m_pCUI;

    public void Start()
    {
        m_pCUI = new CUIListView();
        //m_pCUI.m_scItemList;
        //m_pCUI.m_pfItem;
        //m_pCUI.m_goCreatedItem;
        m_pCUI.Init();
    }

    public void OnEnable()
    {
        m_pCUI.Reset();
    }

    public void Update()
    {
        m_pCUI.UpdateScrollBars();
    }
}
