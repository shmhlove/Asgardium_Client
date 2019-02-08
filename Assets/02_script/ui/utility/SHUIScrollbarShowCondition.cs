using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SHUIScrollbarShowCondition : MonoBehaviour
{
    public enum eShowCondition
    {
        Always,
        OnlyIfNeeded,
    }

    public enum eMoveDirection
    {
        Horizontal,
        Vertical,
    }

    public eShowCondition m_eCondition;
    public eMoveDirection m_eDirection;
    public BoxCollider    m_pBackground;
    public BoxCollider    m_pForground;

    public void Start()
    {
        Single.Coroutine.NextUpdate(() =>
        {
            switch (m_eCondition)
            {
                case eShowCondition.Always:
                    gameObject.SetActive(true);
                    break;
                case eShowCondition.OnlyIfNeeded:
                    if (eMoveDirection.Vertical == m_eDirection)
                    {
                        gameObject.SetActive(m_pBackground.size.y > m_pForground.size.y);
                    }
                    else
                    {
                        gameObject.SetActive(m_pBackground.size.x > m_pForground.size.x);
                    }
                    break;
            }
        });
    }
}
