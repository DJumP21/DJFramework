using DJFrameWork.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �����¼�
/// </summary>
public class PatchEvent
{
    /// <summary>
    /// ��Դ����ʼ��ʧ��
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
    /// ����״̬�ı�
    /// </summary>
    public class PatchStateChange : IEventMessage
    {
        /// <summary>
        /// ��ʾ
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
    /// ������Ҫ���µ��ļ�
    /// </summary>
    public class FindUpdateFiles:IEventMessage
    {
        /// <summary>
        /// Ŀ���ļ�����
        /// </summary>
        public int totalCount;
        /// <summary>
        /// Ŀ���ļ���С
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
    /// ���ؽ��ȸ���
    /// </summary>
    public class DownloadProgressUpdate:IEventMessage
    {
        //��Ҫ���ص�������
        public int totalDownloadCount;
        //��ǰ���ص�����
        public int currenDownloadCount;
        //��Ҫ���ص��ֽ��ܴ�С
        public long totalDownloadSizeBytes;
        //��ǰ���ص��ֽڴ�С
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
    /// ��Դ�汾�Ÿ���ʧ��
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
    /// �����嵥����ʧ��
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
    /// �����ļ�����ʧ��
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
