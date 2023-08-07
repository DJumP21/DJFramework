using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DJFrameWork.Singleton;
using Newtonsoft.Json;
using System.IO;

public class ConfigManager : SingletonInstance<ConfigManager>, ISingleton
{
    public string configPath = "Assets/BundleResources/Configs/";

    public Dictionary<int, CharacterDefine> characterConfigs = new Dictionary<int, CharacterDefine>();

    public void OnCreate(object createParam)
    {
        DJSingleton.StartCoroutine(LoadConfigs());
    }

    public void OnDestroy()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    private IEnumerator LoadConfigs()
    {
        string json=string.Empty;

        json = File.ReadAllText(configPath+"CharacterConfig.txt");
        characterConfigs = JsonConvert.DeserializeObject<Dictionary<int, CharacterDefine>>(json);
        yield return null;


    }
}
