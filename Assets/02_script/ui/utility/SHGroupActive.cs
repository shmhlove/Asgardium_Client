using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHGroupActive : MonoBehaviour
{
    private static Dictionary<string, List<SHGroupActive>> m_dicGroup = new Dictionary<string, List<SHGroupActive>>();

    [Header("Basic Info(Required)")]
    public string m_strGroupName;
    public bool m_bIsStarter;
    public bool m_bIsMaintainWhenReActive;
    public GameObject m_pTargetObject;

    private bool m_bCurState;

    void Start()
    {
        if (SHGroupActive.m_dicGroup.ContainsKey(m_strGroupName))
        {
            SHGroupActive.m_dicGroup[m_strGroupName].Add(this);
        }
        else
        {
            SHGroupActive.m_dicGroup.Add(m_strGroupName, new List<SHGroupActive>());
            SHGroupActive.m_dicGroup[m_strGroupName].Add(this);
        }

        m_bCurState = m_bIsStarter;
    }

    void OnEnable()
    {
        Single.Coroutine.NextUpdate(() =>
        {
            if (m_bIsMaintainWhenReActive)
            {
                if (m_bCurState)
                {
                    ChangeState(true);
                }
            }
            else
            {
                if (m_bIsStarter)
                {
                    ChangeState(true);
                }
            }
        });
    }

    void OnDestroy()
    {
        m_dicGroup[m_strGroupName].Remove(this);
    }

    public void ChangeState(bool bIsSelect)
    {
        if (bIsSelect)
        {
            m_pTargetObject.SetActive(true);

            foreach(var pObject in m_dicGroup[m_strGroupName])
            {
                if (this == pObject)
                    continue;

                pObject.ChangeState(false);
            }
        }
        else
        {
            m_pTargetObject.SetActive(false);
        }

        m_bCurState = bIsSelect;
    }

    public void OnClickButton()
    {
        ChangeState(true);
    }
}
