using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;


class PlazaSession
{
    public TcpClient client = null;
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;
    public bool isUsed = false;
    private const int MAX_READ = 8192;
    public byte[] byteBuffer = new byte[MAX_READ];
    public static bool loggedIn = false;
    public PlazaSession()
    {
    }
    /// <summary>
    /// 注册代理
    /// </summary>
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
        //bufferCount = 0;
       // lastTicketTime = Sys.GetTimeStamp();
    }
    /// <summary>
    /// 移除代理
    /// </summary>
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
        loggedIn = false;
    }
    public string GetAddress()
    {
        if (!isUsed)
            return "无法获取地址";
        return client.Client.RemoteEndPoint.ToString();
    }
}
