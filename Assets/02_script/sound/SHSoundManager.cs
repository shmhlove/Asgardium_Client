using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using DicBGMChannels    = System.Collections.Generic.Dictionary<eSoundBGMChannel, UnityEngine.AudioSource>;
using DicEffectChannels = System.Collections.Generic.Dictionary<eSoundEffectChannel, UnityEngine.AudioSource>;
using DicTables         = System.Collections.Generic.Dictionary<string, SHSound_Table>;

public enum eSoundBGMChannel
{
    Main,
    Sub_1,
}

public enum eSoundEffectChannel
{
    Main,
    Sub_1,
}

public class SHSoundManager : SHSingleton<SHSoundManager>
{
    private DicBGMChannels    m_dicBGMChannel    = new DicBGMChannels();
    private DicEffectChannels m_dicEffectChannel = new DicEffectChannels();

    private DicTables      m_dicSoundTable = new DicTables();

    public override void OnInitialize()
    {
        CreateTable();
        CreateBGMChannel();
        CreateEffectChannel();
    }

    public override void OnFinalize()
    {
        StopAllCoroutines();
    }

    public void PlayBGM(string strName)
    {
        if (false == m_dicSoundTable.ContainsKey(strName))
        {
            Debug.LogError("[LSH] SHSound::PlayBGM() - Not Found TableInfo!!");
            return;
        }
        
        var pInfo = m_dicSoundTable[strName];
        Single.Resources.GetSound(pInfo.m_strFileName, (pObject) => 
        {
            m_dicBGMChannel[pInfo.m_eBGMChannel].clip   = pObject;
            m_dicBGMChannel[pInfo.m_eBGMChannel].loop   = pInfo.m_bIsLoop;
            m_dicBGMChannel[pInfo.m_eBGMChannel].volume = 0.0f;
            m_dicBGMChannel[pInfo.m_eBGMChannel].Play();
            m_dicBGMChannel[pInfo.m_eBGMChannel].Pause();
            m_dicBGMChannel[pInfo.m_eBGMChannel].Play();
            StartCoroutine(CoroutineVolumeUP(m_dicBGMChannel[pInfo.m_eBGMChannel]));
        });
    }

    public void StopBGM(string strName)
    {
        if (false == m_dicSoundTable.ContainsKey(strName))
        {
            Debug.LogError("[LSH] SHSound::StopBGM() - Not Found TableInfo!!");
            return;
        }

        var pInfo = m_dicSoundTable[strName];
        StartCoroutine(CoroutineVolumeDown(
            m_dicBGMChannel[pInfo.m_eBGMChannel], m_dicBGMChannel[pInfo.m_eBGMChannel].Stop));
    }

    public void PlayEffect(string strName)
    {
        if (false == m_dicSoundTable.ContainsKey(strName))
        {
            Debug.LogError("[LSH] SHSound::PlayEffect() - Not Found TableInfo!!");
            return;
        }
        
        var pInfo = m_dicSoundTable[strName];
        Single.Resources.GetSound(pInfo.m_strFileName, (pObject) => 
        {
            m_dicEffectChannel[pInfo.m_eEffectChannel].clip   = pObject;
            m_dicEffectChannel[pInfo.m_eEffectChannel].loop   = pInfo.m_bIsLoop;
            m_dicEffectChannel[pInfo.m_eEffectChannel].volume = 1.0f;
            m_dicEffectChannel[pInfo.m_eEffectChannel].Play();
            m_dicEffectChannel[pInfo.m_eEffectChannel].Pause();
            m_dicEffectChannel[pInfo.m_eEffectChannel].Play();
        });
    }

    public void StopEffect(string strName)
    {
        if (false == m_dicSoundTable.ContainsKey(strName))
        {
            Debug.LogError("[LSH] SHSound::StopEffect() - Not Found TableInfo!!");
            return;
        }

        var pInfo = m_dicSoundTable[strName];
        m_dicEffectChannel[pInfo.m_eEffectChannel].Stop();
    }

    void ClearChannel<T1, T2>(Dictionary<T1, T2> dicChannel) where T2 : UnityEngine.Object
    {
        foreach (var kvp in dicChannel)
        {
            GameObject.DestroyObject(kvp.Value);
        }
        dicChannel.Clear();
    }

    void CreateTable()
    {
        m_dicSoundTable.Clear();
        m_dicSoundTable.Add("Audio_BGM_Pineapple", new SHSound_BGM_Main());
        m_dicSoundTable.Add("Audio_BGM_GameOver",  new SHSound_BGM_GameOver());
        m_dicSoundTable.Add("Audio_Effect_Crash",  new SHSound_Effect_Crash());
    }

    void CreateBGMChannel()
    {
        ClearChannel(m_dicBGMChannel);
        m_dicBGMChannel.Clear();
        SHUtils.ForToEnum<eSoundBGMChannel>((eBGMChannel) =>
        {
            var pObject = new GameObject(eBGMChannel.ToString());
            pObject.transform.SetParent(transform);
            m_dicBGMChannel.Add(eBGMChannel, SHGameObject.GetComponent<AudioSource>(pObject));
        });
    }

    void CreateEffectChannel()
    {
        ClearChannel(m_dicEffectChannel);
        m_dicEffectChannel.Clear();
        SHUtils.ForToEnum<eSoundEffectChannel>((eEffectChannel) =>
        {
            var pObject = new GameObject(eEffectChannel.ToString());
            pObject.transform.SetParent(transform);
            m_dicEffectChannel.Add(eEffectChannel, SHGameObject.GetComponent<AudioSource>(pObject));
        });
    }

    private IEnumerator CoroutineVolumeUP(AudioSource pAudio)
    {
        if (null == pAudio)
            yield break;
        
        while(1.0f != pAudio.volume)
        {
            pAudio.volume += 0.02f;
            Mathf.Clamp(pAudio.volume, 0.0f, 1.0f);
            yield return null;
        }
    }
    private IEnumerator CoroutineVolumeDown(AudioSource pAudio, Action pEndCallback)
    {
        if (null == pAudio)
            yield break;

        while (0.0f != pAudio.volume)
        {
            pAudio.volume -= 0.02f;
            Mathf.Clamp(pAudio.volume, 0.0f, 1.0f);
            yield return null;
        }

        pEndCallback();
    }
}
