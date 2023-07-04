using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AutoBot.Services
{

    public class EventBus
    {
        public ConcurrentDictionary<Type, Dictionary<Type, List<Action>>> _subscriberList;

        public EventBus()
        {
            _subscriberList = new();
        }

        public void Subscribe<T> (object subscriber, Action callback) where T : class
        {
            if (!_subscriberList.TryGetValue(subscriber.GetType(), out var list))
            {
                _subscriberList[subscriber.GetType()] = new Dictionary<Type, List<Action>>();

                _subscriberList[subscriber.GetType()][typeof(T)] = new List<Action>() { callback };
            } else
            {
                list[subscriber.GetType()].Add(callback);
            }
        }

        public void Publish(object T, BaseEvent evt)
        {
            foreach(var subscriber in _subscriberList.Keys)
            {
                if (_subscriberList[subscriber].Keys.Contains(evt.GetType()))
                {
                    foreach(var callback in _subscriberList[subscriber][evt.GetType()])
                    {
                        callback.Invoke();
                    }
                }
               
            }
 
        }
    }
}
