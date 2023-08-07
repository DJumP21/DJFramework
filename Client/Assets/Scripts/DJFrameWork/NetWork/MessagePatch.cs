using DJFrameWork.NetWork;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DJFrameWork.NetWork
{
    /// <summary>
    /// 消息分发器
    /// </summary>
    public class MessagePatch
    {
        public static void PatchMessage(MsgBase msgBase)
        {
            ProtoBuf.IExtensible msg=null;
            if(msgBase.msgPong!=null)
            {
                msg = msgBase.msgPong;
            }
            if(msgBase.loginResponse!=null)
            {
                msg = msgBase.loginResponse;
            }
            NetManager.AddMsgToMsgList(msgBase.msgPong);
        }
    }
}
