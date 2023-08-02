using DJFrameWork.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;
/// <summary>
/// ��Դ������
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
    /// ��ʼ����Դ��
    /// </summary>
    /// <param name="package"></param>
    public void InitPackage(ResourcePackage package)
    {
        this.package = package;
    }

    /// <summary>
    /// ������Դ
    /// </summary>
    /// <param name="assetName">��Դ���������·��</param>
    /// <param name="callBack">�ص�</param>
    public void LoadAsset(string assetName,Action<UnityEngine.Object> callBack)
    {
        DJSingleton.StartCoroutine(LoadAssetAsync(assetName, callBack));
    }

    /// <summary>
    /// ������Դ
    /// </summary>
    /// <param name="assetName">��Դ��</param>
    /// <param name="callBack">�ص�</param>
    private IEnumerator LoadAssetAsync(string assetName,Action<UnityEngine.Object> callBack)
    {
        if(package == null)
        {
            throw new Exception("Ĭ����Դ��������");
        }
        AssetOperationHandle operationHandle = package.LoadAssetAsync<UnityEngine.Object>(assetName);
        yield return operationHandle;
        if(operationHandle.IsDone && operationHandle.AssetObject != null)
        {
            Debug.Log("��Դ���سɹ�");
            callBack.Invoke(operationHandle.AssetObject);
        }
    }

    /// <summary>
    /// ж����Դ
    /// </summary>
    /// <param name="assetName"></param>
    public void UnLoadAsset(string assetName)
    {
        package.UnloadUnusedAssets();
    }


    /// <summary>
    /// ��ȡUI·��
    /// </summary>
    /// <param name="assetName">��Դ����</param>
    public string GetUIPath(string assetName)
    {
        return string.Format($"Assets/BundleResources/UI/{assetName}/{assetName}.prefab");
    }

    /// <summary>
    /// ��ȡScene·��
    /// </summary>
    /// <param name="sceenName">��������</param>
    /// <returns></returns>
    public string GetScenePath(string sceenName)
    {
        return string.Format($"Assets/BundleResources/Scenes/{sceenName}.unity");
    }
}
