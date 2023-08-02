using DJFrameWork.Log;
using DJFrameWork.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.Singleton
{
    /// <summary>
    /// µ¥Àý»ùÀà
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonInstance<T> where T : class, ISingleton
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    DJLog.Error($"{typeof(T)} was not create,please use DJSingleton.Create() to create");
                }
                return instance;
            }
        }

        protected SingletonInstance()
        {
            if(instance!=null)
            {
                DJLog.Log($"{typeof(T)} is already loaded");
            }
            instance = this as T;
        }

        protected void DestroyInstance()
        {
            instance = null;
        }
    }
}

