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
/// 下载资源节点
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
    /// 下载文件
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownLoadWebFiles()
    {
        yield return new WaitForSeconds(0.5f);
        
        //最大下载数量
        int downloadingMaxNum = 10;
        //失败后重试的最大次数
        int failedTryAgain = 3;
        string packageName = "DefaultPackage";
        var package = YooAssets.TryGetPackage(packageName);
        var downloader = YooAssets.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

        //没有需要下载的资源
        if(downloader.TotalDownloadCount == 0)
        {
            DJLog.Log("无需更新资源");
            //切换到更新完成节点
            machine.ChangeStateNode<LoadHotUpdateDllNode>();
            yield break;
        }

        //下载的文件总数量
        int totalDownloadCount = downloader.TotalDownloadCount;
        //下载的文件总大小
        long totalDownloadBytesSize = downloader.TotalDownloadBytes;

        //注册下载回调方法
        downloader.OnDownloadErrorCallback = OnDownloadError;
        downloader.OnDownloadOverCallback = OnDownloadOver;
        downloader.OnDownloadProgressCallback = OnDownloadProgress;
        downloader.OnStartDownloadFileCallback = OnDownloadStart;

        downloader.BeginDownload();
        yield return downloader;

        if(downloader.Status == EOperationStatus.Succeed)
        {
            DJLog.Log("下载资源成功");
            //切换到更新完成节点
            machine.ChangeStateNode<LoadHotUpdateDllNode>();
        }
        else
        {
            DJLog.Error($"下载资源失败，{downloader.Error}");
        }
    }

    /// <summary>
    /// 开始下载回调
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="sizeBytes"></param>
    /// <exception cref="NotImplementedException">大小</exception>
    private void OnDownloadStart(string fileName, long sizeBytes)
    {
        PatchManager.Instance.loadingWindow.SetTips("开始下载资源.....");
        PatchManager.Instance.loadingWindow.ShowSlider();
        PatchManager.Instance.loadingWindow.ShowPercent();
    }

    /// <summary>
    /// 下载过程中回调
    /// </summary>
    /// <param name="totalDownloadCount">总下载文件数量</param>
    /// <param name="currentDownloadCount">当前下载数量</param>
    /// <param name="totalDownloadBytes">总下载大小</param>
    /// <param name="currentDownloadBytes">当前下载大小</param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnDownloadProgress(int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
    {
        float percent = (float)(currentDownloadBytes / totalDownloadBytes);
        PatchManager.Instance.loadingWindow.SetTips($"正在下载资源.....[{currentDownloadBytes}/{totalDownloadBytes}]");
        PatchManager.Instance.loadingWindow.UpdateProgress(percent);
    }

    /// <summary>
    /// 下载超时回调
    /// </summary>
    /// <param name="isSucceed"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnDownloadOver(bool isSucceed)
    {
        
    }

    /// <summary>
    /// 下载错误回调
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="error"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnDownloadError(string fileName, string error)
    {
        
    }
}
