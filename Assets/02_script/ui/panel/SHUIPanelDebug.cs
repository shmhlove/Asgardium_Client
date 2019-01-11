using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHUIPanelDebug : SHUIPanel
{
    public void OnClickConsoleButton()
    {
		  LunarConsolePlugin.LunarConsole.Show();
    }
}
