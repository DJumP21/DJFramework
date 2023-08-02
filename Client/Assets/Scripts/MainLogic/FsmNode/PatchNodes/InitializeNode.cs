using Cysharp.Threading.Tasks;
using DJFrameWork.Log;
using DJFrameWork.Manager;
using DJFrameWork.Singleton;
using DJFrameWork.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;

/// <summary>
/// ��Դ����ʼ���ڵ�
/// </summary>
public class InitializeNode : IStateNode
{
    private DJStateMachine machine;
    private bool isInit = false;
    public void OnCreate(DJStateMachine machine)
    {
        this.machine = machine;
    }

    public void OnEnter()
    {
        PatchManager.Instance.loadingWindow.SetTips("���ڳ�ʼ����Դ��.....");
        PatchEvent.PatchStateChange.SendEventMessage("��ʼ����Դ��");
        DJSingleton.StartCoroutine(InitPackage());
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    /// <summary>
    /// ��ʼ����Դ��
    /// </summary>
    /// <returns></returns>
    public IEnumerator InitPackage()
    {
        PatchManager.Instance.loadingWindow.SetTips("��ʼ����Ϸ��Դ....");
        yield return new WaitForSeconds(1f);
        if (!isInit)
        {
            isInit = true;

            var playMode = PatchManager.Instance.playMode;

            //����Ĭ�ϵ���Դ��
            string packageName = "DefaultPackage";
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(package);
            }
            InitializationOperation operation = null;
            //�༭��ģʽ
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                var createParamters = new EditorSimulateModeParameters();
                createParamters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
                operation = package.InitializeAsync(createParamters);
            }
            //����ģʽ
            if (playMode == EPlayMode.OfflinePlayMode)
            {
                var createParamters = new OfflinePlayModeParameters();
                createParamters.DecryptionServices = new GameDecryptionServices();
                operation = package.InitializeAsync(createParamters);
            }
            //����ģʽ
            if (playMode == EPlayMode.HostPlayMode)
            {
                string defaultHostServer = GetHostServerUrl();
                string fallBackHostServer = GetHostServerUrl();
                var createParamters = new HostPlayModeParameters();
                createParamters.DecryptionServices = new GameDecryptionServices();
                createParamters.QueryServices = new GameQueryServices();
                createParamters.RemoteServices = new RemoteServices(defaultHostServer, fallBackHostServer);
                operation = package.InitializeAsync(createParamters);
            }
            //WebGLģʽ
            if (playMode == EPlayMode.WebPlayMode)
            {
                string defaultHostServer = GetHostServerUrl();
                string fallBackHostServer = GetHostServerUrl();
                var createParamters = new WebPlayModeParameters();
                createParamters.DecryptionServices = new GameDecryptionServices();
                createParamters.QueryServices = new GameQueryServices();
                createParamters.RemoteServices = new RemoteServices(defaultHostServer, fallBackHostServer);
                operation = package.InitializeAsync(createParamters);
            }
            yield return operation;
            if (operation.Status == EOperationStatus.Succeed)
            {
                DJLog.Log("��Դ����ʼ���ɹ�");
                //����Դ����ֵ��ResManager
                ResManager.Instance.InitPackage(package);
                //�л�������״̬�ڵ�
                machine.ChangeStateNode<UpdateVersionNode>();
            }
            else
            {
                DJLog.Warning($"{operation.Error}");
                PatchEvent.InitializeFailed.SendEventMessage();
            }
        }
        
    }

    /// <summary>
    /// ��ȡ��Դ��������ַ
    /// </summary>
    /// <returns></returns>
    private string GetHostServerUrl()
    {
        //string hostServerIP = "http://10.0.0.2.2"; //��׿ģ������ַ
        string hostServerIP = "http://127.0.0.1";
        string appVersion = "v1.0.0.2";
#if UNITY_EDITOR
        if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
        {
            return $"{hostServerIP}/CDN/Android/DefaultPackage/{appVersion}";
        }
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
        {
            return $"{hostServerIP}/CDN/IPhone/DefaultPackage/{appVersion}";
        }
        else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
        {
            return $"{hostServerIP}/CDN/WebGL/DefaultPackage/{appVersion}";
        }
        else 
        {
            return $"{hostServerIP}/CDN/PC/DefaultPackage/{appVersion}";
        }
#else
        if (Application.platform == RuntimePlatform.Android)
			return $"{hostServerIP}/CDN/Android/DefaultPackage/{appVersion}";
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			return $"{hostServerIP}/CDN/IPhone/DefaultPackage/{appVersion}";
		else if (Application.platform == RuntimePlatform.WebGLPlayer)
			return $"{hostServerIP}/CDN/WebGL/DefaultPackage/{appVersion}";
		else
			return $"{hostServerIP}/CDN/PC/DefaultPackage/{appVersion}";
#endif
    }

    /// <summary>
    /// Զ����Դ��ַ��ѯ������
    /// </summary>
    private class RemoteServices : IRemoteServices
    {
        private readonly string defaultHostServer;
        private readonly string fallBackHostServer;

        public RemoteServices(string defaultHostServer, string fallBackHostServer)
        {
            this.defaultHostServer = defaultHostServer;
            this.fallBackHostServer = fallBackHostServer;
        }

        public string GetRemoteFallbackURL(string fileName)
        {
            return $"{defaultHostServer}/{fileName}";
        }

        public string GetRemoteMainURL(string fileName)
        {
            return $"{fallBackHostServer}/{fileName}";
        }
    }

    /// <summary>
    /// ��Դ�ļ����ܷ�����
    /// </summary>
    private class GameDecryptionServices : IDecryptionServices
    {
        public uint GetManagedReadBufferSize()
        {
            return 1024;
        }

        public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
        {
            return 32;
        }

        public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
        {
            throw new System.NotImplementedException();
        }

        public Stream LoadFromStream(DecryptFileInfo fileInfo)
        {
            BundleStream bundleStream = new BundleStream(fileInfo.FilePath,FileMode.Open,FileAccess.Read,FileShare.Read);
            return new MemoryStream();
        }
    }
}
