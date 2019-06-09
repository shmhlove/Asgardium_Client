using UnityEngine;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class SHUILocalizeLabel : MonoBehaviour
{
    [Tooltip("You can read about stringID from JsonClientString_kr.json in Resouces")]
    public string m_strStringID;
    
	async void Awake ()
	{
        await SetLabel();
    }

    private async Task SetLabel()
    {
        var pLabel = gameObject.GetComponent<UILabel>();
        pLabel.text = "loading...";
        
        var pTable = await Single.Table.GetTable<SHTableClientString>();

        // 스트링 테이블을 가져오는 중 초기화에 의해 Labeltext가 변경되었는지 확인하고, 변경되지 않았으면 로컬라이징
        if (pLabel.text.Equals("loading..."))
        {
            pLabel.text = pTable.GetString(m_strStringID);
        }
    }
}