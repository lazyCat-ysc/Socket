using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameSever.Core;


class PlazaPackHanlder
{
    public void HandleConnectSuccess(byte[] bytes, PlazaSession client)
    {
        ByteBuffer buff = new ByteBuffer(bytes);
        string w = buff.ReadString();
        Console.WriteLine(w);
        Console.WriteLine("ConnectSuccess");
        SeverNet.instance.Broadcast(client,bytes);
    }
}

