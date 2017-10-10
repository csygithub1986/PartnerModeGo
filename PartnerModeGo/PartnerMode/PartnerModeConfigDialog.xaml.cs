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
using System.Windows.Shapes;

namespace PartnerModeGo
{
    /// <summary>
    /// SelfVsConfigWin.xaml 的交互逻辑
    /// </summary>
    public partial class PartnerModeConfigDialog : Window
    {
        public int BlackSimNum { get; set; }
        public int BlackThinkTime { get; set; }
        public int WhiteSimNum { get; set; }
        public int WhiteThinkTime { get; set; }
        public int TotalGameCount { get; set; }

        public PartnerModeConfigDialog()
        {
            InitializeComponent(); 
        }

        private void Rb_Checked(object sender, RoutedEventArgs e)
        {
            //if (rbBlack.IsChecked == true)
            //{
            //    gbBlack.IsEnabled = true;
            //    gbWhite.IsEnabled = false;
            //}
            //else if (rbWhite.IsChecked == true)
            //{
            //    gbBlack.IsEnabled = false;
            //    gbWhite.IsEnabled = true;
            //}
            //else
            //{
            //    gbBlack.IsEnabled = true;
            //    gbWhite.IsEnabled = true;
            //}
            //btnOk.IsEnabled = true;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            //BlackSimNum = int.Parse(txtBlackSimNum.Text);
            //WhiteSimNum = int.Parse(txtWhiteSimNum.Text);
            //BlackThinkTime = int.Parse(txtBlackThinkTime.Text);
            //WhiteSimNum = int.Parse(txtWhiteSimNum.Text);
            //TotalGameCount = int.Parse(txtGameCount.Text);
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
