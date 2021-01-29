using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameSever.Core;
using Utility;

class PlazaPackHanlder
{
    //private Serial serial = new Serial();
    PlazaSessionCode sessionCode = new PlazaSessionCode();
    //private DataMgr dataMgr = new DataMgr();
    public void HandleConnectSuccess(byte[] bytes, PlazaSession client)
    {
        //ByteBuffer read = new ByteBuffer(bytes);
        //string readMsg = read.ReadString();
        //string msg = "收到[" + client.GetAddress()+ "]发来消息内容:" + readMsg;
        //ByteBuffer buff = new ByteBuffer();
        //buff.WriteString(msg);
        ////PlazaSessionCode sessionCode = new PlazaSessionCode(0, 11, buff.ToBytes());
        ////byte[] b = serial.Encode(sessionCode);
        //SeverNet.instance.Broadcast(client, buff.ToBytes());
    }
    public void HandleRecevieMsg(byte[] bytes)
    {
        ByteBuffer read = new ByteBuffer(bytes);
        string readMsg = read.ReadString();
        Console.WriteLine(readMsg);
        SocketClient.instance.onSendMsg();
        //string msg = "收到[" + client.GetAddress() + "]发来消息内容:" + readMsg;
        //sessionCode.MainCmdId = 0;
        //sessionCode.SubCmdId = 1002;
        //ByteBuffer buff = new ByteBuffer();
        //buff.WriteString(msg);
        //sessionCode.SetBytes(buff.ToBytes());
        //byte[] byteCode = serial.Encode(sessionCode);
       
    }
    public void HandleRegisterAccount(byte[] bytes)
    {
        //string id = "";
        //string pw = "";
        //string name = "";
        //ByteBuffer read = new ByteBuffer(bytes);
        //id = read.ReadString();
        //pw = read.ReadString();
        //name = read.ReadString();
        //bool registerSucceed = false;
        //if (id != string.Empty && pw != string.Empty && name == string.Empty)
        //    registerSucceed = dataMgr.Register(id, pw, name);
        //string msg = "注册成功！";
        //if (!registerSucceed)
        //{
        //    msg = "注册失败！";
        //}
        //sessionCode.MainCmdId = 0;
        //sessionCode.SubCmdId = 1001;
        //ByteBuffer buff = new ByteBuffer();
        //buff.WriteString(msg);
        //sessionCode.SetBytes(buff.ToBytes());
        //byte[] byteCode = serial.Encode(sessionCode);
        //SeverNet.instance.Send(client, byteCode);
    }
}

