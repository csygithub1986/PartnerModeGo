using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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

namespace LeagueGoServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(new Action(() =>
            {
                try
                {
                    ServiceHost _host = new ServiceHost(typeof(WcfService));
                    _host.Open();
                    //Console.WriteLine("启动");
                    Dispatcher.Invoke(() => { txtPrompt.Text = "启动"; });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => { txtPrompt.Text = ex.ToString(); });
                }
            }));
        }
    }
}
