using DJFrameWork.Log;
using DJFrameWork.Manager;
using DJFrameWork.Singleton;
using DJFrameWork.StateMachine;
using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YooAsset;

/// <summary>
/// 加载DLL热更
/// </summary>
public class LoadHotUpdateDllNode : IStateNode
{
    private DJStateMachine machine;
    public void OnCreate(DJStateMachine machine)
    {
        this.machine = machine;
    }

    public void OnEnter()
    {
        DJSingleton.StartCoroutine(LoadDlls());
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    //元数据Assembly
    private static string[] AotAssemblyDlls = new string[]
   {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
        "UnityEngine.UI.dll",
        "Unity.TextMeshPro.dll",
        "YooAsset.dll",
        "UniTask.dll",
        "DJFramework.dll",
   };

    /// <summary>
    /// 加载热更dll 并 补充元数据dll
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadDlls()
    {
        yield return new WaitForSeconds(0.5f);
        DJLog.Log("加载热更dll");

        // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
        PatchManager.Instance.hotUpdateAssembly = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate.dll.bytes"));
#else
        //将热更dll 缓存到热更管理器汇总 方便后面节点调用
        PatchManager.Instance.hotUpdateAssembly = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#endif

        yield return LoadAotDlls();
        //切换到热更结束节点
        machine.ChangeStateNode<PatchFinishNode>();
    }

    /// <summary>
    /// 补充元数据dll
    /// </summary>
    private IEnumerator LoadAotDlls()
    {
        DJLog.Log("补充元数据dll");
        for (int i = 0; i < AotAssemblyDlls.Length; i++)
        {
            string aotDllName = AotAssemblyDlls[i];
            string path = $"Assets/BundleResources/Codes/{aotDllName}.bytes";
            var asset =  YooAssets.TryGetPackage("DefaultPackage").LoadAssetAsync<TextAsset>($"Assets/BundleResources/Codes/{aotDllName}.bytes");
            yield return asset;
            if (asset.IsDone)
            {
                TextAsset textAsset = asset.AssetObject as TextAsset;
                byte[] bytes = textAsset.bytes;
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(bytes,HomologousImageMode.SuperSet);
                if (err == LoadImageErrorCode.OK)
                {
                    DJLog.Log($"元数据dll：{path} 加载成功");
                }
                else
                {
                    Debug.LogError($"LoadMetadataForAotAssembly:{aotDllName},result:{err}");
                }
            }
        }
    }
}
