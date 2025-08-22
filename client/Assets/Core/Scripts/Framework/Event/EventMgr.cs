using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace YOTO
{
    public interface IEventInfo { }

    public class EventInfo : IEventInfo
    {
        public UnityAction action;
    }

    public class EventInfo<T> : IEventInfo
    {
        public UnityAction<T> action;
    }

    public class EventInfo<T, U> : IEventInfo
    {
        public UnityAction<T, U> action;
    }

    public class EventInfo<T, U, V> : IEventInfo
    {
        public UnityAction<T, U, V> action;
    }

    public class EventInfo<T, U, V, W> : IEventInfo
    {
        public UnityAction<T, U, V, W> action;
    }

    public class EventMgr
    {
        private Dictionary<YOTOEventType, IEventInfo> eventDictionary = new Dictionary<YOTOEventType, IEventInfo>();

        private void AddListenerInternal<TEventInfo>(YOTOEventType type, Action<TEventInfo> addAction) where TEventInfo : IEventInfo, new()
        {
            if (eventDictionary.TryGetValue(type, out IEventInfo existingEventInfo))
            {
                if (existingEventInfo is TEventInfo eventInfo)
                {
                    addAction(eventInfo);
                }
            }
            else
            {
                var newEventInfo = new TEventInfo();
                addAction(newEventInfo);
                eventDictionary[type] = newEventInfo;
            }
        }

        private void RemoveListenerInternal<TEventInfo>(YOTOEventType type, Action<TEventInfo> removeAction) where TEventInfo : class, IEventInfo
        {
            if (eventDictionary.TryGetValue(type, out IEventInfo existingEventInfo))
            {
                if (existingEventInfo is TEventInfo eventInfo)
                {
                    removeAction(eventInfo);
                    if (IsEventInfoEmpty(eventInfo))
                    {
                        eventDictionary.Remove(type);
                    }
                }
            }
        }

        private bool IsEventInfoEmpty(IEventInfo eventInfo)
        {
            if (eventInfo is EventInfo ei)
                return ei.action == null;
            if (eventInfo is EventInfo<object> ei1)
                return ei1.action == null;
            if (eventInfo is EventInfo<object, object> ei2)
                return ei2.action == null;
            if (eventInfo is EventInfo<object, object, object> ei3)
                return ei3.action == null;
            if (eventInfo is EventInfo<object, object, object, object> ei4)
                return ei4.action == null;

            return false;
        }

        public void AddEventListener(YOTOEventType type, UnityAction action)
        {
            AddListenerInternal<EventInfo>(type, ei => ei.action += action);
        }

        public void RemoveEventListener(YOTOEventType type, UnityAction action)
        {
            RemoveListenerInternal<EventInfo>(type, ei => ei.action -= action);
        }

        public void AddEventListener<T>(YOTOEventType type, UnityAction<T> action)
        {
            AddListenerInternal<EventInfo<T>>(type, ei => ei.action += action);
        }

        public void RemoveEventListener<T>(YOTOEventType type, UnityAction<T> action)
        {
            RemoveListenerInternal<EventInfo<T>>(type, ei => ei.action -= action);
        }

        public void AddEventListener<T, U>(YOTOEventType type, UnityAction<T, U> action)
        {
            AddListenerInternal<EventInfo<T, U>>(type, ei => ei.action += action);
        }

        public void RemoveEventListener<T, U>(YOTOEventType type, UnityAction<T, U> action)
        {
            RemoveListenerInternal<EventInfo<T, U>>(type, ei => ei.action -= action);
        }

        public void AddEventListener<T, U, V>(YOTOEventType type, UnityAction<T, U, V> action)
        {
            AddListenerInternal<EventInfo<T, U, V>>(type, ei => ei.action += action);
        }

        public void RemoveEventListener<T, U, V>(YOTOEventType type, UnityAction<T, U, V> action)
        {
            RemoveListenerInternal<EventInfo<T, U, V>>(type, ei => ei.action -= action);
        }

        public void AddEventListener<T, U, V, W>(YOTOEventType type, UnityAction<T, U, V, W> action)
        {
            AddListenerInternal<EventInfo<T, U, V, W>>(type, ei => ei.action += action);
        }

        public void RemoveEventListener<T, U, V, W>(YOTOEventType type, UnityAction<T, U, V, W> action)
        {
            RemoveListenerInternal<EventInfo<T, U, V, W>>(type, ei => ei.action -= action);
        }

        public void TriggerEvent(YOTOEventType type)
        {
            // 先触发普通事件
            if (eventDictionary.TryGetValue(type, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo)?.action?.Invoke();
            }

   
        }

        public void TriggerEvent<T>(YOTOEventType type, T value)
        {
            if (eventDictionary.TryGetValue(type, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo<T>)?.action?.Invoke(value);
            }

        }

        public void TriggerEvent<T, U>(YOTOEventType type, T value1, U value2)
        {
            if (eventDictionary.TryGetValue(type, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo<T, U>)?.action?.Invoke(value1, value2);
            }
   
        }

        public void TriggerEvent<T, U, V>(YOTOEventType type, T value1, U value2, V value3)
        {
            if (eventDictionary.TryGetValue(type, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo<T, U, V>)?.action?.Invoke(value1, value2, value3);
            }

        }

        public void TriggerEvent<T, U, V, W>(YOTOEventType type, T value1, U value2, V value3, W value4)
        {
            if (eventDictionary.TryGetValue(type, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo<T, U, V, W>)?.action?.Invoke(value1, value2, value3, value4);
            }
        }

        public void ClearEvents()
        {
            eventDictionary.Clear();
        }
    }
}
