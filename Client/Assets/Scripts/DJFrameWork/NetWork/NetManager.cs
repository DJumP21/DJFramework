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
    /// 网络管理器
    /// </summary>
    public static class NetManager
    {
        //嵌套字
        private static Socket clientSocket;
        //接收缓冲区
        private static ByteArray readBuffer;
        //写入队列
        private static Queue<ByteArray> writeQueue;

        //消息监听
        public delegate void MsgListener(ProtoBuf.IExtensible msg);
        //监听列表
        private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();
        //消息队列
        private static List<ProtoBuf.IExtensible> msgList;
        //消息队列的长度
        private static int msgCount = 0;
        //每次Update处理的消息量
        private readonly static int MAX_MESSAGE_FIRE = 10;

        //事件监听
        public delegate void EventListener(string err);
        //事件列表
        private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

        //是否正在连接
        private static bool isConnecting = false;
        //是否正在关闭
        private static bool isclosing = false;

        //是否启动心跳机制
        public static bool isUsePing = true;
        //心跳间隔
        public static int pingInterval = 30;
        //上次发送Ping的时间
        private static float lastPingTime = 0;
        //上次接收Pong的时间
        private static float lastPongTime = 0;

        //是否连接成功
        public static bool connectSuccess = false;

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip">远程IP</param>
        /// <param name="port">端口号</param>
        public static void Connect(string ip, int port)
        {
            if(clientSocket != null && clientSocket.Connected)
            {
                DJLog.Log("连接失败，网络已经连接");
                return;
            }
            if(isConnecting)
            {
                DJLog.Log("正在连接....");
                return;
            }
            InitState();
            //不使用nagle算法
            clientSocket.NoDelay = true;
            isConnecting = true;
            //开启连接
            clientSocket.BeginConnect(ip, port, ConnectCallBack, clientSocket);
        }

        /// <summary>
        /// 初始化状态
        /// </summary>
        private static void InitState()
        {
            //Socket 使用tcp连接
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //接收缓冲区
            readBuffer = new ByteArray();
            //缓冲队列
            writeQueue=new Queue<ByteArray>();
            //消息队列
            msgList = new List<ProtoBuf.IExtensible>();
            msgCount = 0;
            isConnecting = false;
            isclosing = false;

            lastPingTime = Time.time;
            lastPongTime = Time.time;
            //监听Pong协议
            if(!msgListeners.ContainsKey("MsgPong"))
            {
                AddMsgListener("MsgPong", OnMsgPong);
            }
        }

        /// <summary>
        /// 连接回调
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);
                DJLog.Log("网络连接成功");
                connectSuccess = true;
                FireEvent(NetEvent.ConnectSuccess, "");
                isConnecting=false;
                clientSocket.BeginReceive(readBuffer.bytes, 0, readBuffer.bytes.Length, 0, ReceiveCallBack, clientSocket);
            }
            catch (Exception ex)
            {
                DJLog.Error($"网络连接失败，Err:{ex.ToString()}");
                FireEvent(NetEvent.ConnectFailed, ex.ToString());
                isConnecting = false;
            }
        }

        /// <summary>
        /// 接收信息回调
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
                //处理字节数据
                OnReceiveData();
                //剩余容量为0 则扩容
                if(readBuffer.Remain<0)
                {
                    readBuffer.MoveBytes();
                    readBuffer.Resize(readBuffer.Length*2);
                }
                //继续接收消息
                socket.BeginReceive(readBuffer.bytes, readBuffer.writeIndex, readBuffer.Remain, 0, ReceiveCallBack, socket);
            }
            catch (Exception ex)
            {
                DJLog.Error($"接收消息失败：{ex.ToString()}");
            }
        }

        /// <summary>
        /// 字节数据处理
        /// </summary>
        private static void OnReceiveData()
        {
            //消息长度 消息不完整
            if(readBuffer.Length<=2)
            {
                return;
            }
            //获取消息体长度
            int readIndex = readBuffer.readIndex;
            byte[] bytes = readBuffer.bytes;
            Int16 bodyLength = (Int16)((bytes[readIndex+1] << 8) | bytes[readIndex]);
            if(readBuffer.Length < bodyLength+2)
            {
                return;
            }
            readBuffer.readIndex += 2;
            //解析协议名
            int nameCount = 0;
            string protoName = ProtobufHelper.DecodeName(readBuffer.bytes, readBuffer.readIndex,out nameCount);
            //协议名为空
            if(string.IsNullOrEmpty(protoName))
            {
                DJLog.Error("解析协议名错误");
                return;
            }
            readBuffer.readIndex += nameCount;
            //解析协议体
            int bodyCount = bodyLength - nameCount;
            MsgBase msgBase = ProtobufHelper.DeserizalizeFromByteAry_PB<MsgBase>(readBuffer.bytes, readBuffer.readIndex, bodyCount);
            readBuffer.readIndex += bodyCount;
            //检查并移动字节
            readBuffer.CheckAndMoveBytes();
            MessagePatch.PatchMessage(msgBase);
            msgCount++;
            //继续读取消息
            if(readBuffer.Length>0)
            {
                OnReceiveData();
            }
        }

        /// <summary>
        /// 添加消息到消息队列
        /// </summary>
        /// <param name="msg"></param>
        public static void AddMsgToMsgList(ProtoBuf.IExtensible msg)
        {
            //添加到消息队列
            lock (msgList)
            {
                msgList.Add(msg);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sendStr">消息内容</param>
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
            //发送消息的长度 = 协议名字节长度+协议体字节长度+2（协议长度的字节） 
            byte[] sendBytes = new byte[length + 2];
            sendBytes[0] = (byte)(length % 256);
            sendBytes[1] = (byte)(length / 256);
            //组装消息
            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
            //写入队列
            ByteArray ba = new ByteArray(sendBytes);
            int count = 0;  //写入队列的长度
            lock(writeQueue)   //防止阻塞
            {
                writeQueue.Enqueue(ba);
                count = writeQueue.Count;
            }
            //发送
            if(count == 1)
            {
                clientSocket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallBack, clientSocket);
            }
           
        }

        /// <summary>
        /// 发送回调
        /// </summary>
        /// <param name="ar"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void SendCallBack(IAsyncResult ar)
        {
            //获取state/EndSend处理
            Socket socket = (Socket)ar.AsyncState;
            if(socket == null || !socket.Connected)
            {
                return;
            }
            //EndSend
            int count= socket.EndSend(ar);
            //获取写入队列第一条数据
            ByteArray ba = null;
            lock(writeQueue)
            {
                if (writeQueue.Count > 0)
                {
                    ba = writeQueue.First();
                }
            }
            //完整发送
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
            //继续发送 直到发送队列为空
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
            //消息处理
            MsgUpdate();
            //心跳检测
            PingUpdate();
        }

        /// <summary>
        /// 每帧执行Msg
        /// </summary>
        public static void MsgUpdate()
        {
            if(msgCount==0)
            {
                return;
            }
            //重复处理消息
            for(int i=0;i<MAX_MESSAGE_FIRE;i++)
            {
                //获取第一条消息
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
                //分发消息
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
        /// 关闭连接
        /// </summary>
        public static void Close()
        {
            //socket为空 或者已经断开连接
            if(clientSocket == null || !clientSocket.Connected)
            {
                return;
            }
            //正在连接时无法关闭
            if(isConnecting)
            {
                return;
            }
            //还有数据在发送,等待发送数据的回调去处理
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
        /// 添加消息监听
        /// </summary>
        /// <param name="msgName">消息名称</param>
        /// <param name="listener">监听事件</param>
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
        /// 移除消息监听
        /// </summary>
        /// <param name="msgName">消息名称</param>
        /// <param name="listener">消息监听事件</param>
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
        /// 分发消息事件
        /// </summary>
        /// <param name="msgName">消息名称</param>
        /// <param name="msgBase">消息</param>
        public static void FireMsg(string msgName,ProtoBuf.IExtensible msgBase)
        {
            if(msgListeners.ContainsKey(msgName))
            {
                msgListeners[msgName](msgBase);
            }
        }

        /// <summary>
        /// 添加网络事件监听
        /// </summary>
        /// <param name="netEvent">网络事件</param>
        /// <param name="listener">监听</param>
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
        /// 移除网络事件监听
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
        /// 分发网络事件
        /// </summary>
        public static void FireEvent(NetEvent netEvent,string err)
        {
            if(eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent](err);
            }
        }

        /// <summary>
        /// 获取描述
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
        /// 发送Ping消息
        /// </summary>
        private static void PingUpdate()
        {
            //是否启动心跳机制
            if (!isUsePing)
            {
                return;
            }
            //发送ping
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
            //检测Pong时间 超过两分钟服务器没有返回结果,则关闭连接
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
