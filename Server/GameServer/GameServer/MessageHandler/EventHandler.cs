using GameServer.NetWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.MessageHandler
{
    /// <summary>
    /// 事件分发器
    /// </summary>
    public partial class EventHandler
    {
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="state"></param>
        public static void OnDisconnect(ClientState state)
        {
            Console.WriteLine($"客户端：{state.socket.RemoteEndPoint} 断开连接");
        }

        /// <summary>
        /// 计时器
        /// </summary>
        public static void OnTimer()
        {

        }
    }
}
