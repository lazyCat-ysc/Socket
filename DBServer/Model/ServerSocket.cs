using Server.Common;
using Server.Core;
using Server.ViewModel;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Utility;

namespace Server.Model
{
    public class ServerSocket
    {
        public SocketBase[] clients;
        public static int maxConn = 50;
        public TcpListener tcpListener;

        public void Start(int port)
        {
            //定时器
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
            //timer.AutoReset = false;
            //timer.Enabled = true;
            //链接池
            clients = new SocketBase[maxConn];
            for (int i = 0; i < maxConn; i++)
            {
                clients[i] = new SocketBase();
            }
            ServerEvent.GetInstance.SendPackage(ServerEvent.EventType.EVENT_SEND_PACKAGE, new ServerPackage(10,20,30));
            tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(AcceptCb, tcpListener);
            Console.WriteLine("[服务器]启动成功");
        }

        public int NewIndex()
        {
            if (clients == null)
                return -1;
            for (int i = 0; i < clients.Length; i++)
            {
                if (clients[i] == null)
                {
                    clients[i] = new SocketBase();
                    return i;
                }
                else if (clients[i].isUsed == false)
                {
                    return i;
                }
            }
            return -1;
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
                    Console.WriteLine("[警告]链接已满");
                }
                else
                {
                    SocketBase socketBase = clients[index];
                    socketBase.Init(client);
                    //string adr = socketBase.GetAddress();
                    //Console.WriteLine("客户端连接 [" + adr + "] conn池ID：" + index);
                    //MessageData messageData = new MessageData();
                    //string msg = " [" + plazaSession.GetAddress() + "] 加入聊天房间,当前房间人数[" + GetOnlineCount() + "]";
                    //plazaSession.client.GetStream().BeginRead(plazaSession.byteBuffer, 0, plazaSession.GetMaxRead(), new AsyncCallback(ReceiveCb), plazaSession);
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

        public void Close()
        {
            
            for (int i = 0; i < clients.Length; i++)
            {
                SocketBase con = clients[i];
                if (con == null)
                    continue;
                if (!con.isUsed)
                    continue;
                lock (con)
                {
                    con.Close();
                }
            }
            tcpListener.Stop();
        }
    }
}
