using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanel : SHMonoWrapper
{
    [Header("Basic Info")]
    [SerializeField] public  bool                m_bStartEnable = true;
    [SerializeField] private GameObject          m_pAnimRoot    = null;
    [SerializeField] private AnimationClip       m_pAnimOfOpen  = null;
    [SerializeField] private AnimationClip       m_pAnimOfClose = null;

    public virtual void OnInitialized()
    {
        SetActive(m_bStartEnable);
    }
    
    public virtual void OnBeforeShow(params object[] pArgs) { }
    public virtual void OnAfterShow(params object[] pArgs) { }
    public virtual void OnBeforeClose(params object[] pArgs) { }
    public virtual void OnAfterClose(params object[] pArgs) { }

    public void Show(params object[] pArgs)
    {
        // 켜져있는 상태에서 다시 켤때 애니메이션이 처음부터 시작하는게 어색해서..
        bool bIsActive = IsActive();

        SetActive(true);
        OnBeforeShow(pArgs);

        if (true == bIsActive)
        {
            OnAfterShow(pArgs);
        }
        else
        {
            PlayAnim(eDirection.Front, m_pAnimRoot, m_pAnimOfOpen, ()=> 
            {
                OnAfterShow(pArgs);
            });
        }
    }

    public void Close(params object[] pArgs)
    {
        OnBeforeClose(pArgs);
        PlayAnim(eDirection.Front, m_pAnimRoot, m_pAnimOfClose, ()=> 
        {
            OnAfterClose(pArgs);
            SetActive(false);
        });
    }
}
