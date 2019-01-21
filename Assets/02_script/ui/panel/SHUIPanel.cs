using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHUIPanel : MonoBehaviour
{
    [Header("BaseInfo")]
    [SerializeField] public  bool                m_bStartEnable = true;
    [SerializeField] private GameObject          m_pAnimRoot    = null;
    [SerializeField] private AnimationClip       m_pAnimToOpen  = null;
    [SerializeField] private AnimationClip       m_pAnimToClose = null;

    public virtual void OnBeforeShow(params object[] pArgs) { }
    public virtual void OnAfterShow(params object[] pArgs) { }
    public virtual void OnBeforeClose() { }
    public virtual void OnAfterClose() { }

    public void Show(params object[] pArgs)
    {
        OnBeforeShow(pArgs);
        // PlayAnimation(m_pAnimToOpen, ()=> 
        // {
        //     OnAfterShow(pArgs);
        // });
    }

    public void Close()
    {
        OnBeforeClose();
        // PlayAnimation(m_pAnimToClose, ()=> 
        // {
        //     OnAfterClose();
        //     SetActive(false);
        // });
    }

    public virtual void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    // void PlayAnimation(AnimationClip pClip, Action pEndCallback)
    // {
    //     PlayAnim(eDirection.Front, m_pAnimRoot, pClip, pEndCallback);
    // }

    public virtual void OnClickToClose()
    {
        Close();
    }
}
