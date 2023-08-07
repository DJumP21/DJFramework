using DJFrameWork.Event;
using DJFrameWork.Manager;
using DJFrameWork.Singleton;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Hosting;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
public class GameStart : MonoBehaviour
{
    /// <summary>
    /// 运行模式
    /// </summary>
    public EPlayMode PlayMode=EPlayMode.EditorSimulateMode;
    /// <summary>
    /// 运行平台
    /// </summary>
    public RuntimePlatform platform;

    private GameObject gameManager;

    private void Awake()
    {
        platform = Application.platform;
        Debug.Log($"资源运行模式:{PlayMode}");
        Debug.Log($"当前平台：{platform}");
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
        DontDestroyOnLoad( gameManager );

        //初始化事件系统
        DJEvent.Init(gameManager);
        //初始化单例系统
        DJSingleton.Init(gameManager);

        //初始化资源加载系统
        YooAssets.Initialize();
        YooAssets.SetOperationSystemMaxTimeSlice(30);

        //加载场景管理器
        DJSingleton.CreateSingleton<MySceneManager>();
        //加载资源管理器
        DJSingleton.CreateSingleton<ResManager>();
        //加载配置管理器
        DJSingleton.CreateSingleton<ConfigManager>();

        DJSingleton.CreateSingleton<TimeManager>();

        //切换到热更场景
        MySceneManager.Instance.LoadScene("Loading", () =>
        {
            //创建更新管理器
            DJSingleton.CreateSingleton<PatchManager>();
            //启动更新流程
            PatchManager.Instance.Run(this.PlayMode);
        });

    }

}
