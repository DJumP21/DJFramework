using GameServer.NetWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoMessage;

namespace GameServer.MessageHandler
{
    /// <summary>
    /// 消息分发器
    /// </summary>
    public partial class MsgHandler
    {
        public static void MsgPing(ClientState client,ProtoBuf.IExtensible msg)
        {
            Console.WriteLine("客户端Ping");
            MsgPong msgPong = new MsgPong();
            msgPong.msgName = "MsgPong";
            msgPong.Id = 1;
            MsgBase msgBase = new MsgBase();
            msgBase.msgPong = msgPong;
            NetManager.Send(client, msgBase);
        }
    }
}
