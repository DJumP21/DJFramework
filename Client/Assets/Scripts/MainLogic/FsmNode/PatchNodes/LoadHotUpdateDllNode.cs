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
/// ����DLL�ȸ�
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

    //Ԫ����Assembly
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
    /// �����ȸ�dll �� ����Ԫ����dll
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadDlls()
    {
        yield return new WaitForSeconds(0.5f);
        DJLog.Log("�����ȸ�dll");

        // Editor�����£�HotUpdate.dll.bytes�Ѿ����Զ����أ�����Ҫ���أ��ظ����ط���������⡣
#if !UNITY_EDITOR
        PatchManager.Instance.hotUpdateAssembly = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate.dll.bytes"));
#else
        //���ȸ�dll ���浽�ȸ����������� �������ڵ����
        PatchManager.Instance.hotUpdateAssembly = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#endif

        yield return LoadAotDlls();
        //�л����ȸ������ڵ�
        machine.ChangeStateNode<PatchFinishNode>();
    }

    /// <summary>
    /// ����Ԫ����dll
    /// </summary>
    private IEnumerator LoadAotDlls()
    {
        DJLog.Log("����Ԫ����dll");
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
                    DJLog.Log($"Ԫ����dll��{path} ���سɹ�");
                }
                else
                {
                    Debug.LogError($"LoadMetadataForAotAssembly:{aotDllName},result:{err}");
                }
            }
        }
    }
}
