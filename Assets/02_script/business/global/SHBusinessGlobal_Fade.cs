using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class SHBusinessGlobal_Fade : SHBusinessPresenter
{
    public async void Show(Action pCallback)
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        (await pUIRoot.GetFade()).Show(pCallback);
    }

    public async void Close(Action pCallback)
    {
        var pUIRoot = await Single.UI.GetRoot<SHUIRootGlobal>(SHUIConstant.ROOT_GLOBAL);
        var pUIPanel = await pUIRoot.GetFade();
        pUIPanel.SetActive(true);
        pUIPanel.Close(pCallback);
    }
}