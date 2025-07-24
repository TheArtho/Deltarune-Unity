using System;

public interface IEventSource<TEventBase>
{
    void SubscribeEvent<T>(Action<T> callback) where T : class, TEventBase;
    void UnsubscribeEvent<T>(Action<T> callback) where T : class, TEventBase;
}