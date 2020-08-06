using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHBusinessPresenter
{
    private Dictionary<Type, SHBusinessPresenter> m_dicPresenters = new Dictionary<Type, SHBusinessPresenter>();

    public virtual void OnInitialize()
    {
        if (null != m_dicPresenters)
        {
            foreach (var item in m_dicPresenters)
            {
                item.Value.OnInitialize();
            }
        }
    }

    public virtual void OnEnter()
    {
        if (null != m_dicPresenters)
        {
            foreach (var item in m_dicPresenters)
            {
                item.Value.OnEnter();
            }
        }
    }

    public virtual void OnLeave()
    {
        if (null != m_dicPresenters)
        {
            foreach (var item in m_dicPresenters)
            {
                item.Value.OnLeave();
            }
        }
    }

    public virtual void OnUpdate()
    {
        if (null != m_dicPresenters)
        {
            foreach (var item in m_dicPresenters)
            {
                item.Value.OnUpdate();
            }
        }
    }

    public virtual void OnFinalize()
    {
        if (null != m_dicPresenters)
        {
            foreach (var item in m_dicPresenters)
            {
                item.Value.OnFinalize();
            }
        }
    }

    public void Add<T>(T pPresenter) where T : SHBusinessPresenter
    {
        if (m_dicPresenters.ContainsKey(typeof(T)))
        {
            m_dicPresenters.Add(typeof(T), pPresenter);
        }
        else
        {
            m_dicPresenters[typeof(T)] = pPresenter;
        }
    }

    public T Get<T>() where T : SHBusinessPresenter
    {
        if (m_dicPresenters.ContainsKey(typeof(T)))
        {
            return m_dicPresenters[typeof(T)] as T;
        }
        else
        {
            return new SHBusinessPresenter() as T;
        }
    }
}