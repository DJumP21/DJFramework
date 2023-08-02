using DJFrameWork.Log;
using DJFrameWork.Manager;
using DJFrameWork.Singleton;
using DJFrameWork.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
/// <summary>
/// 更新资源版本节点
/// </summary>
public class UpdateVersionNode : IStateNode
{
    private DJStateMachine machine;
    public void OnCreate(DJStateMachine machine)
    {
        this.machine = machine;
    }

    public void OnEnter()
    {
        PatchEvent.PatchStateChange.SendEventMessage("更新资源版本");
        DJSingleton.StartCoroutine(GetStaticVersion());
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    /// <summary>
    /// 获取资源版本
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetStaticVersion()
    {
        PatchManager.Instance.loadingWindow.SetTips("正在获取游戏版本信息....");
        yield return new WaitForSeconds(0.5f);
        var package = YooAssets.TryGetPackage("DefaultPackage");
        var operation = package.UpdatePackageVersionAsync();
        yield return operation;
        if(operation.Status == EOperationStatus.Succeed)
        {
            DJLog.Log("更新资源版本成功");
            PatchManager.Instance.packageVersion = operation.PackageVersion;
            //切换到更新资源清单节点
            machine.ChangeStateNode<UpdateManifestNode>();
        }
        else
        {
            DJLog.Error($"更新资源版本失败，Error:{operation.Error}");
        }
    }
}
