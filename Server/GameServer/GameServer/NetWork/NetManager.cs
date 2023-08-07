using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using GameServer.MessageHandler;
using ProtoMessage;
using EventHandler = GameServer.MessageHandler.EventHandler;

namespace GameServer.NetWork
{
    /// <summary>
    /// 网路管理器
    /// </summary>
    public class NetManager
    {
        //监听socket
        public static Socket listendfd;
        //客户端socket缓存
        public static Dictionary<Socket,ClientState> clients = new Dictionary<Socket,ClientState>();
        //select的检查列表
        static List<Socket> checkRead = new List<Socket>();

        /// <summary>
        /// 循环检查
        /// </summary>
        /// <param name="listenPort"></param>
        public static void Start(int listenPort)
        {
            //Socket
            listendfd = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            //绑定
            IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
            IPEndPoint ipEp = new IPEndPoint(ipAddress, listenPort);
            listendfd.Bind(ipEp);
            //监听
            listendfd.Listen(0);
            Console.WriteLine("服务器启动成功....");
        }

        /// <summary>
        /// 监听客户端的消息
        /// </summary>
        public static void ReadFd()
        {
            ResetCheckRead();
            Socket.Select(checkRead, null, null, 1000);
            //检查可读对象
            for (int i = checkRead.Count - 1; i >= 0; i--)
            {
                Socket s = checkRead[i];
                if (s == listendfd)
                {
                    ReadListenfd(s);
                }
                else
                {
                    ReadClientfd(s);
                }
            }
        }

        /// <summary>
        /// 重置检查列表
        /// </summary>
        public static void ResetCheckRead()
        {
            checkRead.Clear();
            checkRead.Add(listendfd);
            foreach(ClientState state in clients.Values)
            {
                checkRead.Add(state.socket);
            }
        }

        /// <summary>
        /// 处理监听事件
        /// </summary>
        /// <param name="socket"></param>
        public static void ReadListenfd(Socket listenfd)
        {
            try
            {
                //接收到客户端连接
                Socket clientfd= listenfd.Accept();
                Console.WriteLine("Accept " + clientfd.RemoteEndPoint.ToString());
                ClientState state = new ClientState();
                state.socket = clientfd;
                clients.Add(clientfd, state);
            }
            catch (Exception e)
            {
                Console.WriteLine("Accept fail"+e.ToString());
            }
        }

        /// <summary>
        /// 处理客户端消息
        /// </summary>
        /// <param name="clientfd"></param>
        public static void ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            ByteArray readBuff = state.readBuffer;
            //接收
            int count = 0;
            //缓冲区不够则进行清除，若依旧不够，则返回
            //缓冲区长度只有1024，单挑协议超过缓冲区长度时会发生报错，根据需要调整长度
            if(readBuff.Remain<=0)
            {
                OnReceiveData(state);
                //扩容
                readBuff.MoveBytes();
            }
            //罗荣后依旧不够
            if(readBuff.Remain<=0)
            {
                Console.WriteLine("Receive fail,msg length > buff capacity.");
                Close(state);
                return;
            }
            try
            {
                count = clientfd.Receive(readBuff.bytes, readBuff.writeIndex, readBuff.Remain, 0);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Receive Exception ："+ex.ToString());
                Close(state);
                return;
            }
            if(count <=0)
            {
                Console.WriteLine("Socket Close"+clientfd.RemoteEndPoint.ToString());
                Close(state);
                return;
            }
            //消息处理
            readBuff.writeIndex += count;
            //处理二进制消息
            OnReceiveData (state);
            //移动缓冲区
            readBuff.CheckAndMoveBytes();
        }

        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="state"></param>
        private static void OnReceiveData(ClientState state)
        {
            ByteArray readBuff= state.readBuffer;
            byte[] bytes = readBuff.bytes;
            //消息长度
            if(readBuff.Length<2)
            {
                return;
            }
            //消息体长度
            Int16 bodyLength = (Int16)((bytes[readBuff.readIndex + 1] << 8) | bytes[readBuff.readIndex]);
            if(readBuff.Length<bodyLength)
            {
                return;
            }
            readBuff.readIndex += 2;
            //解析协议名
            int nameCount = 0;
            string protoName = ProtobufHelper.DecodeName(bytes, readBuff.readIndex,out nameCount);
            if(string.IsNullOrEmpty(protoName))
            {
                Console.WriteLine("解析协议名错误");
                Close(state);
            }
            readBuff.readIndex += nameCount;
            //解析协议体
            int bodyCount = bodyLength - nameCount;
            MsgBase msgBase = ProtobufHelper.DeserizalizeFromByteAry_PB<MsgBase>(bytes, readBuff.readIndex, bodyCount);
            Console.WriteLine(msgBase.ToString());
            readBuff.readIndex += bodyCount;
            readBuff.CheckAndMoveBytes();
            MessagePatch.PatchMessage(state,msgBase);
            if(readBuff.Length>2)
            {
                OnReceiveData(state);
            }
        }

        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        /// <param name="state"></param>
        public static void Close(ClientState state)
        {
            //事件分发
            MethodInfo method = typeof(EventHandler).GetMethod("OnDisconnect");
            object[] ob = {state };
            method.Invoke(null, ob);
            //关闭
            state.socket.Close();
            clients.Remove(state.socket);
        }


        /// <summary>
        /// 定时器
        /// </summary>
        private static void Timer()
        {
            //消息分发
            MethodInfo method = typeof(EventHandler).GetMethod("OnTimer");
            Object[] obj = { };
            method.Invoke(null, obj);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="state">目标客户端</param>
        /// <param name="msg">消息体</param>
        public static void Send(ClientState state,ProtoBuf.IExtensible msg)
        {
            //目标客户端为空
            if(state==null)
            {
                return;
            }
            //目标客户端断开连接
            if(!state.socket.Connected)
            {
                return;
            }

            byte[] nameBytes = ProtobufHelper.EncodeName(msg);
            byte[] bodyByets = ProtobufHelper.SerializeToByteAry_PB(msg);
            int length = nameBytes.Length + bodyByets.Length;
            //发送消息的当度为 协议名长度+协议体长度+2 (2表示协议长度字节)
            byte[] sendBytes = new byte[length+2];
            //组装长度
            sendBytes[0] = (byte)(length % 256);
            sendBytes[1] = (byte)(length / 256);
            //组装名字
            Array.Copy(nameBytes,0,sendBytes,2,nameBytes.Length);
            //组装消息体
            Array.Copy(bodyByets,0,sendBytes,2+nameBytes.Length,bodyByets.Length);
            //不设置回调
            try
            {
                state.socket.BeginSend(sendBytes, 0, sendBytes.Length,0, null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送消息失败:"+ex.ToString());
            }
        }

        /// <summary>
        /// 关闭服务器socket
        /// </summary>
        public static void Stop()
        {
            listendfd.Shutdown(SocketShutdown.Both);
        }
    }
}
