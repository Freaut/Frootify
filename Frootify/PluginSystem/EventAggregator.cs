using System;
using System.Collections.Generic;

namespace Frootify.PluginSystem;

public class EventAggregator
{
    private readonly Dictionary<Type, List<object>> _subscribers = new Dictionary<Type, List<object>>();

    public void Subscribe<TEvent>(Action<TEvent> subscriber) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);

        if (!_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType] = new List<object>();
        }

        _subscribers[eventType].Add(subscriber);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> subscriber) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);

        if (_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType].Remove(subscriber);
        }
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);

        if (_subscribers.ContainsKey(eventType))
        {
            foreach (var subscriber in _subscribers[eventType])
            {
                ((Action<TEvent>)subscriber).Invoke(@event);
            }
        }
    }
}