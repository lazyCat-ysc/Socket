using Server.Common;
using Server.Common.Base;
using System.Net.Sockets;

namespace Server.Model
{
    public class MainModel : NotifyBase
    {
        private string ip;//私有
        private int port;
        private string buttonText;
        private string laberText;
        private int textBox;
        public static int status = 0;
        public TcpListener tcpListener;

        public int TextBox
        {
            get { return textBox; }
            set
            {
                textBox = value;
                ServerSocket.maxConn = value;
                DoNotify("TextBox");
            }
        }

        public string LaberText
        {
            get { return laberText; }
            set
            {
                laberText = value;
                DoNotify("LaberText");
            }
        }

        public string ButtonText
        {
            get { return buttonText; }
            set
            {
                buttonText = value;
                DoNotify("ButtonText");
            }
        }

        public string Ip
        {
            get { return ip; }
            set
            {
                ip = value;
                DoNotify("Ip");
            }
        }

        public int Port
        {
            get { return port; }
            set
            {
                port = value;
                DoNotify("Port");
            }
        }

        public MainModel()
        {
            //ip = Utilty.GetLocalIP();
            //port = 10085;
            TextBox = ServerSocket.maxConn;
            Ip = Utilty.GetLocalIP(); ;
            Port = 10085;
            //int port = 10085;
            //Start(port);
        }

        public void Start()
        {
            
        }
    }
}
