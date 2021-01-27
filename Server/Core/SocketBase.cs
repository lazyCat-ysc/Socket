using System;
using System.IO;
using System.Net.Sockets;

namespace Server.Core
{
    public class SocketBase
    {
        public TcpClient client = null;
        private NetworkStream outStream = null;
        private MemoryStream memStream;
        private BinaryReader reader;
        public bool isUsed = false;
        private const int MAX_READ = 8192;
        public byte[] byteBuffer = new byte[MAX_READ];

        public int GetMaxRead()
        {
            return MAX_READ;
        }

        public void OnRegister()
        {
            memStream = new MemoryStream();
            reader = new BinaryReader(memStream);
        }

        public void Init(TcpClient client)
        {
            this.client = client;
            isUsed = true;
        }

        public void OnRemove()
        {
            Console.WriteLine("remove");
            this.Close();
            reader.Close();
            memStream.Close();
        }

        public void Close()
        {
            if (client != null)
            {
                if (client.Connected) client.Close();
                client = null;
            }
        }

        public string GetAddress()
        {
            if (!isUsed)
                return "无法获取地址";
            return client.Client.RemoteEndPoint.ToString();
        }
    }
}
