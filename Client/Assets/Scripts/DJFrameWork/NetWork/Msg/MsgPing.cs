using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.NetWork
{
    /// <summary>
    /// 心跳协议 ping协议（客户端发送，服务端接收）
    /// </summary>
    public class MsgPing : MsgBase
    {
        public MsgPing()
        {
            protoName = "MsgPing";
        }

    }
}

