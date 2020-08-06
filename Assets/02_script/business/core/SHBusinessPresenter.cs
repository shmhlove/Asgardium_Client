using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHBusinessPresenter
{
    private readonly Dictionary<Type, SHBusinessPresenter> m_dicPresenters = new Dictionary<Type, SHBusinessPresenter>();

    public virtual void OnInitialize()
    {
        foreach (var item in m_dicPresenters)
        {
            item.Value.OnInitialize();
        }
    }

    public virtual void OnEnter()
    {
        foreach (var item in m_dicPresenters)
        {
            item.Value.OnEnter();
        }
    }

    public virtual void OnLeave()
    {
        foreach (var item in m_dicPresenters)
        {
            item.Value.OnLeave();
        }
    }

    public virtual void OnUpdate()
    {
        foreach (var item in m_dicPresenters)
        {
            item.Value.OnUpdate();
        }
    }

    public virtual void OnFinalize()
    {
        foreach (var item in m_dicPresenters)
        {
            item.Value.OnFinalize();
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