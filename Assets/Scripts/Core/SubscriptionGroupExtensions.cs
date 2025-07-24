using System;

public static class SubscriptionGroupExtensions
{
    public static void AddFrom<TBase, TEvent>(
        this SubscriptionGroup group,
        IEventSource<TBase> source,
        Action<TEvent> handler)
        where TEvent : class, TBase
    {
        group.Add(handler, source.SubscribeEvent, source.UnsubscribeEvent);
    }
}