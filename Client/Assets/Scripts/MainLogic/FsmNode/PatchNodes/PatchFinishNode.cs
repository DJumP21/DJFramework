using DJFrameWork.Log;
using DJFrameWork.Manager;
using DJFrameWork.Singleton;
using DJFrameWork.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public class PatchFinishNode : IStateNode
{
    private DJStateMachine machine;
    public void OnCreate(DJStateMachine machine)
    {
        machine = machine;
    }

    public void OnEnter()
    {
        DJSingleton.StartCoroutine(StartHotUpdate());
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    private IEnumerator StartHotUpdate()
    {
        PatchManager.Instance.loadingWindow.SetTips("正在进入游戏....");
        PatchManager.Instance.loadingWindow.HideSlider();
        PatchManager.Instance.loadingWindow.HidePercent();
        yield return new WaitForSeconds(1f);
        //加载热更场景
        string sceneName = "LoginScene";
        var loadScene = YooAssets.LoadSceneAsync(ResManager.Instance.GetScenePath(sceneName));
        yield return loadScene;
        if(loadScene.Status == EOperationStatus.Succeed)
        {
            DJLog.Log($"场景{sceneName} 加载成功");
            //Todo ：执行热更代码
            Type hotUpdateStart = PatchManager.Instance.hotUpdateAssembly.GetType("HotUpdateStart");
            hotUpdateStart.GetMethod("Start").Invoke(null,null);
        }
        

    }
}
