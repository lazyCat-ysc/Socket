using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Utility;

public enum DisType
{
    Exception,
    Disconnect,
}

public class SocketClient
{
    private TcpClient client = null;
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;
    //private Serial serial;
    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];
    public static bool loggedIn = false;
    public static SocketClient instance;

    // Use this for initialization
    public SocketClient()
    {
        //serial = new Serial();
        instance = this;
    }

    /// <summary>
    /// 注册代理
    /// </summary>
    public void OnRegister()
    {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
    }

    /// <summary>
    /// 移除代理
    /// </summary>
    public void OnRemove()
    {
        this.Close();
        reader.Close();
        memStream.Close();
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    void ConnectServer(string host, int port)
    {
        client = null;
        try
        {
            IPAddress[] address = Dns.GetHostAddresses(host);
            if (address.Length == 0)
            {
                //Debug.LogError("host invalid");
                return;
            }
            if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
            {
                client = new TcpClient(AddressFamily.InterNetworkV6);
            }
            else
            {
                client = new TcpClient(AddressFamily.InterNetwork);
            }
            client.SendTimeout = 1000;
            client.ReceiveTimeout = 1000;
            client.NoDelay = true;
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), client);
        }
        catch (Exception e)
        {
            //Close(); Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// 连接上服务器
    /// </summary>
    void OnConnect(IAsyncResult asr)
    {
        TcpClient tcpClient = (TcpClient)asr.AsyncState;
        if (tcpClient.Connected)
        {
            Console.WriteLine("连接服务器成功");
            client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), client);
            onSendMsg();
            //NetworkManager.AddEvent(Protocal.Connect, new ByteBuffer());
        }
        else
        {
            Console.WriteLine("连接服务器失败");
        }
        
    }

    public void onSendMsg()
    {
        Console.Write("发送消息:");
        string str = Console.ReadLine();
        outStream = client.GetStream();
        ByteBuffer buff = new ByteBuffer();
        buff.WriteString(str);
        ServerPackage serverPackage = new ServerPackage(0, 1, 1, buff.ToBytes());
        SendMessage(serverPackage);
    }
    /// <summary>
    /// 写数据
    /// </summary>
    void WriteMessage(byte[] message)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            ushort msglen = (ushort)message.Length;
            writer.Write(msglen);
            writer.Write(message);
            writer.Flush();
            if (client != null && client.Connected)
            {
                //NetworkStream stream = client.GetStream();
                byte[] payload = ms.ToArray();
                outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
                outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
            }
            else
            {
                //Debug.LogError("client.connected----->>false");
            }
        }
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    void OnRead(IAsyncResult asr)
    {
        int bytesRead = 0;
        try
        {
            lock (client.GetStream())
            {         //读取字节流到缓冲区
                bytesRead = client.GetStream().EndRead(asr);
            }
            if (bytesRead < 1)
            {                //包尺寸有问题，断线处理
                OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                return;
            }
            byte[] lenBytes = new byte[bytesRead - sizeof(ushort)];
            Array.Copy(byteBuffer, sizeof(ushort), lenBytes, 0, bytesRead - sizeof(ushort));
            PlazaSessionCode sessionCode = (PlazaSessionCode)Serial.GetInstance.Decode(lenBytes, 0, lenBytes.Length);
            MessageOperate msgOperate = new MessageOperate();
            msgOperate.MainPackHanlder(sessionCode.MainCmdId, sessionCode);
            lock (client.GetStream())
            {         //分析完，再次监听服务器发过来的新消息
                Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            //PrintBytes();
            OnDisconnected(DisType.Exception, ex.Message);
        }
    }

    /// <summary>
    /// 丢失链接
    /// </summary>
    void OnDisconnected(DisType dis, string msg)
    {
        Close();   //关掉客户端链接
        //int protocal = dis == DisType.Exception ?
        ////Protocal.Exception : Protocal.Disconnect;

        ////ByteBuffer buffer = new ByteBuffer();
        ////buffer.WriteShort((ushort)protocal);
        ////NetworkManager.AddEvent(protocal, buffer);
        //Debug.LogError("Connection was closed by the server:>" + msg + " Distype:>" + dis);
    }

    /// <summary>
    /// 打印字节
    /// </summary>
    /// <param name="bytes"></param>
    void PrintBytes()
    {
        string returnStr = string.Empty;
        for (int i = 0; i < byteBuffer.Length; i++)
        {
            returnStr += byteBuffer[i].ToString("X2");
        }
        //Debug.LogError(returnStr);
    }

    /// <summary>
    /// 向链接写入数据流
    /// </summary>
    void OnWrite(IAsyncResult r)
    {
        try
        {
            outStream.EndWrite(r);
        }
        catch (Exception ex)
        {
            //Debug.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
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

    /// <summary>
    /// 剩余的字节
    /// </summary>
    private long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    /// <param name="ms"></param>
    void OnReceivedMessage(MemoryStream ms)
    {
        BinaryReader r = new BinaryReader(ms);
        byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));
        //int msglen = message.Length;

        //ByteBuffer buffer = new ByteBuffer(message);
        //int mainId = buffer.ReadShort();
        //NetworkManager.AddEvent(mainId, buffer);
    }


    /// <summary>
    /// 会话发送
    /// </summary>
    void SessionSend(byte[] bytes)
    {
        WriteMessage(bytes);
    }

    /// <summary>
    /// 关闭链接
    /// </summary>
    public void Close()
    {
        if (client != null)
        {
            if (client.Connected) client.Close();
            client = null;
        }
        loggedIn = false;
    }

    /// <summary>
    /// 发送连接请求
    /// </summary>
    public void SendConnect()
    {
        ConnectServer("172.16.9.151", 10085);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage(ServerPackage serverPackage)
    {
        //PlazaSessionCode sessionCode = new PlazaSessionCode(mainCmd, subCmd, buffer.ToBytes());
        byte[] bytes = Serial.GetInstance.Encode(serverPackage);
        SessionSend(bytes);
        //buffer.Close();
    }
}
