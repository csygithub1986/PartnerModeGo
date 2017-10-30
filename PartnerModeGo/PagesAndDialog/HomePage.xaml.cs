using System;
using System.Collections.Generic;
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

namespace PartnerModeGo
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : UserControl
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void btn_LocalPlay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            string userName = txtUserName.Text;
            string ip = txtIP.Text;
            MainWindow.Instance.ShowProcessWindowAsync("正在登陆......", Login, LoginCallback, userName, ip);
        }

        private object Login(object[] obj)
        {
            try
            {
                string userName = obj[0].ToString();
                string ip = obj[1].ToString();
                bool openSuccess = ServiceProxy.Instance.ClientOpen(ip);
                if (openSuccess)
                {
                    string ssid = ServiceProxy.Instance.Login(userName);
                    if (ssid != null)
                    {
                        ServiceProxy.Instance.Session.UserName = userName;
                        ServiceProxy.Instance.Session.SessionID = ssid;
                        return true;
                    }
                    else
                    {
                        ServiceProxy.Instance.Session.UserName = null;
                        ServiceProxy.Instance.Session.SessionID = null;
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

        }

        private void LoginCallback(object obj)
        {
            bool reply = (bool)obj;
            if (reply)
            {
                Dispatcher.Invoke(() =>
                {
                    HallPage page = new HallPage();
                    MainWindow.Instance.ChangePageTo(page);
                });
            }
            else
            {
                MessageBox.Show("登陆失败");
            }
        }
    }
}
