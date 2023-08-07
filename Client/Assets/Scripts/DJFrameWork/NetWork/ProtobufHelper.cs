using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace DJFrameWork.NetWork
{
    /// <summary>
    /// ProtoBufЭ�������
    /// </summary>
    public static class ProtobufHelper
    {
        /// <summary>
        /// ���������л�Ϊ�ַ���
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="obj">����ʵ��</param>
        /// <returns>�ַ�����Base64�����ʽ��</returns>
        public static string SerializeToString_PB<T>(this T obj)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, obj);
                return Convert.ToBase64String(ms.GetBuffer(),0,(int)ms.Length);
            }
        }

        /// <summary>
        /// ���ַ��������л�Ϊ����ʵ��
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="txt">�ַ���</param>
        /// <returns></returns>
        public static T DeserializeFromString_PB<T>(this string txt)
        {
            byte[] arr = Convert.FromBase64String(txt);
            using(MemoryStream ms = new MemoryStream(arr))
            {
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }

        /// <summary>
        /// ���������л�Ϊ�ֽ�����
        /// </summary>
        /// <typeparam name="T">����ʵ��</typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeToByteAry_PB<T>(this T obj)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms,obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// ���ֽ����鷴���л�Ϊ����ʵ��
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="arr">�ֽ�����</param>
        /// <returns></returns>
        public static T DeserizalizeFromByteAry_PB<T>(this byte[] arr,int offset,int count)
        {
            using(MemoryStream ms = new MemoryStream(arr,offset,count))
            {
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }


        /// <summary>
        /// Э�����Ʊ���
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] EncodeName(ProtoBuf.IExtensible msg)
        {
            //����bytes�ͳ���
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msg.ToString());
            Int16 len = (Int16)nameBytes.Length;
            //����bytes��ֵ
            byte[] bytes = new byte[2 + len];
            bytes[0] = (byte)(len % 256);
            bytes[1] = (byte)(len / 256);
            //��װ����Bytes
            Array.Copy(nameBytes, 0, bytes, 2, len);
            return bytes;
        }

        /// <summary>
        /// ����Э������
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offest"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string DecodeName(byte[] bytes,int offset,out int count)
        {
            count = 0;
            //������������ֽ�
            if(offset+2>bytes.Length)
            {
                return "";
            }
            Int16 len = (Int16)((bytes[offset+1])<<8 | bytes[offset]);
            if(len<0)
            {
                return "";
            }
            //���ȱ��빻
            if(offset+2+len>bytes.Length)
            {
                return "";
            }
            //����
            count = 2 + len;
            string name = System.Text.Encoding.UTF8.GetString(bytes,offset+2,len);
            return name;
        }

        /// <summary>
        /// ������ʵ�����л�Ϊ�������ļ�
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="obj">����ʵ��</param>
        /// <param name="path">�ļ�·��</param>
        public static void SerizalizeToFile<T>(this T obj,string path)
        {
            using(var file = File.Create(path))
            {
                ProtoBuf.Serializer.Serialize(file, obj);
            }
        }

        /// <summary>
        /// ���������ļ������л�Ϊ����ʵ������ProtoBuf
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T DeserializeFromFile_PB<T>(this string path)
        {
            using (var file = File.OpenRead(path))
            {
                return ProtoBuf.Serializer.Deserialize<T>(file);
            }
        }
    }

}
