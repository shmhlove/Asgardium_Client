using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class SHBusinessGlobal : SHSingleton<SHBusinessGlobal>
{
    public async void ShowAlertUI(string message)
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        await pUIRoot.ShowAlert(message);
    }
    
    public void ShowErrorAlertUI(SHReply reply)
    {
        ShowAlertUI(reply.ToString());
    }
}