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
                msgOperate.MainPackHanlder(sessionCode.MainCmdId, sessionCode);
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
    //    private void ProcessData(Connection con)
    //    {
    //        if (con.bufferCount < sizeof(Int32))
    //            return;
    //        Array.Copy(con.readBuffer,con.lenBytes,sizeof(Int32));
    //        con.msgLength = BitConverter.ToInt32(con.lenBytes,0);
    //        if (con.bufferCount < con.msgLength + sizeof(Int32))
    //            return;
            
    //        lock(con)
    //        {
    //            Serial ser = new Serial();
    //            MessageData message = new MessageData();
    //            message = (MessageData)ser.Decode(con.readBuffer, sizeof(Int32), con.msgLength);
    //            HandleMainMsg(con, message);
    //        }
    //        int count = con.bufferCount - con.msgLength - sizeof(Int32);
    //        Array.Copy(con.readBuffer, con.msgLength + sizeof(Int32), con.readBuffer, 0, count);
    //        con.bufferCount = count;
    //        if (con.bufferCount > 0)
    //            ProcessData(con);
    //    }

    //    private void HandleMainMsg(Connection con, MessageData messageData)
    //    {
    //        MethodInfo mm = this.GetType().GetMethod(SeverData.instance.GetMainCmd(messageData.data.mainCmdId));
    //        Object[] obj = new object[] { con, messageData };
    //        mm.Invoke(this, obj);
    //        //if (messagesubCmd.subCmd.mainCmdId == 0)
    //        //{
    //        //    HandleSubMsg(con, messagesubCmd);
    //        //}
    //    }

    //    public void HandleSysMsg(Connection con, MessageData messageData)
    //    {
    //        MethodInfo mm = handleConnect.GetType().GetMethod(SeverData.instance.GetSubCmd(messageData.data.subCmdId));
    //        Object[] obj = new object[] { con, messageData };
    //        mm.Invoke(handleConnect,obj);
    //        //if (messagesubCmd.subCmd.subCmdId == 0)
    //        //{
    //        //    //Console.WriteLine("[更新心跳时间]:" + con.GetAddress());
    //        //    con.lastTicketTime = Sys.GetTimeStamp();
    //        //}
    //        //else if(messagesubCmd.subCmd.subCmdId == 1)
    //        //{
    //        //    FixedMessage(con,ref messagesubCmd);
    //        //    Broadcast(con, messagesubCmd);
    //        //}
    //    }

    //    private void AddMessage(Connection con, ref MessageData message,int mainCmd, int subCmd, string msg)
    //    {
    //        ProtocolBytes protocol = new ProtocolBytes();
    //        protocol.AddString(msg);
    //        message.data.mainCmdId = mainCmd;
    //        message.data.subCmdId = subCmd;
    //        message.data.data = protocol;
    //    }

    //    private void FixedMessage(Connection con,ref MessageData message)
    //    {
    //        ProtocolBytes protocol = new ProtocolBytes();
    //        protocol.AddString(con.GetAddress());
    //        protocol.AddString(Sys.GetNowTime());
    //        protocol.AddString(message.data.data.GetString());
    //        message.data.data = protocol;
    //    }

        public void Broadcast(PlazaSession client, MessageData message, bool isTalkSelf = false)
        {
            for (int i = 0; i < clients.Length; i++)
            {
                if (clients[i] == null) continue;
                if (!clients[i].isUsed) continue;
                if (clients[i] == client && !isTalkSelf) continue;
                lock (clients[i])
                {
                    Send(clients[i], message);
                }
            }
        }

    //    private void HandleMsg(Connection con, ProtocolBase protocolBase)
    //    {
    //        string name = protocolBase.GetName();
    //        ProtocolBytes pro = (ProtocolBytes)protocolBase;
    //        int num = pro.GetInt(0);
    //        Console.WriteLine("[收到协议]:" +name + num);
    //        if(name == "HeartBeat")
    //        {
    //           // Console.WriteLine("[更新心跳时间]:" + con.GetAddress());
    //            con.lastTicketTime = Sys.GetTimeStamp();
    //        }
    //    }

    //    private void SetMessagesubCmd()
    //    {

    //    }

    //    public void Broadcast(ProtocolBase protocol)
    //    {
    //        for(int i =0; i < conns.Length;i++)
    //        {
    //            if (conns[i] == null) continue;
    //            if (!conns[i].isUsed) continue;
    //            Send(conns[i],protocol);
    //        }
    //    }

        public bool Send(PlazaSession pla, MessageData messageData)
        {
            outStream = pla.client.GetStream();
            ByteBuffer buff = new ByteBuffer();
            buff.WriteString(str);
            SendMessage(buff, 0, 10);
            //Serial ser = new Serial();
            //byte[] buff = ser.Encode(messageData);
            //byte[] buffLen = BitConverter.GetBytes(buff.Length);
            //byte[] sendBuff = buffLen.Concat(buff).ToArray();
            //uint len = BitConverter.ToUInt32(sendBuff, 0);
            //try
            //{
            //    con.socket.BeginSend(sendBuff, 0, sendBuff.Length, SocketFlags.None, SendCallBack, con);
            //    return true;
            //}
            //catch (Exception e)
            //{
            //    MessageData message = new MessageData();
            //    string msg = "收到 [" + con.GetAddress() + "] 退出聊天房间";
            //    AddMessage(con, ref message, 0, 2, msg);
            //    Broadcast(con, message);
            //    Console.WriteLine("[SeverNet] Send:" + e.Message);
            //    con.Close();
            //    return false;
            //}
        }

    //    private void SendCallBack(IAsyncResult ar)
    //    {
    //        Connection conn = (Connection)ar.AsyncState;
    //        lock (conn)
    //        {
    //           // Console.WriteLine(conn.socket.EndSend(ar));
    //        }
    //    }

    //    public bool Send(Connection con, ProtocolBase protocol)
    //    {
    //        byte[] bytes = protocol.Encode();
    //        byte[] length = BitConverter.GetBytes(bytes.Length);
    //        byte[] sendBuff = length.Concat(bytes).ToArray();
    //        try
    //        {
    //            con.socket.BeginSend(sendBuff, 0, sendBuff.Length, SocketFlags.None, null, null);
    //            return true;
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine("[SeverNet] Send:" + e.Message);
    //            return false;
    //        }
    //    }

    //    public bool Send(Connection con, string str)
    //    {
    //        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
    //        byte[] length = BitConverter.GetBytes(bytes.Length);
    //        byte[] sendBuff = length.Concat(bytes).ToArray();
    //        try
    //        {
    //            con.socket.BeginSend(sendBuff,0,sendBuff.Length,SocketFlags.None,null,null);
    //            return true;
    //        }
    //        catch(Exception e)
    //        {
    //            Console.WriteLine("[SeverNet] Send:"+ e.Message);
    //            return false;
    //        }
    //    }
    //    public void HandleMainTimer(object sender,ElapsedEventArgs e)
    //    {
    //        HeartBeat();
    //        timer.Start();
    //    }
    //    public void HeartBeat()
    //    {
    //        //Console.WriteLine("[主定时器执行]");
    //        long timeNow = Sys.GetTimeStamp();
    //        for(int i = 0;i < conns.Length; i++)
    //        {
    //            Connection con = conns[i];
    //            if (con == null) continue;
    //            if (!con.isUsed) continue;
    //            //Console.WriteLine(con.lastTicketTime);
    //            //Console.WriteLine(timeNow - heartBeatTime);
    //            if (con.lastTicketTime < timeNow - heartBeatTime)
    //            {
    //                Console.WriteLine("心跳引起断开");
    //                lock(con)
    //                {
    //                    MessageData messageData = new MessageData();
    //                    string msg = "收到 [" + con.GetAddress() + "] 退出聊天房间";
    //                    AddMessage(con, ref messageData, 0, 2, msg);
    //                    Broadcast(con, messageData);
    //                    con.Close();
    //                }
                    
    //            }
    //        }
    //    }
    }
}
