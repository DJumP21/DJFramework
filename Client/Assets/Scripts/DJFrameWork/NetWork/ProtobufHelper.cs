using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace DJFrameWork.NetWork
{
    /// <summary>
    /// ProtoBuf协议帮助类
    /// </summary>
    public static class ProtobufHelper
    {
        /// <summary>
        /// 将对象序列化为字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象实例</param>
        /// <returns>字符串（Base64编码格式）</returns>
        public static string SerializeToString_PB<T>(this T obj)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, obj);
                return Convert.ToBase64String(ms.GetBuffer(),0,(int)ms.Length);
            }
        }

        /// <summary>
        /// 将字符串反序列化为对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="txt">字符串</param>
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
        /// 将对象序列化为字节数组
        /// </summary>
        /// <typeparam name="T">对象实例</typeparam>
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
        /// 将字节数组反序列化为对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="arr">字节数组</param>
        /// <returns></returns>
        public static T DeserizalizeFromByteAry_PB<T>(this byte[] arr,int offset,int count)
        {
            using(MemoryStream ms = new MemoryStream(arr,offset,count))
            {
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }


        /// <summary>
        /// 协议名称编码
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] EncodeName(ProtoBuf.IExtensible msg)
        {
            //名字bytes和长度
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msg.ToString());
            Int16 len = (Int16)nameBytes.Length;
            //申请bytes数值
            byte[] bytes = new byte[2 + len];
            bytes[0] = (byte)(len % 256);
            bytes[1] = (byte)(len / 256);
            //组装名字Bytes
            Array.Copy(nameBytes, 0, bytes, 2, len);
            return bytes;
        }

        /// <summary>
        /// 解析协议名称
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offest"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string DecodeName(byte[] bytes,int offset,out int count)
        {
            count = 0;
            //必须大于两个字节
            if(offset+2>bytes.Length)
            {
                return "";
            }
            Int16 len = (Int16)((bytes[offset+1])<<8 | bytes[offset]);
            if(len<0)
            {
                return "";
            }
            //长度必须够
            if(offset+2+len>bytes.Length)
            {
                return "";
            }
            //解析
            count = 2 + len;
            string name = System.Text.Encoding.UTF8.GetString(bytes,offset+2,len);
            return name;
        }

        /// <summary>
        /// 将对象实例序列化为二进制文件
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象实例</param>
        /// <param name="path">文件路径</param>
        public static void SerizalizeToFile<T>(this T obj,string path)
        {
            using(var file = File.Create(path))
            {
                ProtoBuf.Serializer.Serialize(file, obj);
            }
        }

        /// <summary>
        /// 将二进制文件反序列化为对象实例——ProtoBuf
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
