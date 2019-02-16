using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHGroupButton : MonoBehaviour
{
    private static Dictionary<string, List<SHGroupButton>> m_dicGroup = new Dictionary<string, List<SHGroupButton>>();

    [Header("Basic Info(Required)")]
    public string m_strGroupName;
    public bool m_bIsStarter;
    public bool m_bIsMaintainWhenReActive;
    public SHUIButton m_pTargetButton;

    private bool m_bCurState;
    private EventDelegate m_pEventClick;

    void Start()
    {
        if (SHGroupButton.m_dicGroup.ContainsKey(m_strGroupName))
        {
            SHGroupButton.m_dicGroup[m_strGroupName].Add(this);
        }
        else
        {
            SHGroupButton.m_dicGroup.Add(m_strGroupName, new List<SHGroupButton>());
            SHGroupButton.m_dicGroup[m_strGroupName].Add(this);
        }

        m_bCurState = m_bIsStarter;
        m_pEventClick = new EventDelegate(this, "OnClickButton");
        m_pTargetButton.onClick.Add(m_pEventClick);
    }

    void OnEnable()
    {
        Single.Coroutine.NextUpdate(() =>
        {
            if (m_bIsMaintainWhenReActive)
            {
                if (m_bCurState)
                {
                    m_pTargetButton.ExecuteClick();
                }
            }
            else
            {
                if (m_bIsStarter)
                {
                    m_pTargetButton.ExecuteClick();
                }
            }
        });
    }

    void OnDestroy()
    {
        m_dicGroup[m_strGroupName].Remove(this);
        m_pTargetButton.onClick.Remove(m_pEventClick);
    }

    public void ChangeState(bool bIsSelect)
    {
        if (bIsSelect)
        {
            m_pTargetButton.SetState(UIButtonColor.State.Disabled, false);

            foreach (var pObject in m_dicGroup[m_strGroupName])
            {
                if (this == pObject)
                    continue;

                pObject.ChangeState(false);
            }
        }
        else
        {
            m_pTargetButton.SetState(UIButtonColor.State.Normal, false);
        }


        m_bCurState = bIsSelect;
    }

    public void OnClickButton()
    {
        ChangeState(true);
    }
}
