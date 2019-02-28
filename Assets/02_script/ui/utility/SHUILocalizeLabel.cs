using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUILocalizeLabel : MonoBehaviour
{
    [Tooltip("You can read about stringID from JsonClientString_kr.json in Resouces")]
    public string m_strStringID;
    
	public void Awake ()
	{
        SetLabel();
    }

    private async void SetLabel()
    {
        var pLabel = gameObject.GetComponent<UILabel>();
        pLabel.text = "loading...";
        
        var pTable = await Single.Table.GetTable<SHTableClientString>();
        if (pLabel.text.Equals("loading..."))
        {
            pLabel.text = pTable.GetString(m_strStringID);
        }
    }
}