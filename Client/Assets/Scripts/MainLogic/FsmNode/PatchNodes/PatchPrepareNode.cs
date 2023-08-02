using DJFrameWork.Log;
using DJFrameWork.Manager;
using DJFrameWork.Singleton;
using DJFrameWork.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 更新准备节点
/// </summary>
public class PatchPrepareNode : IStateNode
{
    private DJStateMachine machine;

    public void OnCreate(DJStateMachine machine)
    {
        this.machine = machine;
    }

    public void OnEnter()
    {
        //加载更新界面
        DJLog.Log("进入资源更新准备界面");
        PatchManager.Instance.loadingWindow = UnityEngine.GameObject.Find("LoadingWindow").GetComponent<LoadingWindow>();
        if (PatchManager.Instance.loadingWindow == null)
        {
            DJLog.Error("LoadingWindwo is not exist");
            return;
        }
        DJSingleton.StartCoroutine(OnPrepared());
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    private IEnumerator OnPrepared()
    {
        PatchManager.Instance.loadingWindow.HideSlider();
        PatchManager.Instance.loadingWindow.HidePercent();
        PatchManager.Instance.loadingWindow.SetTips("正在初始化游戏......");
        yield return new WaitForSeconds(1f);
        machine.ChangeStateNode<InitializeNode>();
    }
}
