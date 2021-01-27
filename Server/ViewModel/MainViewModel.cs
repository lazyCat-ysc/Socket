using Server.Model;
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
        public MainViewModel()
        {
            mainModel = new MainModel();
            mainModel.PropertyChanged += new PropertyChangedEventHandler(UpdateIp);
            //mainModel.Ip = "46464";
            int i = 0;
        }

        private void UpdateIp(object sender, PropertyChangedEventArgs e)
        {

        }
    }
}
