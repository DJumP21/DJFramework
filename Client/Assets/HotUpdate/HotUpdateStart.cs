using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DJFrameWork.Singleton;
using UnityEngine;
/// <summary>
/// 游戏启动入口
/// </summary>
public class HotUpdateStart
{
    /// <summary>
    /// 游戏启动
    /// </summary>
    public static void Start()
    {
        InitGame();
    }

    /// <summary>
    /// 初始化游戏
    /// </summary>
    private static void InitGame()
    {
        DJSingleton.CreateSingleton<WindowManager>();
        WindowManager.Instance.OpenWindow(WindowType.StoreWindow);
    }
}
