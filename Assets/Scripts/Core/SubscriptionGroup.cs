using System;
using System.Collections.Generic;

public class SubscriptionGroup
{
    private readonly List<Action> _unsubscribers = new();

    public void Add<T>(Action<T> handler, Action<Action<T>> subscribe, Action<Action<T>> unsubscribe)
    {
        subscribe(handler);
        _unsubscribers.Add(() => unsubscribe(handler));
    }

    public void UnsubscribeAll()
    {
        foreach (var unsubscribe in _unsubscribers)
        {
            unsubscribe?.Invoke();
        }

        _unsubscribers.Clear();
    }
}