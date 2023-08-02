using DJFrameWork.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 用户事件
/// </summary>
public class UserEventDefine
{
    /// <summary>
    /// 用户再次尝试初始化资源包
    /// </summary>
    public class UserTryInitializeAgain : IEventMessage
    {
        public static void SendEventMessage()
        {
            var message = new UserTryInitializeAgain();
            DJEvent.SendMessage(message);
        }
    }

    /// <summary>
    /// 用户开始下载网络文件
    /// </summary>
    public class UserDownLoadWebFilesBegin:IEventMessage
    {
        public static void SendEventMessage()
        { 
            var message = new UserDownLoadWebFilesBegin();
            DJEvent.SendMessage(message);
        }
    }

    /// <summary>
    /// 用户尝试更新静态资源版本
    /// </summary>
    public class UserTryUpdatePackageVersionAgain : IEventMessage
    { 
        public static void SendEventMessage() 
        {
            var message = new UserTryUpdatePackageVersionAgain(); 
            DJEvent.SendMessage(message); 
        } 
    }

    /// <summary>
    /// 用户再次尝试更新补丁清单
    /// </summary>
    public class UserTryUpdatePatchManifestAgain:IEventMessage
    {
        public static void SendEventMessage()
        {
            var message = new UserTryUpdatePatchManifestAgain();
            DJEvent.SendMessage(message);
        }

    }

    /// <summary>
    /// 用户再次尝试下载网络文件
    /// </summary>
    public class UserTryDownloadWebFilesAgain:IEventMessage
    {
        public static void SendEventMessage()
        {
            var message = new UserTryDownloadWebFilesAgain();
            DJEvent.SendMessage(message);
        }
    }
}
