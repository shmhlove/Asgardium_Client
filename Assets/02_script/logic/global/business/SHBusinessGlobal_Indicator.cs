using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public partial class SHBusinessGlobal : SHSingleton<SHBusinessGlobal>
{
    public async void ShowIndicator()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        await pUIRoot.ShowIndicator();
    }

    public async void UpdateIndicatorMessage(string strMessage)
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        await pUIRoot.UpdateIndicatorMessage(strMessage);
    }

    public async void CloseIndicator()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        await pUIRoot.CloseIndicator();
    }
}