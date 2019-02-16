using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHGroupPanel : MonoBehaviour
{
    private static Dictionary<string, List<SHGroupPanel>> m_dicGroup = new Dictionary<string, List<SHGroupPanel>>();

    [Header("Basic Info(Required)")]
    public string m_strGroupName;
    public bool m_bIsStarter;
    public bool m_bIsMaintainWhenReActive;
    public SHUIPanel m_pTargetPanel;

    private bool m_bCurState;

    void Start()
    {
        if (SHGroupPanel.m_dicGroup.ContainsKey(m_strGroupName))
        {
            SHGroupPanel.m_dicGroup[m_strGroupName].Add(this);
        }
        else
        {
            SHGroupPanel.m_dicGroup.Add(m_strGroupName, new List<SHGroupPanel>());
            SHGroupPanel.m_dicGroup[m_strGroupName].Add(this);
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
            m_pTargetPanel.Show();

            foreach (var pObject in m_dicGroup[m_strGroupName])
            {
                if (this == pObject)
                    continue;

                pObject.ChangeState(false);
            }
        }
        else
        {
            m_pTargetPanel.Close();
        }

        m_bCurState = bIsSelect;
    }

    public void OnClickButton()
    {
        ChangeState(true);
    }
}
