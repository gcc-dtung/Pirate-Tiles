using System;
using UnityEngine;

[CreateAssetMenu(menuName = "PirateTiles/Events/VoidEventChannel")]
public class VoidEventChannelSO : ScriptableObject
{
    
    //-----Reentrancy Safe-----
    private bool _isRunning;
    
    //-----Event-----
    public delegate void OnHandler();
    private event OnHandler EventRaised;

    public void EventRaise()
    {
        if (_isRunning)
        {
            return;
        }
        try
        {
            if (EventRaised == null)
            {
                return;
            }
            _isRunning = true;
            Delegate[] listener = EventRaised.GetInvocationList();
            foreach (var lis in listener)
            {
                try
                {
                    ((OnHandler)lis)?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError("RaiseEvent called but exception: " + e.Message);
                }
            }
        }
        finally
        {
            _isRunning = false;
        }
    }

    public void AddListener(OnHandler listener)
    {
        if (listener == null)
        {
            return;
        }
        EventRaised += listener;
    }

    public void RemoveListener(OnHandler listener)
    {
        if (listener == null)
        {
            return;
        }
        EventRaised -= listener;
    }
    
}