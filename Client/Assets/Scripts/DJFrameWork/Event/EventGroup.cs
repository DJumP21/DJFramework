using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.Event
{
    /// <summary>
    /// �¼���
    /// </summary>
    public class EventGroup
    {
        //����ļ���
        private readonly Dictionary<System.Type,List<Action<IEventMessage>>> cacheListener = new Dictionary<Type, List<Action<IEventMessage>>> ();
        /// <summary>
        /// ���һ������
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        public void AddListener<TEvent>(Action<IEventMessage> listener) where TEvent : IEventMessage
        {
            System.Type eventType = typeof(TEvent);
            if(!cacheListener.ContainsKey(eventType))
            {
                cacheListener.Add(eventType, new List<Action<IEventMessage>>());
            }
            if (!cacheListener[eventType].Contains(listener))
            {
                cacheListener[eventType].Add(listener);
                DJEvent.AddListener(eventType, listener);
            }
        }

        /// <summary>
        /// �Ƴ������е����м���
        /// </summary>
        public void RemoveAllListeners()
        {
            foreach(var pair in cacheListener)
            {
                System.Type eventType = pair.Key;
                for(int i=0;i<pair.Value.Count;i++)
                {
                    DJEvent.RemoveListener(eventType, pair.Value[i]);
                }
                pair.Value.Clear();
            }
            cacheListener.Clear();
        }
    }
}

