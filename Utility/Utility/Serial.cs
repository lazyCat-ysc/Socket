using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class Serial
    {
        private static Serial Instance;

        public static Serial GetInstance
        {
            get
            {
                if (Instance == null)
                    Instance = new Serial();
                return Instance;
            }
        }

        public byte[] Encode(object subCmd)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            try
            {
                formatter.Serialize(stream, subCmd);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("[Serializable] Encode 序列化:" + e.Message);
            }
            return stream.ToArray();
        }

        public object Decode(byte[] bytes, int start, int len)
        {
            byte[] readBuff = new byte[len];
            Array.Copy(bytes, start, readBuff, 0, len);
            object message = new object();
            MemoryStream stream = new MemoryStream(readBuff);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                message = formatter.Deserialize(stream);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("[Serializable] Decode 反序列化:" + e.Message);
            }
            return message;
        }
    }
}
