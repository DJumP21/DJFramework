using DJFrameWork.Log;
using DJFrameWork.Manager;
using DJFrameWork.Singleton;
using DJFrameWork.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����׼���ڵ�
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
        //���ظ��½���
        DJLog.Log("������Դ����׼������");
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
        PatchManager.Instance.loadingWindow.SetTips("���ڳ�ʼ����Ϸ......");
        yield return new WaitForSeconds(1f);
        machine.ChangeStateNode<InitializeNode>();
    }
}
