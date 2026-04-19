using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventChannelSO<T> : ScriptableObject
{   
    private readonly List<EventListener<T>> eventRaised = new List<EventListener<T>>();
    private bool _isRunning;
    public void EventRaise(T value)
    {
        if (_isRunning)
        {
            Debug.LogWarning("RaiseEvent called while event is already running (reentrancy blocked).");
            return;
        }
        try
        {
            if (eventRaised == null)
            {
                Debug.LogWarning("RaiseEvent called but no listeners are registered.");
                return;
            }
            _isRunning = true;
            List<EventListener<T>> snapshot = new List<EventListener<T>>(eventRaised);
            foreach (var listener in snapshot)
            {
                try
                {
                    listener?.Raise(value);
                }
                catch (Exception ex)
                {
                    Debug.LogError("RaiseEvent called but exeption " + ex.Message);
                }
            }
        }
        finally
        {
            _isRunning = false;
        }
    }

    public void AddListener(EventListener<T> listener)
    {
        if (listener == null)
        {
            Debug.LogWarning("AddListener called but no listeners are registered.");
            return;
        }
        if (!eventRaised.Contains(listener))
        {
            eventRaised.Add(listener);
            Debug.LogWarning("Listener added: " + listener);
        }
    }

    public void RemoveListener(EventListener<T> listener)
    {
        if (listener == null)
        {
            Debug.LogWarning("RemoveListener called but no listeners are registered.");
            return;
        }
        if (eventRaised.Contains(listener))
        {
            Debug.LogWarning("Listener removed: " + listener);
            eventRaised.Remove(listener);
        }
    }
}
