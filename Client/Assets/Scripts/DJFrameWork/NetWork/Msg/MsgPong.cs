using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DJFrameWork.NetWork
{
    /// <summary>
    /// 心跳机制 pong协议(服务端发送，客户端接收)
    /// </summary>
    public class MsgPong : MsgBase
    {
        public MsgPong() 
        {
            protoName = "MsgPong";
        }
    }
}

