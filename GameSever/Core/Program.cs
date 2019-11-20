using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data;
using GameSever.Core;
using GameSever.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using GameSever.Serializable;
using GameSever.Operation;

namespace GameSever
{
    class Program
    {
        static void Main(string[] args)
        {
            //DataMgr sql = new DataMgr();
            ////bool isregister = sql.Register("479856082", "64450252", "爱睡觉的懒喵喵");
            //string name = sql.GetUserName("407551879");
            //Console.WriteLine(name);

            //MessageOperate tt = new MessageOperate();
            //tt.PackHanlder(0, new PlazaSessionCode());
            //XmlOperation xml = new XmlOperation();
            //xml.XmlRead();
            SeverData cmd = new SeverData();
            cmd.ReadConfig();
            SeverNet sever = new SeverNet();
            sever.Start("172.16.12.235",6379);
            //sever.protocol = new ProtocolBytes();
            while (true) ;
        }
    }
}
