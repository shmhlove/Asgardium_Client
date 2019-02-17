using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUIPanelCompany : MonoBehaviour
{
    public CUIListView m_pCUI;

    void Start()
    {
        m_pCUI.OnUpdateItem = OnUpdateItem;
        m_pCUI.OnGetItemCount = OnGetItemCount;
        m_pCUI.Init();
    }

    public void OnUpdateItem(int index, GameObject go)
    {

    }

    public int OnGetItemCount()
    {
        return 50;
    }
}
