using DJFrameWork.Log;
using DJFrameWork.Manager;
using DJFrameWork.Singleton;
using DJFrameWork.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
/// <summary>
/// ������Դ�汾�ڵ�
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
        PatchEvent.PatchStateChange.SendEventMessage("������Դ�汾");
        DJSingleton.StartCoroutine(GetStaticVersion());
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    /// <summary>
    /// ��ȡ��Դ�汾
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetStaticVersion()
    {
        PatchManager.Instance.loadingWindow.SetTips("���ڻ�ȡ��Ϸ�汾��Ϣ....");
        yield return new WaitForSeconds(0.5f);
        var package = YooAssets.TryGetPackage("DefaultPackage");
        var operation = package.UpdatePackageVersionAsync();
        yield return operation;
        if(operation.Status == EOperationStatus.Succeed)
        {
            DJLog.Log("������Դ�汾�ɹ�");
            PatchManager.Instance.packageVersion = operation.PackageVersion;
            //�л���������Դ�嵥�ڵ�
            machine.ChangeStateNode<UpdateManifestNode>();
        }
        else
        {
            DJLog.Error($"������Դ�汾ʧ�ܣ�Error:{operation.Error}");
        }
    }
}
