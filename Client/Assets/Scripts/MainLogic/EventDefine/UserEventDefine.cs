using DJFrameWork.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �û��¼�
/// </summary>
public class UserEventDefine
{
    /// <summary>
    /// �û��ٴγ��Գ�ʼ����Դ��
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
    /// �û���ʼ���������ļ�
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
    /// �û����Ը��¾�̬��Դ�汾
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
    /// �û��ٴγ��Ը��²����嵥
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
    /// �û��ٴγ������������ļ�
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
