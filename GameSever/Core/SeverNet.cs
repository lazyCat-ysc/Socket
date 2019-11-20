using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using GameSever.Serializable;
using GameSever.Data;
using GameSever.Logic;
using System.IO;
using System.Reflection;

namespace GameSever.Core
{
    class SeverNet
    {
        public PlazaSession[] clients;
        public TcpListener tcpListener;
    //    public Connection[] conns;
        public int maxConn = 50;
        public static SeverNet instance;
        private Serial serial;
        private NetworkStream outStream = null;
    //    Timer timer = new Timer(1000);
    //    public long heartBeatTime = 180;
    //    public ProtocolBase protocol;
    //    private HandleConnect handleConnect;
    //    private SeverData severData;
        public SeverNet()
        {
            serial = new Serial();
            //handleConnect = new HandleConnect();
            instance = this;
        }
        ~SeverNet()
        {
            Close();
        }

        private int GetOnlineCount()
        {
            int count = 0;
            for (int i = 0; i < clients.Length; i++)
            {
                if (clients[i] == null) continue;
                if (!clients[i].isUsed) continue;
                count += 1;
            }
            return count;
        }

        public int NewIndex()
        {
            if (clients == null)
                return -1;
            for (int i = 0; i < clients.Length; i++)
            {
                if (clients[i] == null)
                {
                    clients[i] = new PlazaSession();
                    return i;
                }
                else if (clients[i].isUsed == false)
                {
                    return i;
                }
            }
            return -1;
        }

        private void ReceiveCb(IAsyncResult ar)
        {
            PlazaSession plazaSession = (PlazaSession)ar.AsyncState;
            int bytesRead = 0;
            try
            {
                lock (plazaSession.client.GetStream())
                {         //读取字节流到缓冲区
                    bytesRead = plazaSession.client.GetStream().EndRead(ar);
                }
                if (bytesRead < 1)
                {                //包尺寸有问题，断线处理
                    //OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                    return;
                }
                byte[] lenBytes = new byte[bytesRead - sizeof(ushort)];
                Array.Copy(plazaSession.byteBuffer, sizeof(ushort), lenBytes, 0, bytesRead - sizeof(ushort));
                PlazaSessionCode sessionCode = (PlazaSessionCode)serial.Decode(lenBytes, 0, lenBytes.Length);
                MessageOperate msgOperate = new MessageOperate();
                msgOperate.MainPackHanlder(sessionCode.MainCmdId, sessionCode, plazaSession);
                //ByteBuffer buff = new ByteBuffer(sessionCode.message.byteBuffer);
                //Console.WriteLine(sessionCode.message.mainCmdId);
                //Console.WriteLine(sessionCode.message.subCmdId);
                //string w = buff.ReadString();
                //string q = buff.ReadString();
                //Console.WriteLine(w);
                //Console.WriteLine(q);
                //OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层
                lock (plazaSession.client.GetStream())
                {         //分析完，再次监听服务器发过来的新消息
                    Array.Clear(plazaSession.byteBuffer, 0, plazaSession.byteBuffer.Length);   //清空数组
                    plazaSession.client.GetStream().BeginRead(plazaSession.byteBuffer, 0, plazaSession.GetMaxRead(), new AsyncCallback(ReceiveCb), plazaSession);
                }
            }
            catch (Exception ex)
            {
                //PrintBytes();
                //OnDisconnected(DisType.Exception, ex.Message);
            }
        }
        public void Start(string host, int port)
        {
            //定时器
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
            //timer.AutoReset = false;
            //timer.Enabled = true;
            //链接池
            clients = new PlazaSession[maxConn];
            for (int i = 0; i < maxConn; i++)
            {
                clients[i] = new PlazaSession();
            }
            tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, 6379));
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(AcceptCb, tcpListener);
            Console.WriteLine("[服务器]启动成功");
        }

        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                TcpListener listener = (TcpListener)ar.AsyncState;
                TcpClient client = (TcpClient)listener.EndAcceptTcpClient(ar);
                int index = NewIndex();

                if (index < 0)
                {
                    client.Close();
                    Console.Write("[警告]链接已满");
                }
                else
                {
                    PlazaSession plazaSession = clients[index];
                    plazaSession.Init(client);
                    string adr = plazaSession.GetAddress();
                    Console.WriteLine("客户端连接 [" + adr + "] conn池ID：" + index);
                    MessageData messageData = new MessageData();
                    string msg = " [" + plazaSession.GetAddress() + "] 加入聊天房间,当前房间人数[" + GetOnlineCount() + "]";
                    plazaSession.client.GetStream().BeginRead(plazaSession.byteBuffer, 0, plazaSession.GetMaxRead(), new AsyncCallback(ReceiveCb), plazaSession);
                    //plazaSession.client
                    //AddMessage(conn, ref messageData, 0, 3, msg);
                    //Broadcast(conn, messageData,true);
                    //conn.socket.BeginReceive(conn.readBuffer,
                    //                         conn.bufferCount, conn.Buffremain(),
                    //                         SocketFlags.None, ReceiveCb, conn);
                }
                tcpListener.BeginAcceptTcpClient(AcceptCb, tcpListener);
            }
            catch (Exception e)
            {
                Console.WriteLine("AcceptCb失败:" + e.Message);
            }
        }
    //    private void AcceptCb(IAsyncResult ar)
    //    {
    //        try
    //        {
    //            Socket socket = listenfd.EndAccept(ar);
    //            int index = NewIndex();

    //            if (index < 0)
    //            {
    //                socket.Close();
    //                Console.Write("[警告]链接已满");
    //            }
    //            else
    //            {
    //                Connection conn = conns[index];
    //                conn.Init(socket);
    //                string adr = conn.GetAddress();
    //                Console.WriteLine("客户端连接 [" + adr + "] conn池ID：" + index);
    //                MessageData messageData = new MessageData();
    //                string msg = " [" + conn.GetAddress() + "] 加入聊天房间,当前房间人数[" + GetOnlineCount() + "]";
    //                //AddMessage(conn, ref messageData, 0, 3, msg);
    //                //Broadcast(conn, messageData,true);
    //                conn.socket.BeginReceive(conn.readBuffer,
    //                                         conn.bufferCount, conn.Buffremain(),
    //                                         SocketFlags.None, ReceiveCb, conn);
    //            }
    //            listenfd.BeginAccept(AcceptCb, null);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine("AcceptCb失败:" + e.Message);
    //        }
    //    }
        public void Close()
        {
            //for(int i = 0;i<conns.Length;i++)
            //{
            //    Connection con = conns[i];
            //    if (con == null)
            //        continue;
            //    if (!con.isUsed)
            //        continue;
            //    lock(con)
            //    {
            //        con.Close();
            //    }
            //}
        }
        public void Broadcast(PlazaSession client, byte[] bytes, bool isTalkSelf = false)
        {
            for (int i = 0; i < clients.Length; i++)
            {
                if (clients[i] == null) continue;
                if (!clients[i].isUsed) continue;
                if (clients[i] == client && !isTalkSelf) continue;
                lock (clients[i])
                {
                    Send(clients[i], bytes);
                }
            }
        }
        void OnWrite(IAsyncResult r)
        {
            try
            {
                outStream.EndWrite(r);
            }
            catch (Exception ex)
            {
                Console.WriteLine("OnWrite--->>>" + ex.Message);
                //Debug.LogError("OnWrite--->>>" + ex.Message);
            }
        }
        public void Send(PlazaSession pla, byte[] bytes)
        {
            MemoryStream ms = null;
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                ushort msglen = (ushort)bytes.Length;
                writer.Write(msglen);
                writer.Write(bytes);
                writer.Flush();
                if (pla.client != null && pla.client.Connected)
                {
                    outStream = pla.client.GetStream();
                    //NetworkStream stream = client.GetStream();
                    byte[] payload = ms.ToArray();
                    outStream.BeginWrite(payload, 0, payload.Length, null, null);
                }
            }
        }
    }
}
