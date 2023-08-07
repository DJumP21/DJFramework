using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class DBService:Singleton<DBService>
    {
        private TestServerEntities entities;
        public TestServerEntities Entities
        {
            get
            {
                return entities;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            entities = new TestServerEntities();
            Console.WriteLine("DBService Started...");
        }

        /// <summary>
        /// 保存数据库
        /// </summary>
        /// <param name="isAsync"></param>
        public void SaveDB(bool isAsync=false)
        {
            if(isAsync)
            {
                entities.SaveChangesAsync();
            }
            else
            {
                entities.SaveChanges();
            }
        }
    }
}
