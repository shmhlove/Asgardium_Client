using UnityEngine;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class SHBusinessGlobal_Indicator : SHBusinessPresenter
{
    public async void Show()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        (await pUIRoot.GetIndicator()).Show();
    }

    public async void UpdateMessage(string strMessage)
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        (await pUIRoot.GetIndicator()).SetMessage(strMessage);
    }

    public async void Close()
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        (await pUIRoot.GetIndicator()).Close();
    }
}