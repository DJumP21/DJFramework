using DJFrameWork.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View;

public class WindowManager : SingletonInstance<WindowManager>,ISingleton
{
    private Dictionary<WindowType, BaseWindow> windowDic = new Dictionary<WindowType, BaseWindow>();

    public void OnCreate(object createParam)
    {
        UIRoot.Init();

        //商店
        windowDic.Add(WindowType.StoreWindow, new UIStoreWindow());
        //登录
    }

    public void OnUpdate()
    {
        foreach (var window in windowDic.Values)
        {
            if (window.IsVisible())
            {
                window.Update(Time.deltaTime);
            }
        }
    }

    public void OnDestroy()
    {
        
    }


    /// <summary>
    /// 打开窗体
    /// </summary>
    /// <param name="type">窗体类型</param>
    /// <returns></returns>
    public BaseWindow OpenWindow(WindowType type)
    {
        BaseWindow window;
        if (windowDic.TryGetValue(type, out window))
        {
            window.Open();
            return window;
        }
        else
        {
            Debug.LogError($"Open Error:{type}");
            return null;
        }
    }

    /// <summary>
    /// 关闭窗体
    /// </summary>
    /// <param name="type">窗体类型</param>
    public void CloseWindow(WindowType type)
    {
        BaseWindow window;
        if (windowDic.TryGetValue(type, out window))
        {
            window.Close();
        }
        else
        {
            Debug.LogError($"Close Error:{type}");
        }
    }
    /// <summary>
    /// 根据场景类型，预加载场景内的窗体
    /// </summary>
    /// <param name="type">场景类型</param>
    public void PreLoadWindow(ScenesType type)
    {
        foreach (var item in windowDic.Values)
        {
            if (item.GetSceneType() == type)
            {
                item.PreLoad();
            }
        }
    }
    
    /// <summary>
    /// 关闭场景中的所有窗体
    /// </summary>
    /// <param name="type">场景类型</param>
    /// <param name="isForceClose">是否强制关闭</param>
    public void HideAllWindow(ScenesType type,bool isForceClose=false)
    {
        foreach (var item in windowDic.Values)
        {
            if (item.GetSceneType() == type)
            {
                item.Close(isForceClose);
            }
        }
    }
    
}
