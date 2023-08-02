using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization.Advanced;
using UnityEngine;

namespace DJFrameWork.NetWork
{
    /// <summary>
    /// ���ݻ���
    /// </summary>
    public class ByteArray
    {
        //Ĭ�ϴ�С
        const int DEFAULT_SIZE = 1024;
        //��ʼ��С
        int initSize = 0;
        //������
        public byte[] bytes;
        //��дλ��
        public int readIndex = 0;
        public int writeIndex = 0;
        //����
        private int capacity = 0;
        //ʣ��ռ�
        public int Remain
        {
            get
            {
                return capacity - writeIndex;
            }
        }

        /// <summary>
        /// ���ݳ���
        /// </summary>
        public int Length
        {
            get
            {
                return writeIndex - readIndex;
            }
        }

        //���캯��
        public ByteArray(int size = DEFAULT_SIZE)
        {
            bytes = new byte[size];
            capacity = size;
            initSize = size;
            readIndex = 0;
            writeIndex = 0;
        }

        public ByteArray(byte[] defaultBytes)
        {
            bytes = defaultBytes;
            capacity = defaultBytes.Length;
            initSize = defaultBytes.Length;
            readIndex = 0;
            writeIndex = defaultBytes.Length;
        }

        /// <summary>
        /// �������ô�С���������ݹ�������Ҫ����
        /// </summary>
        /// <param name="size"></param>
        public void Resize(int size)
        {
            if (size < Length)
            {
                return;
            }
            if (size < initSize)
            {
                return;
            }
            int n = 1;
            while (n < size)
            {
                n *= 2;
            }
            capacity = n;
            byte[] newBytes = new byte[capacity];
            Array.Copy(bytes, readIndex, newBytes, 0, writeIndex - readIndex);
            bytes = newBytes;
            writeIndex = Length;
            readIndex = 0;
        }

        /// <summary>
        /// ��鲢�ƶ�����
        /// </summary>
        public void CheckAndMoveBytes()
        {
            if(Length<8)
            {
                MoveBytes();
            }
        }
        /// <summary>
        /// �ƶ�����
        /// </summary>
        public void MoveBytes()
        {
            if(Length>0)
            {
                Array.Copy(bytes, readIndex, bytes, 0, Length);
            }
            writeIndex = Length;
            readIndex = 0;
        }

        /// <summary>
        /// д������
        /// </summary>
        /// <param name="bs">�ֽ�</param>
        /// <param name="offset">ƫ��</param>
        /// <param name="count">���ݳ���</param>
        /// <returns></returns>
        public int Write(byte[] bs,int offset,int count)
        {
            //ʣ����������  ������
            if(Remain<count)
            {
                Resize(Length+count);
            }
            Array.Copy(bs,offset,bytes,writeIndex,count);
            writeIndex += count;
            return count;
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(byte[] bs,int offset,int count)
        {
            count = Math.Min(count, Length);
            Array.Copy(bytes, readIndex, bs, offset, count);
            readIndex += count;
            CheckAndMoveBytes();
            return count;
        }

        /// <summary>
        /// ��ȡInt16
        /// </summary>
        /// <returns></returns>
        public Int16 ReadInt16()
        {
            if(Length<2)
            {
                return 0;
            }
            Int16 ret = (Int16)(bytes[readIndex+1]<<8 | bytes[readIndex]);
            readIndex += 2;
            CheckAndMoveBytes() ;
            return ret;
        }

        /// <summary>
        /// ��ȡInt32
        /// </summary>
        /// <returns></returns>
        public Int32 ReadInt32()
        {
            if(Length<4)
            {
                return 0;
            }
            Int32 ret = (Int32)((bytes[readIndex + 3] << 24) | (bytes[readIndex + 2] << 16) | (bytes[readIndex + 1] << 8) | (bytes[readIndex]));
            readIndex += 4;
            CheckAndMoveBytes();
            return ret;
        }


    }

}
