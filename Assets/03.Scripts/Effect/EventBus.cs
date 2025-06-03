using System;
using System.Collections.Generic;
using UnityEngine;


public struct SlashHitEvent
{
    public float revoverValue;
}

public struct DeathEvent
{
    public GameObject target;
    public float duration;

    public EffectRequest req;
}

public static class EventBus
{
    private static Dictionary<Type, List<Delegate>> listeners = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        Type type = typeof(T);
        if (!listeners.ContainsKey(type))
            listeners[type] = new List<Delegate>();

        listeners[type].Add(callback);
    }

    public static void Publish<T>(T evt)
    {
        Type type = typeof(T);
        if (listeners.TryGetValue(type, out var list))
        {
            foreach (var cb in list)
                ((Action<T>)cb)?.Invoke(evt);
        }
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        Type type = typeof(T);
        if (listeners.TryGetValue(type, out var list))
            list.Remove(callback);
    }
}
