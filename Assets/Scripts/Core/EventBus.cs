using System;
using System.Collections.Generic;
using System.Linq;

public class EventBus
{
    private readonly Dictionary<Type, List<Delegate>> handlers = new();

    public void Subscribe<T>(Action<T> callback) where T : class
    {
        var type = typeof(T);
        if (!handlers.ContainsKey(type))
            handlers[type] = new List<Delegate>();

        handlers[type].Add(callback);
    }

    public void Unsubscribe<T>(Action<T> callback) where T : class
    {
        var type = typeof(T);
        if (handlers.TryGetValue(type, out var list))
            list.Remove(callback);
    }

    public void Emit<T>(T evt) where T : class
    {
        var type = typeof(T);
        if (handlers.TryGetValue(type, out var list))
        {
            foreach (var cb in list.Cast<Action<T>>())
            {
                cb.Invoke(evt);
            }
        }
    }

    public void Clear()
    {
        handlers.Clear();
    }
}