using Server.Core;
using Server.ViewModel;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server.Model
{
    public class ServerSocket
    {
        public SocketBase[] clients;
        public TcpListener tcpListener;
        public static string ip;

        public static string GetLocalIP()
        {
            try
            {

                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        string ip = "";
                        ip = IpEntry.AddressList[i].ToString();
                        int w = ip.LastIndexOf(".");
                        string q = ip.Substring(w + 1);
                        if (q.Equals("1"))
                            continue;
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void Start(int port)
        {
            //定时器
            //timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
            //timer.AutoReset = false;
            //timer.Enabled = true;
            //链接池
            clients = new SocketBase[1];
            clients[0] = new SocketBase();
            //for (int i = 0; i < maxConn; i++)
            //{
            //    clients[i] = new PlazaSession();
            //}
            tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            tcpListener.Start();
            ip = GetLocalIP();
            //tcpListener.BeginAcceptTcpClient(AcceptCb, tcpListener);
            Console.WriteLine("[服务器]启动成功");
        }
    }
}
