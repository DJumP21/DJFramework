using GameServer.NetWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Service;

namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("启动服务器...");
            //启动服务器
            GameServer gameServer = new GameServer();
            gameServer.Init();
            gameServer.Start();

        }
    }
}
