using UnityEngine;
using System;
using System.Collections;

public class SHMonoWrapper : MonoBehaviour
{
    #region Members : Animation
    [HideInInspector] public Animation   m_pAnim             = null;
    [HideInInspector] public bool        m_bIsAnimPlaying    = false;
    #endregion


    #region Interface : Active
    public void SetActive(bool bIsActive)
    {
        if (IsActive() == bIsActive)
            return;
        
        gameObject.SetActive(bIsActive);
    }
    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }
    #endregion


    #region Interface : Position
    public void SetPosition(Vector3 vPos)
    {
        gameObject.transform.position = vPos;
    }
    public void SetPositionX(float fX)
    {
        Vector3 vPos = GetPosition();
        vPos.x = fX;
        SetPosition(vPos);
    }
    public void SetPositionY(float fY)
    {
        Vector3 vPos = GetPosition();
        vPos.y = fY;
        SetPosition(vPos);
    }
    public void SetPositionZ(float fZ)
    {
        Vector3 vPos = GetPosition();
        vPos.z = fZ;
        SetPosition(vPos);
    }
    public void SetLocalPosition(Vector3 vPos)
    {
        gameObject.transform.localPosition = vPos;
    }
    public void SetLocalPositionX(float fX)
    {
        Vector3 vPos = GetLocalPosition();
        vPos.x = fX;
        SetLocalPosition(vPos);
    }
    public void SetLocalPositionY(float fY)
    {
        Vector3 vPos = GetLocalPosition();
        vPos.y = fY;
        SetLocalPosition(vPos);
    }
    public void SetLocalPositionZ(float fZ)
    {
        Vector3 vPos = GetLocalPosition();
        vPos.z = fZ;
        SetLocalPosition(vPos);
    }
    public void AddLocalPosition(Vector3 vPos)
    {
        gameObject.transform.localPosition = (GetLocalPosition() + vPos);
    }
    public void AddLocalPositionX(float fX)
    {
        Vector3 vPos = GetLocalPosition();
        vPos.x += fX;
        SetLocalPosition(vPos);
    }
    public void AddLocalPositionY(float fY)
    {
        Vector3 vPos = GetLocalPosition();
        vPos.y += fY;
        SetLocalPosition(vPos);
    }
    public void AddLocalPositionZ(float fZ)
    {
        Vector3 vPos = GetLocalPosition();
        vPos.z += fZ;
        SetLocalPosition(vPos);
    }
    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }
    public Vector3 GetLocalPosition()
    {
        return gameObject.transform.localPosition;
    }
    #endregion


    #region Interface : Scale
    public void SetLocalScale(Vector3 vScale)
    {
        gameObject.transform.localScale = vScale;
    }
    public Vector3 GetLocalScale()
    {
        return gameObject.transform.localScale;
    }
    #endregion


    #region Interface : Rotate
    public void SetRotate(Vector3 vRotate)
    {
        var pRotation = GetRotate();
        pRotation.eulerAngles = vRotate;
        SetRotate(pRotation);
    }
    public void SetRotate(Quaternion qRotate)
    {
        gameObject.transform.rotation = qRotate;
    }
    public void SetLocalRotate(Vector3 vRotate)
    {
        var pRotation = GetLocalRotate();
        pRotation.eulerAngles = vRotate;
        SetLocalRotate(pRotation);
    }
    public void SetLocalRotate(Quaternion qRotate)
    {
        gameObject.transform.localRotation = qRotate;
    }
    public void SetLocalRotateX(float fValue)
    {
        Quaternion  qRot = GetLocalRotate();
        Vector3     vRet = qRot.eulerAngles;
        vRet.x = fValue;
        SetLocalRotate(vRet);
    }
    public void AddLocalRotateX(float fValue)
    {
        SetLocalRotateX(GetLocalRotateX() + fValue);
    }
    public float GetLocalRotateX()
    {
        return GetLocalRotate().eulerAngles.x;
    }
    public void SetLocalRotateY(float fValue)
    {
        Quaternion  qRot = GetLocalRotate();
        Vector3     vRet = qRot.eulerAngles;
        vRet.y = fValue;
        SetLocalRotate(vRet);
    }
    public void AddLocalRotateY(float fValue)
    {
        SetLocalRotateY(GetLocalRotateY() + fValue);
    }
    public float GetLocalRotateY()
    {
        return GetLocalRotate().eulerAngles.y;
    }
    public void SetLocalRotateZ(float fValue)
    {
        Quaternion qRot = GetLocalRotate();
        Vector3 vRet = qRot.eulerAngles;
        vRet.z = fValue;
        SetLocalRotate(vRet);
    }
    public void AddLocalRotateZ(float fValue)
    {
        SetLocalRotateZ(GetLocalRotateZ() + fValue);
    }
    public float GetLocalRotateZ()
    {
        return GetLocalRotate().eulerAngles.z;
    }
    public Quaternion GetRotate()
    {
        return gameObject.transform.rotation;
    }
    public Quaternion GetLocalRotate()
    {
        return gameObject.transform.localRotation;
    }
    #endregion


    #region Interface : Animation
    public void PlayAnim(eDirection ePlayDir, GameObject pTargetObject, AnimationClip pClip, Action pEndCallback)
    {
        if (null == pTargetObject)
            pTargetObject = gameObject;

        if (null == pClip)
        {
            if (null != pEndCallback)
                pEndCallback();
            return;
        }

        if (false == pTargetObject.activeInHierarchy)
        {
            if (null != pEndCallback)
                pEndCallback();
            return;
        }

        var pAnim = GetAnimation(pTargetObject);
        if (null == pAnim.GetClip(pClip.name))
            pAnim.AddClip(pClip, pClip.name);

        if (1.0f == Time.timeScale)
        {
            var pState = pAnim[pClip.name];
            pState.normalizedTime = (eDirection.Front == ePlayDir) ? 0.0f :  1.0f;
            pState.speed          = (eDirection.Front == ePlayDir) ? 1.0f : -1.0f;

            pAnim.Stop();
            pAnim.Play(pClip.name);

            if (WrapMode.Loop != pState.wrapMode)
            {
                StartCoroutine(CoroutinePlayAnim_WaitTime(pState.length, pEndCallback));
            }
        }
        else
        {
            switch (ePlayDir)
            {
                case eDirection.Front:
                    StartCoroutine(CoroutinePlayAnim_UnScaledForward(pTargetObject, pAnim[pClip.name], pEndCallback));
                    break;
                case eDirection.Back:
                    StartCoroutine(CoroutinePlayAnim_UnScaledBackward(pTargetObject, pAnim[pClip.name], pEndCallback));
                    break;
            }
        }
    }
    private IEnumerator CoroutinePlayAnim_WaitTime(float fSec, Action pCallback)
    {
        yield return new WaitForSeconds(fSec);

        if (null != pCallback)
            pCallback();
    }
    private IEnumerator CoroutinePlayAnim_UnScaledForward(GameObject pObject, AnimationState pState, Action pEndCallback)
    {
        m_bIsAnimPlaying = true;

        float fStart     = Time.unscaledTime;
        float fElapsed   = 0.0f;
        
        while (true)
        {
            if ((null == pObject) || (null == pState))
                break;
            
            if (false == pObject.activeInHierarchy)
                break;

            fElapsed = Time.unscaledTime - fStart;
            pState.clip.SampleAnimation(pObject, fElapsed);

            if (pState.length <= fElapsed)
            {
                fStart = Time.unscaledTime;
                if (WrapMode.Loop != pState.wrapMode)
                    break;
            }

            yield return null;
        }

        if ((null != pObject) || (null != pState))
            pState.clip.SampleAnimation(pObject, pState.length);

        if (null != pEndCallback)
            pEndCallback();

        m_bIsAnimPlaying = false;
    }
    private IEnumerator CoroutinePlayAnim_UnScaledBackward(GameObject pObject, AnimationState pState, Action pEndCallback)
    {
        m_bIsAnimPlaying = true;

        float fStart    = Time.unscaledTime;
        float fElapsed  = 0.0f;

        while (true)
        {
            if ((null == pObject) || (null == pState))
                break;

            if (false == pObject.activeInHierarchy)
                break;

            fElapsed = pState.length - (Time.unscaledTime - fStart);
            pState.clip.SampleAnimation(pObject, fElapsed);

            if (0.0f >= fElapsed)
            {
                fStart = Time.unscaledTime;
                if (WrapMode.Loop != pState.wrapMode)
                    break;
            }

            yield return null;
        }

        if ((null != pObject) || (null != pState))
            pState.clip.SampleAnimation(pObject, 0.0f);

        if (null != pEndCallback)
            pEndCallback();

        m_bIsAnimPlaying = false;
    }
    #endregion


    #region Utility Functions
    private Animation GetAnimation(GameObject pObject = null)
    {
        if (null != m_pAnim)
            return m_pAnim;

        if (null == pObject)
            pObject = gameObject;
        
        return (m_pAnim = SHGameObject.GetComponent<Animation>(pObject));
    }
    #endregion
}