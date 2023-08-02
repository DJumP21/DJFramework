using DJFrameWork.Log;
using DJFrameWork.Manager;
using DJFrameWork.Singleton;
using DJFrameWork.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
/// <summary>
/// ������Դ�嵥�ڵ�
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
        PatchEvent.PatchStateChange.SendEventMessage("������Դ�嵥");
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
        PatchManager.Instance.loadingWindow.SetTips("���ڸ�����Դ�嵥.....");
        yield return new WaitForSeconds(0.5f);
        string packageName = "DefaultPackage";
        bool savePackageVersion = true;
        var package = YooAssets.TryGetPackage(packageName);
        var operation = package.UpdatePackageManifestAsync(PatchManager.Instance.packageVersion, savePackageVersion);
        yield return operation;
        if(operation.Status == EOperationStatus.Succeed)
        {
            DJLog.Log("��Դ�嵥���³ɹ�");
            //������Դ
            machine.ChangeStateNode<DownLoadWebFilesNode>();
        }
        else
        {
            DJLog.Error($"������Դ�嵥ʧ�ܣ�Error:{operation.Error}");
        }
    }
}
