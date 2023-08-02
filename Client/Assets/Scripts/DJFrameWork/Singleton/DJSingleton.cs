using DJFrameWork.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace DJFrameWork.Singleton
{
    /// <summary>
    /// 单例管理器
    /// </summary>
    public static class DJSingleton
    {
        public class Warp
        {
            public int Priority { private set; get; }
            public ISingleton Singleton { private set; get; }
            public Warp(ISingleton singleton,int priority) 
            { 
                Singleton = singleton;
                Priority = priority;
            }
        }

        private static bool isInit = false;
        private static GameObject driver;
        private static readonly List<Warp> warps = new List<Warp>();
        private static MonoBehaviour monoBehaviour;
        //是否需要重新排序
        private static bool isDrity=false;

        /// <summary>
        /// 初始化单例管理
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void Init(UnityEngine.GameObject parent)
        {
            if (isInit)
                throw new Exception($"{nameof(DJSingleton)} is initialized !");
            else
            {
                isInit = true;
                driver = new UnityEngine.GameObject("DJSingleton");
                monoBehaviour = driver.AddComponent<SingletonDriver>();
                driver.transform.SetParent(parent.transform);
                DJLog.Log($"单例系统初始化");
            }
        }

        /// <summary>
        /// 更新单例
        /// </summary>
        public static void Update()
        {
            //如果需要重新排序
            if(isDrity)
            {
                isDrity = false;
                //从小到大排序
                warps.Sort((left, right) =>
                {
                    if(left.Priority > right.Priority)   //左大  右小 交换位置
                    {
                        return -1;
                    }else if(left.Priority == right.Priority)
                    {
                        return 0;
                    }
                    else    //左小  右大 不交换位置
                    {
                        return 1;
                    }
                });
                //轮询所有单例
                for(int i=0;i<warps.Count;i++)
                {
                    warps[i].Singleton.OnUpdate();
                }
            }
        }

        /// <summary>
        /// 销毁单例管理
        /// </summary>
        public static void Destroy()
        {
            if(isInit)
            {
                DestyorAll();
                isInit = false;
                if(driver != null)
                {
                    UnityEngine.GameObject.Destroy(driver);
                }
                DJLog.Log("单例管理销毁");
            }
        }
        /// <summary>
        /// 销毁所有单例
        /// </summary>
        private static void DestyorAll()
        {
            for(int i=0;i<warps.Count;i++)
            {
                warps[i].Singleton.OnDestroy();
            }
            warps.Clear();
        }

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetInstance<T>() where T : class,ISingleton
        {
            System.Type type = typeof(T);
            for(int i=0;i<warps.Count; i++)
            {
                if (warps[i].GetType() == type)
                {
                    return warps[i].GetType() as T;
                }
            }
            DJLog.Log($"{typeof(T)} 单例不存在");
            return null;
        }

        /// <summary>
        /// 判断是否存在单例T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Contains<T>() where T : class,ISingleton
        {
            System.Type type = typeof(T);
            for (int i = 0; i < warps.Count; i++)
            {
                if (warps[i].GetType() == type)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 创建单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="priority">运行优先级  优先级越高，越早执行</param>
        public static T CreateSingleton<T>(int priority = 0) where T : class,ISingleton
        {
            return CreateSingleton<T>(null, priority);
        }

        /// <summary>
        /// 创建单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param">附加参数</param>
        /// <param name="priority">运行优先级  优先级越高，越早执行，如果没有设置优先级，则按照添加顺序执行</param>
        /// <returns></returns>
        public static T CreateSingleton<T>(System.Object param,int priority) where T : class,ISingleton
        {
            if(priority<0)
            {
                throw new Exception("单例运行优先级不能小于0");
            }
            if(Contains<T>())
            {
                throw new Exception($"单例{typeof(T)}已经存在");
            }
            if(priority==0)
            {
                int minPriority = GetMinPriority();
                priority = --minPriority;
            }
            T module = Activator.CreateInstance<T>();
            Warp warp = new Warp(module, priority);
            warp.Singleton.OnCreate(param);
            warps.Add(warp);
            //添加新的单例之后  重新排序
            isDrity = true;
            return module;
        }

        /// <summary>
        /// 获取最小的优先级
        /// </summary>
        /// <returns></returns>
        public static int GetMinPriority()
        {
            int minPrority = 0;
            for(int i=0;i<warps.Count;i++)
            {
                if (warps[i].Priority<minPrority)
                {
                    minPrority = warps[i].Priority;
                }
            }
            return minPrority;
        }

        /// <summary>
        /// 开启一个协程
        /// </summary>
        /// <param name="coroutine">协程</param>
        /// <returns></returns>
        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return monoBehaviour.StartCoroutine(coroutine);
        }
        /// <summary>
        /// 开启一个协程
        /// </summary>
        /// <param name="methodName">协程名</param>
        /// <returns></returns>
        public static Coroutine StartCoroutine(string methodName)
        {
            return monoBehaviour.StartCoroutine(methodName);
        }
        /// <summary>
        /// 停止一个协程
        /// </summary>
        /// <param name="coroutine">协程</param>
        public static void StopCoroutine(IEnumerator coroutine)
        {
            monoBehaviour.StopCoroutine(coroutine);
        }
        /// <summary>
        /// 停止一个协程
        /// </summary>
        /// <param name="methodName">协程名</param>
        public static void StopCoroutine(string methodName)
        {
            monoBehaviour.StopCoroutine(methodName);
        }
        /// <summary>
        /// 停止所有协程
        /// </summary>
        public static void StopAllCoroutine()
        {
            monoBehaviour.StopAllCoroutines();
        }
    }
}

