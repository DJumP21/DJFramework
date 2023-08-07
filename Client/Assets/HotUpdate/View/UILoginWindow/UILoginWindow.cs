using DJFrameWork.NetWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 登录界面，界面不按照MVC模式
/// </summary>
public class UILoginWindow : MonoBehaviour
{
    private bool connected = false;
    private bool openNetUpdate =false;
    private void Start()
    {
        LoginController.Instance.OnConnectSuccess();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    public void ConnectToServer()
    {
        string ip = "127.0.0.1";
        int port = 8888;
        NetManager.Connect(ip, port);
        Debug.Log($"连接服务器:{ip}:{port}");
    }
}
