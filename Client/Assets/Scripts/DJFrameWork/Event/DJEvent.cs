using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DJFrameWork.Log;
using System.Runtime.Remoting.Messaging;
using System.Linq;

namespace DJFrameWork.Event
{
    /// <summary>
    /// 事件系统管理器
    /// </summary>
    public static class DJEvent
    {
        private class PostWarp
        {
            public int PostFrame;
            public int EventId;
            public IEventMessage Message;

            public void OnRelease()
            {
                PostFrame = 0;
                EventId = 0;
                Message = null;
            }
        }
        //是否初始化
        private static bool isInit=false;
        //启动器
        private static GameObject driver;
        //事件监听器缓存
        private static readonly Dictionary<int, LinkedList<Action<IEventMessage>>> listeners = new Dictionary<int, LinkedList<Action<IEventMessage>>>();
        /// <summary>
        /// 延迟广播的消息列表
        /// </summary>
        private static readonly List<PostWarp> postList = new List<PostWarp>();

        /// <summary>
        /// 初始化事件系统
        /// </summary>
        public static void Init(UnityEngine.GameObject parent)
        {
            if(isInit)
            {
                throw new Exception($"事件系统已经初始化");
            }
            else
            {
                isInit = true;
                driver = new UnityEngine.GameObject("DJEvent");
                driver.AddComponent<DJEventDriver>();
                driver.transform.SetParent(parent.transform);
                DJLog.Log("事件系统初始化");
            }
        }

        /// <summary>
        /// 更新事件系统
        /// </summary>
        public static void Update()
        {
            //延迟广播
            for(int i=postList.Count-1; i>=0; i--)
            {
                var warpPost = postList[i];
                if(UnityEngine.Time.frameCount > warpPost.PostFrame)
                {
                    SendMessage(warpPost.EventId, warpPost.Message);
                    postList.RemoveAt(i);
                }
            }
        }


        public static void Destroy()
        {
            if(isInit)
            {
                ClearAll();
                isInit = false;
                UnityEngine.GameObject.Destroy(driver);
                DJLog.Log("EventDriver Destory All");
            }
        }

        /// <summary>
        /// 清空所有监听
        /// </summary>
        public static void ClearAll()
        {
            foreach(var eventId in listeners.Keys)
            {
                listeners[eventId].Clear();
            }
            listeners.Clear();
            postList.Clear();
        }
        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <typeparam name="TEvnet">事件消息</typeparam>
        /// <param name="listener">事件消息</param>
        public static void AddListener<TEvnet>(Action<IEventMessage> listener) where TEvnet : IEventMessage
        {
            System.Type eventType = typeof(TEvnet);
            int eventId = eventType.GetHashCode();
            AddListener(eventId, listener);
        }
        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="listener">事件监听</param>
        public static void AddListener(Type eventType, Action<IEventMessage> listener)
        {
            int eventId = eventType.GetHashCode();
            AddListener(eventId, listener);
        }
        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="eventId">事件id</param>
        /// <param name="listener">事件</param>
        public static void AddListener(int eventId, Action<IEventMessage> listener)
        {
            if(!listeners.ContainsKey(eventId))
            {
                listeners.Add(eventId,new LinkedList<Action<IEventMessage>>());
            }
            if (!listeners[eventId].Contains(listener))
            {
                listeners[eventId].AddLast(listener);
            }
        }
        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="listener"></param>
        public static void RemoveListener<TEvent>(Action<IEventMessage> listener) where TEvent : IEventMessage
        {
            System.Type eventType = typeof (TEvent);
            int eventId = eventType.GetHashCode();
            RemoveListener(eventId, listener);
        }
        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        public static void RemoveListener(Type eventType, Action<IEventMessage> listener)
        {
            int eventId = eventType.GetHashCode();
            RemoveListener(eventId, listener);
        }
        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="listener"></param>
        public static void RemoveListener(int eventId,Action<IEventMessage> listener)
        {
            if(listeners.ContainsKey(eventId))
            {
                if (listeners[eventId].Contains(listener))
                {
                    listeners[eventId].Remove(listener);
                }
            }
        }

        /// <summary>
        /// 实时广播事件消息
        /// </summary>
        /// <param name="message">事件消息</param>
        public static void SendMessage(IEventMessage message)
        {
            int eventId = message.GetType().GetHashCode();
            SendMessage(eventId, message);
        }

        /// <summary>
        /// 实时广播事件消息
        /// </summary>
        /// <param name="eventId">id</param>
        /// <param name="message">事件消息</param>
        public static void SendMessage(int eventId, IEventMessage message)
        {
            //事件不存在
            if(!listeners.ContainsKey(eventId))
            {
                return;
            }
            //使用链表存储 方便从后往前执行
            LinkedList<Action<IEventMessage>> listenerList = listeners[eventId];
            if(listenerList.Count > 0)
            {
                //从后往前广播
                var currentNode = listenerList.Last;
                while(currentNode!=null)
                {
                    currentNode.Value.Invoke(message);
                    currentNode = currentNode.Previous;
                }
            }
        }

        /// <summary>
        /// 延迟广播事件消息
        /// </summary>
        /// <param name="message"></param>
        public static void PostMessage(IEventMessage message)
        {
            int eventId = message.GetType().GetHashCode();
            PostMessage(eventId, message);
        }
        /// <summary>
        /// 延迟广播消息
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        public static void PostMessage(int eventId, IEventMessage message)
        {
            var wrapper = new PostWarp();
            wrapper.PostFrame = UnityEngine.Time.frameCount;
            wrapper.EventId = eventId;
            wrapper.Message = message;
            postList.Add(wrapper);
        }
    }

}
