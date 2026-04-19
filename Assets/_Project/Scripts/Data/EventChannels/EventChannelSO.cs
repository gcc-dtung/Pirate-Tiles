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
            Debug.LogWarning($"[EventChannel] {name}: RaiseEvent blocked — reentrancy detected.");
            return;
        }
        try
        {
            if (eventRaised == null || eventRaised.Count == 0)
            {
#if UNITY_EDITOR
                Debug.Log($"[EventChannel] {name}: RaiseEvent called but no listeners registered.");
#endif
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
                    Debug.LogError($"[EventChannel] {name}: Exception in listener — {ex.Message}");
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
            Debug.LogWarning($"[EventChannel] {name}: AddListener called with null listener.");
            return;
        }
        if (!eventRaised.Contains(listener))
        {
            eventRaised.Add(listener);
#if UNITY_EDITOR
            Debug.Log($"[EventChannel] {name}: Listener added — {listener.gameObject.name}");
#endif
        }
    }





    public void RemoveListener(EventListener<T> listener)
    {
        if (listener == null)
        {
            Debug.LogWarning($"[EventChannel] {name}: RemoveListener called with null listener.");
            return;
        }
        if (eventRaised.Contains(listener))
        {
            eventRaised.Remove(listener);
#if UNITY_EDITOR
            Debug.Log($"[EventChannel] {name}: Listener removed — {listener.gameObject.name}");
#endif
        }
    }
}
