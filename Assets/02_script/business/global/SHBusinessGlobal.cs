using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public partial class SHBusinessGlobal : SHSingleton<SHBusinessGlobal>
{
    private SHBusinessPresenter m_pPresenters = new SHBusinessPresenter();

    public override void OnInitialize()
    {
        m_pPresenters.Add(new SHBusinessGlobal_Alert());
        m_pPresenters.Add(new SHBusinessGlobal_Indicator());
        m_pPresenters.Add(new SHBusinessGlobal_Fade());
        m_pPresenters.OnInitialize();

        SetDontDestroy();
    }

    public override void OnFinalize()
    {
        m_pPresenters.OnFinalize();
    }

    public SHBusinessGlobal_Alert GetAlert()
    {
        return m_pPresenters.Get<SHBusinessGlobal_Alert>();
    }

    public SHBusinessGlobal_Indicator GetIndicator()
    {
        return m_pPresenters.Get<SHBusinessGlobal_Indicator>();
    }

    public SHBusinessGlobal_Fade GetFade()
    {
        return m_pPresenters.Get<SHBusinessGlobal_Fade>();
    }
}