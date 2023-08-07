using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DJFrameWork.NetWork;
using DJFrameWork.Log;
using ProtoBuf;
using System;
/// <summary>
/// 登录控制器
/// </summary>
public class LoginController:Singletion<LoginController>
{
    public NetManager.EventListener connectSuccess;
    public NetManager.EventListener connectFailed;

    /// <summary>
    /// 连接服务器成功
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
        //开启计时器
        TimeManager.Instance.OpenTime();
    }

    private void ConnectFailed(string err)
    {
        DJLog.Log("连接服务器失败");
    }

}
