using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
class PlazaSessionCode
{
    [Serializable]
    public struct Message
    {
        public int mainCmdId;
        public int subCmdId;
        public byte[] byteBuffer;
    }
    public Message message;

    public PlazaSessionCode()
    {
        message = new Message();
        message.mainCmdId = 0;
        message.subCmdId = 0;
        message.byteBuffer = new byte[0];
    }

    public PlazaSessionCode(int mainCmdId, int subCmdId, byte[] byteBuffer)
    {
        message = new Message();
        message.mainCmdId = mainCmdId;
        message.subCmdId = subCmdId;
        message.byteBuffer = byteBuffer;
    }

    public int MainCmdId
    {
        set
        {
            message.mainCmdId = value;
        }
        get
        {
            return message.mainCmdId;
        }
    }
    public int SubCmdId
    {
        set
        {
            message.subCmdId = value;
        }
        get
        {
            return message.subCmdId;
        }
    }

    public byte[] GetBytes
    {
        get
        {
            return message.byteBuffer;
        }
    }
}

