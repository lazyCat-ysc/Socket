using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model
{
    public class MainModel : INotifyPropertyChanged
    {
        private string ip;//私有
        public TcpListener tcpListener;

        public MainModel()
        {
            int port = 10085;
            Start(port);
        }

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
            //clients = new SocketBase[1];
            //clients[0] = new SocketBase();
            //for (int i = 0; i < maxConn; i++)
            //{
            //    clients[i] = new PlazaSession();
            //}
            tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            tcpListener.Start();
            Ip = GetLocalIP();
            //tcpListener.BeginAcceptTcpClient(AcceptCb, tcpListener);
            Console.WriteLine("[服务器]启动成功");
        }

        public string Ip
        {
            get { return ip; }//获取值时将私有字段传出；
            set
            {
                ip = value;//赋值时将值传给私有字段
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ip"));//一旦执行了赋值操作说明其值被修改了，则立马通过INotifyPropertyChanged接口告诉UI(IntValue)被修改了
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
