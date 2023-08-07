using DJFrameWork.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using ProtoMessage;
using Assets.Scripts.DJFrameWork.NetWork;

namespace DJFrameWork.NetWork
{
    /// <summary>
    /// ���������
    /// </summary>
    public static class NetManager
    {
        //Ƕ����
        private static Socket clientSocket;
        //���ջ�����
        private static ByteArray readBuffer;
        //д�����
        private static Queue<ByteArray> writeQueue;

        //��Ϣ����
        public delegate void MsgListener(ProtoBuf.IExtensible msg);
        //�����б�
        private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();
        //��Ϣ����
        private static List<ProtoBuf.IExtensible> msgList;
        //��Ϣ���еĳ���
        private static int msgCount = 0;
        //ÿ��Update�������Ϣ��
        private readonly static int MAX_MESSAGE_FIRE = 10;

        //�¼�����
        public delegate void EventListener(string err);
        //�¼��б�
        private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

        //�Ƿ���������
        private static bool isConnecting = false;
        //�Ƿ����ڹر�
        private static bool isclosing = false;

        //�Ƿ�������������
        public static bool isUsePing = true;
        //�������
        public static int pingInterval = 30;
        //�ϴη���Ping��ʱ��
        private static float lastPingTime = 0;
        //�ϴν���Pong��ʱ��
        private static float lastPongTime = 0;

        //�Ƿ����ӳɹ�
        public static bool connectSuccess = false;

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="ip">Զ��IP</param>
        /// <param name="port">�˿ں�</param>
        public static void Connect(string ip, int port)
        {
            if(clientSocket != null && clientSocket.Connected)
            {
                DJLog.Log("����ʧ�ܣ������Ѿ�����");
                return;
            }
            if(isConnecting)
            {
                DJLog.Log("��������....");
                return;
            }
            InitState();
            //��ʹ��nagle�㷨
            clientSocket.NoDelay = true;
            isConnecting = true;
            //��������
            clientSocket.BeginConnect(ip, port, ConnectCallBack, clientSocket);
        }

        /// <summary>
        /// ��ʼ��״̬
        /// </summary>
        private static void InitState()
        {
            //Socket ʹ��tcp����
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //���ջ�����
            readBuffer = new ByteArray();
            //�������
            writeQueue=new Queue<ByteArray>();
            //��Ϣ����
            msgList = new List<ProtoBuf.IExtensible>();
            msgCount = 0;
            isConnecting = false;
            isclosing = false;

            lastPingTime = Time.time;
            lastPongTime = Time.time;
            //����PongЭ��
            if(!msgListeners.ContainsKey("MsgPong"))
            {
                AddMsgListener("MsgPong", OnMsgPong);
            }
        }

        /// <summary>
        /// ���ӻص�
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                DJLog.Log("�������ӳɹ�");
                connectSuccess = true;
                FireEvent(NetEvent.ConnectSuccess, "");
                isConnecting=false;
                clientSocket.BeginReceive(readBuffer.bytes, 0, readBuffer.bytes.Length, 0, ReceiveCallBack, clientSocket);
            }
            catch (Exception ex)
            {
                DJLog.Error($"��������ʧ�ܣ�Err:{ex.ToString()}");
                FireEvent(NetEvent.ConnectFailed, ex.ToString());
                isConnecting = false;
            }
        }

        /// <summary>
        /// ������Ϣ�ص�
        /// </summary>
        /// <param name="ar"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int count = socket.EndReceive(ar);
                if(count == 0)
                {
                    Close();
                    return;
                }
                readBuffer.writeIndex += count;
                //�����ֽ�����
                OnReceiveData();
                //ʣ������Ϊ0 ������
                if(readBuffer.Remain<0)
                {
                    readBuffer.MoveBytes();
                    readBuffer.Resize(readBuffer.Length*2);
                }
                //����������Ϣ
                socket.BeginReceive(readBuffer.bytes, readBuffer.writeIndex, readBuffer.Remain, 0, ReceiveCallBack, socket);
            }
            catch (Exception ex)
            {
                DJLog.Error($"������Ϣʧ�ܣ�{ex.ToString()}");
            }
        }

        /// <summary>
        /// �ֽ����ݴ���
        /// </summary>
        private static void OnReceiveData()
        {
            //��Ϣ���� ��Ϣ������
            if(readBuffer.Length<=2)
            {
                return;
            }
            //��ȡ��Ϣ�峤��
            int readIndex = readBuffer.readIndex;
            byte[] bytes = readBuffer.bytes;
            Int16 bodyLength = (Int16)((bytes[readIndex+1] << 8) | bytes[readIndex]);
            if(readBuffer.Length < bodyLength+2)
            {
                return;
            }
            readBuffer.readIndex += 2;
            //����Э����
            int nameCount = 0;
            string protoName = ProtobufHelper.DecodeName(readBuffer.bytes, readBuffer.readIndex,out nameCount);
            //Э����Ϊ��
            if(string.IsNullOrEmpty(protoName))
            {
                DJLog.Error("����Э��������");
                return;
            }
            readBuffer.readIndex += nameCount;
            //����Э����
            int bodyCount = bodyLength - nameCount;
            MsgBase msgBase = ProtobufHelper.DeserizalizeFromByteAry_PB<MsgBase>(readBuffer.bytes, readBuffer.readIndex, bodyCount);
            readBuffer.readIndex += bodyCount;
            //��鲢�ƶ��ֽ�
            readBuffer.CheckAndMoveBytes();
            MessagePatch.PatchMessage(msgBase);
            msgCount++;
            //������ȡ��Ϣ
            if(readBuffer.Length>0)
            {
                OnReceiveData();
            }
        }

        /// <summary>
        /// �����Ϣ����Ϣ����
        /// </summary>
        /// <param name="msg"></param>
        public static void AddMsgToMsgList(ProtoBuf.IExtensible msg)
        {
            //��ӵ���Ϣ����
            lock (msgList)
            {
                msgList.Add(msg);
            }
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="sendStr">��Ϣ����</param>
        public static void Send(ProtoBuf.IExtensible msg)
        {
            if (clientSocket == null || !clientSocket.Connected)
            {
                return;
            }
            if (isConnecting)
            {
                return;
            }
            if (isclosing)
            {
                return;
            }
            
            byte[] nameBytes = ProtobufHelper.EncodeName(msg);
            byte[] bodyBytes = ProtobufHelper.SerializeToByteAry_PB(msg);
            int length = nameBytes.Length + bodyBytes.Length;
            //������Ϣ�ĳ��� = Э�����ֽڳ���+Э�����ֽڳ���+2��Э�鳤�ȵ��ֽڣ� 
            byte[] sendBytes = new byte[length + 2];
            sendBytes[0] = (byte)(length % 256);
            sendBytes[1] = (byte)(length / 256);
            //��װ��Ϣ
            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
            //д�����
            ByteArray ba = new ByteArray(sendBytes);
            int count = 0;  //д����еĳ���
            lock(writeQueue)   //��ֹ����
            {
                writeQueue.Enqueue(ba);
                count = writeQueue.Count;
            }
            //����
            if(count == 1)
            {
                clientSocket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallBack, clientSocket);
            }
           
        }

        /// <summary>
        /// ���ͻص�
        /// </summary>
        /// <param name="ar"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void SendCallBack(IAsyncResult ar)
        {
            //��ȡstate/EndSend����
            Socket socket = (Socket)ar.AsyncState;
            if(socket == null || !socket.Connected)
            {
                return;
            }
            //EndSend
            int count= socket.EndSend(ar);
            //��ȡд����е�һ������
            ByteArray ba = null;
            lock(writeQueue)
            {
                if (writeQueue.Count > 0)
                {
                    ba = writeQueue.First();
                }
            }
            //��������
            ba.readIndex += count;
            if(ba.Length==0)
            {
                lock(writeQueue)
                {
                    writeQueue.Dequeue();
                    if(writeQueue.Count > 0)
                    {
                        ba = writeQueue.First();
                    }
                }
            }
            //�������� ֱ�����Ͷ���Ϊ��
            if(ba!=null)
            {
                socket.BeginSend(ba.bytes, 0, ba.Length, 0, SendCallBack, socket);
            }else if(isclosing)
            {
                socket.Close();
            }
        }

        //Update
        public static void Update()
        {
            //��Ϣ����
            MsgUpdate();
            //�������
            PingUpdate();
        }

        /// <summary>
        /// ÿִ֡��Msg
        /// </summary>
        public static void MsgUpdate()
        {
            if(msgCount==0)
            {
                return;
            }
            //�ظ�������Ϣ
            for(int i=0;i<MAX_MESSAGE_FIRE;i++)
            {
                //��ȡ��һ����Ϣ
                ProtoBuf.IExtensible msg = null;
                lock(msgList)
                {
                    if(msgCount>0)
                    {
                        msg = msgList[0];
                        msgList.RemoveAt(0);
                        msgCount--;
                    }
                }
                //�ַ���Ϣ
                if (msg != null)
                {
                    string protoName = msg.ToString().Split('.')[1];
                    FireMsg(protoName, msg);
                }
                else
                {
                    break;
                }
            }
            
        }

        /// <summary>
        /// �ر�����
        /// </summary>
        public static void Close()
        {
            //socketΪ�� �����Ѿ��Ͽ�����
            if(clientSocket == null || !clientSocket.Connected)
            {
                return;
            }
            //��������ʱ�޷��ر�
            if(isConnecting)
            {
                return;
            }
            //���������ڷ���,�ȴ��������ݵĻص�ȥ����
            if(writeQueue.Count>0)
            {
                isclosing = true;
            }
            else
            {
                clientSocket.Close();
                FireEvent(NetEvent.Close,"");
            }
        }

        /// <summary>
        /// �����Ϣ����
        /// </summary>
        /// <param name="msgName">��Ϣ����</param>
        /// <param name="listener">�����¼�</param>
        public static void AddMsgListener(string msgName, MsgListener listener)
        {
            if(msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] += listener;
            }
            else
            {
                msgListeners[msgName] = listener;
            }
        }

        /// <summary>
        /// �Ƴ���Ϣ����
        /// </summary>
        /// <param name="msgName">��Ϣ����</param>
        /// <param name="listener">��Ϣ�����¼�</param>
        public static void Remove(string msgName,MsgListener listener)
        {
            if(msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName] -= listener;
                if (msgListeners[msgName] == null)
                {
                    msgListeners.Remove(msgName);
                }
            }
        }

        /// <summary>
        /// �ַ���Ϣ�¼�
        /// </summary>
        /// <param name="msgName">��Ϣ����</param>
        /// <param name="msgBase">��Ϣ</param>
        public static void FireMsg(string msgName,ProtoBuf.IExtensible msgBase)
        {
            if(msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName](msgBase);
            }
        }

        /// <summary>
        /// ��������¼�����
        /// </summary>
        /// <param name="netEvent">�����¼�</param>
        /// <param name="listener">����</param>
        public static void AddEventListener(NetEvent netEvent,EventListener listener)
        {
            if(eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] += listener;
            }
            else
            {
                eventListeners[netEvent] = listener;
            }
        }

        /// <summary>
        /// �Ƴ������¼�����
        /// </summary>
        /// <param name="netEvent"></param>
        /// <param name="listener"></param>
        public static void RemoveEventListener(NetEvent netEvent,EventListener listener)
        {
            if(eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] -= listener;
                if (eventListeners[netEvent] == null)
                {
                    eventListeners.Remove(netEvent);
                }
            }
        }

        /// <summary>
        /// �ַ������¼�
        /// </summary>
        public static void FireEvent(NetEvent netEvent,string err)
        {
            if(eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent](err);
            }
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <returns></returns>
        public static string GetDesc()
        {
            if (clientSocket == null)
            {
                return string.Empty;
            }
            if (!clientSocket.Connected)
            {
                return string.Empty;
            }
            return clientSocket.LocalEndPoint.ToString();
        }

        /// <summary>
        /// ����Ping��Ϣ
        /// </summary>
        private static void PingUpdate()
        {
            //�Ƿ�������������
            if (!isUsePing)
            {
                return;
            }
            //����ping
            if(Time.time - lastPingTime>pingInterval)
            {
                MsgPing msgPing = new MsgPing();
                msgPing.Id = 1;
                msgPing.msgName = "MsgPing"; 
                MsgBase msg = new MsgBase();
                msg.msgPing = msgPing;
                Send(msg);
                lastPingTime = Time.time;
            }
            //���Pongʱ�� ���������ӷ�����û�з��ؽ��,��ر�����
            if(Time.time - lastPongTime > pingInterval*4)
            {
                Close();
            }
        }


        private static void OnMsgPong(ProtoBuf.IExtensible msgBase)
        {
            lastPongTime = Time.time;
            DJLog.Log("Receive MgsPong");
        }
    }

}
