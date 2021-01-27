using Server.Model;
using Server.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Server.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        
        //public MainViewModel MainViewModelProperty { get { return w; } }

        public MainView()
        {

            InitializeComponent();
            DataContext = new MainViewModel();
            //MainViewModel w = new MainViewModel();
            //w.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(test);
            //laber1.DataContext = w;
            //ServerSocket serverSocket = new ServerSocket();
            //int port = 10085;
            //laber3.Content = port;
            //serverSocket.Start(port);
            
            //w.IntValue = ServerSocket.ip;
            
        }

        //private void test(object sender, PropertyChangedEventArgs e)
        //{
            
        //}
    }
}
