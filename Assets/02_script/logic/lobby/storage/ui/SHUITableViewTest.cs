using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHUITableViewTest : MonoBehaviour
{
    public UITable m_pTableView;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [FuncButton]
    void Reflash()
    {
        m_pTableView.repositionNow = true;
    }
}
