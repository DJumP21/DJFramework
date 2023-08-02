using DJFrameWork.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 更新事件
/// </summary>
public class PatchEvent
{
    /// <summary>
    /// 资源包初始化失败
    /// </summary>
    public class InitializeFailed : IEventMessage
    { 
        public static void SendEventMessage()
        {
            var message = new InitializeFailed();
            DJEvent.SendMessage(message);
        }
    }

    /// <summary>
    /// 更新状态改变
    /// </summary>
    public class PatchStateChange : IEventMessage
    {
        /// <summary>
        /// 提示
        /// </summary>
        public string tips;
        public static void SendEventMessage(string tips)
        {
            var message = new PatchStateChange();
            message.tips = tips;
            DJEvent.SendMessage(message);
        }
    }

    /// <summary>
    /// 发现需要更新的文件
    /// </summary>
    public class FindUpdateFiles:IEventMessage
    {
        /// <summary>
        /// 目标文件数量
        /// </summary>
        public int totalCount;
        /// <summary>
        /// 目标文件大小
        /// </summary>
        public long totalSizeBytes;

        public static void SendEventMessage(int count,long sizeBytes)
        {
            var message = new FindUpdateFiles();
            message.totalCount = count;
            message.totalSizeBytes = sizeBytes;
            DJEvent.SendMessage(message);
        }
    }

    /// <summary>
    /// 下载进度更新
    /// </summary>
    public class DownloadProgressUpdate:IEventMessage
    {
        //需要下载的总数量
        public int totalDownloadCount;
        //当前下载的数量
        public int currenDownloadCount;
        //需要下载的字节总大小
        public long totalDownloadSizeBytes;
        //当前下载的字节大小
        public long currenDownloadSizeBytes;
        public static void SendEventMessage(int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, int currentDownloadSizeBytes)
        {
            var message = new DownloadProgressUpdate();
            message.totalDownloadCount = totalDownloadCount;
            message.currenDownloadCount = currentDownloadCount;
            message.totalDownloadSizeBytes = totalDownloadSizeBytes;
            message.currenDownloadSizeBytes = currentDownloadSizeBytes;
            DJEvent.SendMessage(message);
        }
    }

    /// <summary>
    /// 资源版本号更新失败
    /// </summary>
    public class PackageVerisonUpdateFailed:IEventMessage
    {
        public static void SendEventMessage()
        {
            var message = new PackageVerisonUpdateFailed();
            DJEvent.SendMessage(message);
        }
    }

    /// <summary>
    /// 补丁清单更新失败
    /// </summary>
    public class PatchManifestUpdateFailed:IEventMessage
    {
        public static void SendEventMessage()
        {
            var message = new PatchManifestUpdateFailed();
            DJEvent.SendMessage(message);
        }
    }

    /// <summary>
    /// 网络文件下载失败
    /// </summary>
    public class WebFileDownloadFailed:IEventMessage
    {
        public string fileName;
        public string error;
        public static void SendEventMessage(string fileName,string error)
        {
            var message = new WebFileDownloadFailed();
            message.fileName = fileName;    
            message.error = error;
            DJEvent.SendMessage(message);
        }
    }
}
