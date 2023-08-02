using DJFrameWork.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;
/// <summary>
/// 资源管理器
/// </summary>
public class ResManager : SingletonInstance<ResManager>,ISingleton
{
    private ResourcePackage package;
    private string assetPath = "Assets/BundleResources/";
    public void OnCreate(object createParam)
    {
        
    }

    public void OnDestroy()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    /// <summary>
    /// 初始化资源包
    /// </summary>
    /// <param name="package"></param>
    public void InitPackage(ResourcePackage package)
    {
        this.package = package;
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="assetName">资源对象的完整路径</param>
    /// <param name="callBack">回调</param>
    public void LoadAsset(string assetName,Action<UnityEngine.Object> callBack)
    {
        DJSingleton.StartCoroutine(LoadAssetAsync(assetName, callBack));
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="assetName">资源名</param>
    /// <param name="callBack">回调</param>
    private IEnumerator LoadAssetAsync(string assetName,Action<UnityEngine.Object> callBack)
    {
        if(package == null)
        {
            throw new Exception("默认资源包不存在");
        }
        AssetOperationHandle operationHandle = package.LoadAssetAsync<UnityEngine.Object>(assetName);
        yield return operationHandle;
        if(operationHandle.IsDone && operationHandle.AssetObject != null)
        {
            Debug.Log("资源加载成功");
            callBack.Invoke(operationHandle.AssetObject);
        }
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="assetName"></param>
    public void UnLoadAsset(string assetName)
    {
        package.UnloadUnusedAssets();
    }


    /// <summary>
    /// 获取UI路径
    /// </summary>
    /// <param name="assetName">资源名称</param>
    public string GetUIPath(string assetName)
    {
        return string.Format($"Assets/BundleResources/UI/{assetName}/{assetName}.prefab");
    }

    /// <summary>
    /// 获取Scene路径
    /// </summary>
    /// <param name="sceenName">场景名称</param>
    /// <returns></returns>
    public string GetScenePath(string sceenName)
    {
        return string.Format($"Assets/BundleResources/Scenes/{sceenName}.unity");
    }
}
