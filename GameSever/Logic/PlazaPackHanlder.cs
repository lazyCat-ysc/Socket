using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PlazaPackHanlder
{
    public void HandleConnectSuccess(byte[] bytes)
    {
        ByteBuffer buff = new ByteBuffer(bytes);
        string w = buff.ReadString();
        Console.WriteLine(w);
        Console.WriteLine("ConnectSuccess");
    }
}

