using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.NetWork
{
    /// <summary>
    /// ����Э�� pingЭ�飨�ͻ��˷��ͣ�����˽��գ�
    /// </summary>
    public class MsgPing : MsgBase
    {
        public MsgPing()
        {
            protoName = "MsgPing";
        }

    }
}

