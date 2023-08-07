using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.NetWork
{
    /// <summary>
    /// 客户端信息类,每一个客户端连接会对应一个ClientState对象
    /// </summary>
    public class ClientState
    {
        public Socket socket;
        public ByteArray readBuffer = new ByteArray();
        //玩家数据

    }
}
