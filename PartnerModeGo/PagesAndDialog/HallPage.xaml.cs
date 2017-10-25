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
    /// HallPage.xaml 的交互逻辑
    /// </summary>
    public partial class HallPage : UserControl
    {
        private HallViewModel VM;
        public HallPage()
        {
            InitializeComponent();
            VM = new HallViewModel();
            DataContext = VM;
        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //控件加载以后，向服务器请求一次所有列表

        }
    }
}
