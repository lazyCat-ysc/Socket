using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    [Serializable]
    public class ServerPackage
    {
        private int Command;
        private int MOpcode;
        private int SOpcode;
        private byte[] BufferData;

        public ServerPackage(int command, int mOpcode, int sOpcode)
        {
            Command = command;
            MOpcode = mOpcode;
            SOpcode = sOpcode;
        }

        public ServerPackage(int command, int mOpcode, int sOpcode, byte[] data)
        {
            Command = command;
            MOpcode = mOpcode;
            SOpcode = sOpcode;
            BufferData = data;
        }

        public void SetBufferData(byte[] data)
        {
            BufferData = data;
        }

        public byte[] GetBufferData()
        {
            return BufferData;
        }

        public int GetCommand()
        {
            return Command;
        }

        public int GetMOpcode()
        {
            return MOpcode;
        }

        public int GetSOpcode()
        {
            return SOpcode;
        }
    }
}
