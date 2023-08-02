using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.NetWork
{
    /// <summary>
    /// 消息基类
    /// </summary>
    public class MsgBase
    {
        //协议名
        public string protoName = "";

        //协议体的编码解码

        /// <summary>
        /// 消息类转化为字节数据
        /// </summary>
        /// <param name="msgBase"></param>
        /// <returns></returns>
        public static byte[] Encode(MsgBase msgBase)
        {
            //可以使用Json 或者ProtoBuf
            return null;
        }

        /// <summary>
        /// 字节数据转化为消息类
        /// </summary>
        /// <param name="protoName">协议名</param>
        /// <param name="bytes">字节数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">长度</param>
        /// <returns></returns>
        public static MsgBase Decode(string protoName, byte[] bytes,int offset,int count)
        {
            //可以使用Json 或者ProtoBuf
            return null;
        }

        //协议名的编码解码

        /// <summary>
        /// 协议的协议名转化为字节数据
        /// </summary>
        /// <param name="msgBase"></param>
        /// <returns></returns>
        public static byte[] EncodeName(MsgBase msgBase)
        {
            return null;
        }

        /// <summary>
        /// 字节数据转化为协议名
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string DecodeName(byte[] bytes,int offset,out int count)
        {
            count = 0;
            return null;
        }
    }
}

