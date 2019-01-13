using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHUIPanelChat : SHUIPanel 
{
	public void OnClickConnectButton()
	{
        Single.Network.ConnectWebSocket((reply) =>
        {

        });
	}
}
