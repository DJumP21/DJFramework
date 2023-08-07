using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GameServer.Configs;
using System.IO;

namespace GameServer.Manager
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    public class ConfigManager:Singleton<ConfigManager>
    {
        private string path = System.AppDomain.CurrentDomain.BaseDirectory+"Configs/";
        public Dictionary<int, CharacterConfig> characterConfigs = new Dictionary<int, CharacterConfig>();

        public void LoadConfigs()
        {
            Console.WriteLine("读取配置表");
            string json;
            json = File.ReadAllText(path + "CharacterConfig.txt");
            characterConfigs = JsonConvert.DeserializeObject<Dictionary<int, CharacterConfig>>(json);
        }

    }
}
