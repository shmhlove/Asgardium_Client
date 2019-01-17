using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanelDebug : SHUIPanel
{
    public void OnClickConsoleButton()
    {
		  LunarConsolePlugin.LunarConsole.Show();
    }
}
