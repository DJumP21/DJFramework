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
    /// �¼�ϵͳ������
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
        //�Ƿ��ʼ��
        private static bool isInit=false;
        //������
        private static GameObject driver;
        //�¼�����������
        private static readonly Dictionary<int, LinkedList<Action<IEventMessage>>> listeners = new Dictionary<int, LinkedList<Action<IEventMessage>>>();
        /// <summary>
        /// �ӳٹ㲥����Ϣ�б�
        /// </summary>
        private static readonly List<PostWarp> postList = new List<PostWarp>();

        /// <summary>
        /// ��ʼ���¼�ϵͳ
        /// </summary>
        public static void Init(UnityEngine.GameObject parent)
        {
            if(isInit)
            {
                throw new Exception($"�¼�ϵͳ�Ѿ���ʼ��");
            }
            else
            {
                isInit = true;
                driver = new UnityEngine.GameObject("DJEvent");
                driver.AddComponent<DJEventDriver>();
                driver.transform.SetParent(parent.transform);
                DJLog.Log("�¼�ϵͳ��ʼ��");
            }
        }

        /// <summary>
        /// �����¼�ϵͳ
        /// </summary>
        public static void Update()
        {
            //�ӳٹ㲥
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
        /// ������м���
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
        /// ����¼�����
        /// </summary>
        /// <typeparam name="TEvnet">�¼���Ϣ</typeparam>
        /// <param name="listener">�¼���Ϣ</param>
        public static void AddListener<TEvnet>(Action<IEventMessage> listener) where TEvnet : IEventMessage
        {
            System.Type eventType = typeof(TEvnet);
            int eventId = eventType.GetHashCode();
            AddListener(eventId, listener);
        }
        /// <summary>
        /// ����¼�
        /// </summary>
        /// <param name="eventType">�¼�����</param>
        /// <param name="listener">�¼�����</param>
        public static void AddListener(Type eventType, Action<IEventMessage> listener)
        {
            int eventId = eventType.GetHashCode();
            AddListener(eventId, listener);
        }
        /// <summary>
        /// ����¼�
        /// </summary>
        /// <param name="eventId">�¼�id</param>
        /// <param name="listener">�¼�</param>
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
        /// �Ƴ��¼�����
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
        /// �Ƴ��¼�����
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listener"></param>
        public static void RemoveListener(Type eventType, Action<IEventMessage> listener)
        {
            int eventId = eventType.GetHashCode();
            RemoveListener(eventId, listener);
        }
        /// <summary>
        /// �Ƴ��¼�����
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
        /// ʵʱ�㲥�¼���Ϣ
        /// </summary>
        /// <param name="message">�¼���Ϣ</param>
        public static void SendMessage(IEventMessage message)
        {
            int eventId = message.GetType().GetHashCode();
            SendMessage(eventId, message);
        }

        /// <summary>
        /// ʵʱ�㲥�¼���Ϣ
        /// </summary>
        /// <param name="eventId">id</param>
        /// <param name="message">�¼���Ϣ</param>
        public static void SendMessage(int eventId, IEventMessage message)
        {
            //�¼�������
            if(!listeners.ContainsKey(eventId))
            {
                return;
            }
            //ʹ������洢 ����Ӻ���ǰִ��
            LinkedList<Action<IEventMessage>> listenerList = listeners[eventId];
            if(listenerList.Count > 0)
            {
                //�Ӻ���ǰ�㲥
                var currentNode = listenerList.Last;
                while(currentNode!=null)
                {
                    currentNode.Value.Invoke(message);
                    currentNode = currentNode.Previous;
                }
            }
        }

        /// <summary>
        /// �ӳٹ㲥�¼���Ϣ
        /// </summary>
        /// <param name="message"></param>
        public static void PostMessage(IEventMessage message)
        {
            int eventId = message.GetType().GetHashCode();
            PostMessage(eventId, message);
        }
        /// <summary>
        /// �ӳٹ㲥��Ϣ
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
