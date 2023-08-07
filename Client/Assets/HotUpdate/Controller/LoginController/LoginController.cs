using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DJFrameWork.NetWork;
using DJFrameWork.Log;
using ProtoBuf;
using System;
/// <summary>
/// ��¼������
/// </summary>
public class LoginController:Singletion<LoginController>
{
    public NetManager.EventListener connectSuccess;
    public NetManager.EventListener connectFailed;

    /// <summary>
    /// ���ӷ������ɹ�
    /// </summary>
    public void OnConnectSuccess()
    {
        connectSuccess += ConnectSuccess;
        connectFailed += ConnectFailed;
        NetManager.AddEventListener(NetEvent.ConnectSuccess, connectSuccess);
        NetManager.AddEventListener(NetEvent.ConnectFailed, connectFailed);
    }

    private void ConnectSuccess(string err)
    {
        //������ʱ��
        TimeManager.Instance.OpenTime();
    }

    private void ConnectFailed(string err)
    {
        DJLog.Log("���ӷ�����ʧ��");
    }

}
