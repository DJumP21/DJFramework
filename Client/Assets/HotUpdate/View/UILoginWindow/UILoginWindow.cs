using DJFrameWork.NetWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��¼���棬���治����MVCģʽ
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
    /// ���ӷ�����
    /// </summary>
    public void ConnectToServer()
    {
        string ip = "127.0.0.1";
        int port = 8888;
        NetManager.Connect(ip, port);
        Debug.Log($"���ӷ�����:{ip}:{port}");
    }
}
