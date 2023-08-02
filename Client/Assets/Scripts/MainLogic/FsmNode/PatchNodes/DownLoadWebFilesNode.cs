using Cysharp.Threading.Tasks.Triggers;
using DJFrameWork.Event;
using DJFrameWork.Log;
using DJFrameWork.Manager;
using DJFrameWork.Singleton;
using DJFrameWork.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
/// <summary>
/// ������Դ�ڵ�
/// </summary>
public class DownLoadWebFilesNode : IStateNode
{
    private DJStateMachine machine;
    public void OnCreate(DJStateMachine machine)
    {
        this.machine = machine;
    }

    public void OnEnter()
    {
        DJSingleton.StartCoroutine(DownLoadWebFiles());
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    /// <summary>
    /// �����ļ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownLoadWebFiles()
    {
        yield return new WaitForSeconds(0.5f);
        
        //�����������
        int downloadingMaxNum = 10;
        //ʧ�ܺ����Ե�������
        int failedTryAgain = 3;
        string packageName = "DefaultPackage";
        var package = YooAssets.TryGetPackage(packageName);
        var downloader = YooAssets.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

        //û����Ҫ���ص���Դ
        if(downloader.TotalDownloadCount == 0)
        {
            DJLog.Log("���������Դ");
            //�л���������ɽڵ�
            machine.ChangeStateNode<LoadHotUpdateDllNode>();
            yield break;
        }

        //���ص��ļ�������
        int totalDownloadCount = downloader.TotalDownloadCount;
        //���ص��ļ��ܴ�С
        long totalDownloadBytesSize = downloader.TotalDownloadBytes;

        //ע�����ػص�����
        downloader.OnDownloadErrorCallback = OnDownloadError;
        downloader.OnDownloadOverCallback = OnDownloadOver;
        downloader.OnDownloadProgressCallback = OnDownloadProgress;
        downloader.OnStartDownloadFileCallback = OnDownloadStart;

        downloader.BeginDownload();
        yield return downloader;

        if(downloader.Status == EOperationStatus.Succeed)
        {
            DJLog.Log("������Դ�ɹ�");
            //�л���������ɽڵ�
            machine.ChangeStateNode<LoadHotUpdateDllNode>();
        }
        else
        {
            DJLog.Error($"������Դʧ�ܣ�{downloader.Error}");
        }
    }

    /// <summary>
    /// ��ʼ���ػص�
    /// </summary>
    /// <param name="fileName">�ļ���</param>
    /// <param name="sizeBytes"></param>
    /// <exception cref="NotImplementedException">��С</exception>
    private void OnDownloadStart(string fileName, long sizeBytes)
    {
        PatchManager.Instance.loadingWindow.SetTips("��ʼ������Դ.....");
        PatchManager.Instance.loadingWindow.ShowSlider();
        PatchManager.Instance.loadingWindow.ShowPercent();
    }

    /// <summary>
    /// ���ع����лص�
    /// </summary>
    /// <param name="totalDownloadCount">�������ļ�����</param>
    /// <param name="currentDownloadCount">��ǰ��������</param>
    /// <param name="totalDownloadBytes">�����ش�С</param>
    /// <param name="currentDownloadBytes">��ǰ���ش�С</param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnDownloadProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
    {
        float percent = (float)(currentDownloadBytes / totalDownloadBytes);
        PatchManager.Instance.loadingWindow.SetTips($"����������Դ.....[{currentDownloadBytes}/{totalDownloadBytes}]");
        PatchManager.Instance.loadingWindow.UpdateProgress(percent);
    }

    /// <summary>
    /// ���س�ʱ�ص�
    /// </summary>
    /// <param name="isSucceed"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnDownloadOver(bool isSucceed)
    {
        
    }

    /// <summary>
    /// ���ش���ص�
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="error"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnDownloadError(string fileName, string error)
    {
        
    }
}
