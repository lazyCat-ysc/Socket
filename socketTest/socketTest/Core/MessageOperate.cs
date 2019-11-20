using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MessageOperate
{
    private PlazaPackHanlder plazaPackHanlder = new PlazaPackHanlder();
    private delegate void mainPlazaPack(int op, byte[] bytes, PlazaSession client);
    Dictionary<int, mainPlazaPack> mainPack = new Dictionary<int, mainPlazaPack>();
    private delegate void subPlazaPack(byte[] bytes, PlazaSession client);
    Dictionary<int, subPlazaPack> subPack = new Dictionary<int, subPlazaPack>();
    public MessageOperate()
    {
        InitDictionary(); 
    }
    public void MainPackHanlder(int op, PlazaSessionCode code, PlazaSession client)
    {
        if (mainPack.ContainsKey(op))
            mainPack[op](code.SubCmdId, code.GetBytes, client);
    }
    public void SubPackHanlder(int op, byte[] bytes, PlazaSession client)
    {
        if (subPack.ContainsKey(op))
            subPack[op](bytes, client);
    }
    public void InitDictionary()
    {
        mainPack.Add(0, SubPackHanlder);
        subPack.Add(10, plazaPackHanlder.HandleRecevieMsg);
        subPack.Add(1, plazaPackHanlder.HandleRegisterAccount);
    }
}

