using GameServer.MessageHandler;
using ProtoMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.NetWork
{
    public class MessagePatch
    {
        public static void PatchMessage(ClientState state,MsgBase msgBase)
        {
            Console.WriteLine($"分发消息,{msgBase}");
            string methodName = string.Empty;
            //分发消息
            if(msgBase.msgPing!=null)
            {
                methodName = msgBase.msgPing.ToString().Split('.')[1];
                ExcuteMethod(state, methodName, (ProtoBuf.IExtensible)msgBase.msgPing);
            }
           
        }

        private static void ExcuteMethod(ClientState state,string methodName,ProtoBuf.IExtensible msg)
        {
            MethodInfo method = typeof(MsgHandler).GetMethod(methodName);
            Object[] obj = { state, msg };
            Console.WriteLine("收到消息：" + methodName);
            if (method != null)
            {
                method.Invoke(null, obj);
            }
            else
            {
                Console.WriteLine("消息接收失败：" + msg.ToString());
            }
        }
    }
}
