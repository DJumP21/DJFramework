using DJFrameWork.Log;
using DJFrameWork.Manager;
using DJFrameWork.Singleton;
using DJFrameWork.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
/// <summary>
/// 更新资源清单节点
/// </summary>
public class UpdateManifestNode : IStateNode
{
    private DJStateMachine machine;
    public void OnCreate(DJStateMachine machine)
    {
        this.machine = machine;
    }

    public void OnEnter()
    {
        PatchEvent.PatchStateChange.SendEventMessage("更新资源清单");
        DJSingleton.StartCoroutine(UpdateManifest());
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    private IEnumerator UpdateManifest()
    {
        PatchManager.Instance.loadingWindow.SetTips("正在更新资源清单.....");
        yield return new WaitForSeconds(0.5f);
        string packageName = "DefaultPackage";
        bool savePackageVersion = true;
        var package = YooAssets.TryGetPackage(packageName);
        var operation = package.UpdatePackageManifestAsync(PatchManager.Instance.packageVersion, savePackageVersion);
        yield return operation;
        if(operation.Status == EOperationStatus.Succeed)
        {
            DJLog.Log("资源清单更新成功");
            //下载资源
            machine.ChangeStateNode<DownLoadWebFilesNode>();
        }
        else
        {
            DJLog.Error($"更新资源清单失败，Error:{operation.Error}");
        }
    }
}
