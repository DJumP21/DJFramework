using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.NetWork
{
    /// <summary>
    /// ��Ϣ����
    /// </summary>
    public class MsgBase
    {
        //Э����
        public string protoName = "";

        //Э����ı������

        /// <summary>
        /// ��Ϣ��ת��Ϊ�ֽ�����
        /// </summary>
        /// <param name="msgBase"></param>
        /// <returns></returns>
        public static byte[] Encode(MsgBase msgBase)
        {
            //����ʹ��Json ����ProtoBuf
            return null;
        }

        /// <summary>
        /// �ֽ�����ת��Ϊ��Ϣ��
        /// </summary>
        /// <param name="protoName">Э����</param>
        /// <param name="bytes">�ֽ�����</param>
        /// <param name="offset">ƫ��</param>
        /// <param name="count">����</param>
        /// <returns></returns>
        public static MsgBase Decode(string protoName, byte[] bytes,int offset,int count)
        {
            //����ʹ��Json ����ProtoBuf
            return null;
        }

        //Э�����ı������

        /// <summary>
        /// Э���Э����ת��Ϊ�ֽ�����
        /// </summary>
        /// <param name="msgBase"></param>
        /// <returns></returns>
        public static byte[] EncodeName(MsgBase msgBase)
        {
            return null;
        }

        /// <summary>
        /// �ֽ�����ת��ΪЭ����
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

