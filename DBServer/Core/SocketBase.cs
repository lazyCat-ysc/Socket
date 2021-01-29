using Server.Common;
using System;
using System.IO;
using System.Net.Sockets;
using Utility;

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
            this.client.SendTimeout = 1000;
            this.client.ReceiveTimeout = 1000;
            this.client.NoDelay = true;
            isUsed = true;
            OnRegister();
            this.client.GetStream().BeginRead(byteBuffer, 0, GetMaxRead(), new AsyncCallback(ReceiveCb), this.client);
        }

        private void ReceiveCb(IAsyncResult ar)
        {
            int bytesRead = 0;
            try
            {
                lock (this.client.GetStream())
                {         //读取字节流到缓冲区
                    bytesRead = this.client.GetStream().EndRead(ar);
                }
                if (bytesRead < 1)
                {                //包尺寸有问题，断线处理
                    OnRemove();
                    return;
                }
                OnReceive(byteBuffer, bytesRead);
                lock (this.client.GetStream())
                {         //分析完，再次监听服务器发过来的新消息
                    Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                    this.client.GetStream().BeginRead(byteBuffer, 0, GetMaxRead(), new AsyncCallback(ReceiveCb), this.client);
                }
            }
            catch (Exception ex)
            {
                OnRemove();
            }
        }

        private long RemainingBytes()
        {
            return memStream.Length - memStream.Position;
        }

        void OnReceive(byte[] bytes, int length)
        {
            memStream.Seek(0, SeekOrigin.End);
            memStream.Write(bytes, 0, length);
            //Reset to beginning
            memStream.Seek(0, SeekOrigin.Begin);
            while (RemainingBytes() > 2)
            {
                ushort messageLen = reader.ReadUInt16();
                if (RemainingBytes() >= messageLen)
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(ms);
                    writer.Write(reader.ReadBytes(messageLen));
                    ms.Seek(0, SeekOrigin.Begin);
                    OnReceivedMessage(ms);
                }
                else
                {
                    //Back up the position two bytes
                    memStream.Position = memStream.Position - 2;
                    break;
                }
            }
            //Create a new stream with any leftover bytes
            byte[] leftover = reader.ReadBytes((int)RemainingBytes());
            memStream.SetLength(0);     //Clear
            memStream.Write(leftover, 0, leftover.Length);
        }

        void OnReceivedMessage(MemoryStream ms)
        {
            BinaryReader r = new BinaryReader(ms);
            byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));
            ServerPackage serverPackage = (ServerPackage)Serial.GetInstance.Decode(message, 0, message.Length);
            ServerEvent.GetInstance.SendPackage(ServerEvent.EventType.EVENT_SEND_PACKAGE, serverPackage);
        }

        public void Send(ServerPackage serverPackage)
        {
            MemoryStream ms = null;
            byte[] bytes = Serial.GetInstance.Encode(serverPackage);
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                ushort msglen = (ushort)bytes.Length;
                writer.Write(msglen);
                writer.Write(bytes);
                writer.Flush();
                if (client != null && client.Connected)
                {
                    outStream = client.GetStream();
                    byte[] payload = ms.ToArray();
                    outStream.BeginWrite(payload, 0, payload.Length, null, null);
                }
            }
        }

        public void OnRemove()
        {
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
            isUsed = false;
        }

        public string GetAddress()
        {
            if (!isUsed)
                return "无法获取地址";
            return client.Client.RemoteEndPoint.ToString();
        }
    }
}
