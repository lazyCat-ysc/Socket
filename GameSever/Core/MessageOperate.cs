using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MessageOperate
{
    private delegate void mainPlazaPack(int op, byte[] bytes);
    Dictionary<int, mainPlazaPack> mainPack = new Dictionary<int, mainPlazaPack>();
    private delegate void subPlazaPack(byte[] bytes);
    Dictionary<int, subPlazaPack> subPack = new Dictionary<int, subPlazaPack>();
    public MessageOperate()
    {
        InitDictionary(); 
    }
    public void MainPackHanlder(int op, PlazaSessionCode code)
    {
        if (mainPack.ContainsKey(op))
            mainPack[op](code.SubCmdId,code.GetBytes);
    }
    public void SubPackHanlder(int op, byte[] bytes)
    {
        if (subPack.ContainsKey(op))
            subPack[op](bytes);
    }
    public void InitDictionary()
    {
        mainPack.Add(0, SubPackHanlder);
        subPack.Add(10, new PlazaPackHanlder().HandleConnectSuccess);
    }
}

