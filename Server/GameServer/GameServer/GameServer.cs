using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameServer.NetWork;
using GameServer.Service;
using GameServer.Manager;
namespace GameServer
{
    public class GameServer
    {
        Thread thread;
        bool running = false;
        
        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            //启动服务器
            NetManager.Start(8888);
            //启动数据库管理器
            DBService.Instance.Init();
            //启动配置表服务
            ConfigManager.Instance.LoadConfigs();



            //主线程和Update
            thread = new Thread(new ThreadStart(this.Update));
            return true;
        }

        /// <summary>
        /// 开启线程
        /// </summary>
        public void Start()
        {
            running = true;
            thread.Start();
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public void Stop()
        {
            running = false;
            thread.Join();
            NetManager.Stop();
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        public void Update()
        {
            while(running)
            {
                Time.Tick();
                //100毫秒跑一帧
                Thread.Sleep(100);
                NetManager.ReadFd();
            }
        }
    }
}
