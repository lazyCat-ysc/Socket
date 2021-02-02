using Server.Common.Base;
using Server.Core;
using Server.Model;
using Server.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ViewModel
{
    public class MainViewModel
    {
        public MainModel mainModel { get; set; }
        public CommandBase startCommand { get; set; }
        private SocketHandler socketHandler { get; set; }
        private ServerSocket serverSocket;
        private MySqlUser mySqlUser;

        public MainViewModel()
        {
            serverSocket = new ServerSocket();
            mainModel = new MainModel();
            socketHandler = SocketHandler.GetInstance;
            mySqlUser = MySqlUser.GetInstance;
            mainModel.LaberText = "停止";
            mainModel.ButtonText = "启动";
            startCommand = new CommandBase();
            startCommand.DoExecute = new Action<object>((o) => {
                if(MainModel.status == 0)
                {
                    MainModel.status = 1;
                    mainModel.LaberText = "运行中";
                    mainModel.ButtonText = "停止";
                    serverSocket.Start(mainModel.Port);
                    
                }
                else
                {
                    MainModel.status = 0;
                    mainModel.LaberText = "停止";
                    mainModel.ButtonText = "启动";
                    serverSocket.Close();
                }
            });
            startCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            
        }
    }
}
