using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SHSound_Table
{
    public string               m_strFileName    = string.Empty;
    public bool                 m_bIsLoop        = false;
    public eSoundBGMChannel     m_eBGMChannel    = eSoundBGMChannel.Main;
    public eSoundEffectChannel  m_eEffectChannel = eSoundEffectChannel.Main;
}

public class SHSound_BGM_Main : SHSound_Table
{
    public SHSound_BGM_Main()
    {
        m_strFileName = "Audio_BGM_Pineapple";
        m_bIsLoop     = true;
        m_eBGMChannel = eSoundBGMChannel.Main;
    }
}

public class SHSound_BGM_GameOver : SHSound_Table
{
    public SHSound_BGM_GameOver()
    {
        m_strFileName = "Audio_BGM_GameOver";
        m_bIsLoop     = false;
        m_eBGMChannel = eSoundBGMChannel.Sub_1;
    }
}

public class SHSound_Effect_Crash : SHSound_Table
{
    public SHSound_Effect_Crash()
    {
        m_strFileName    = "Audio_Effect_Crash";
        m_bIsLoop        = false;
        m_eEffectChannel = eSoundEffectChannel.Main;
    }
}