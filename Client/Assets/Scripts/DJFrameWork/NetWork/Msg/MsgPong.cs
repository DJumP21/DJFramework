using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DJFrameWork.NetWork
{
    /// <summary>
    /// �������� pongЭ��(����˷��ͣ��ͻ��˽���)
    /// </summary>
    public class MsgPong : MsgBase
    {
        public MsgPong() 
        {
            protoName = "MsgPong";
        }
    }
}

