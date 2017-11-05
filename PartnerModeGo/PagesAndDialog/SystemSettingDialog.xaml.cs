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
    /// SystemSettingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSettingDialog : Window
    {
        public SystemSettingDialog()
        {
            InitializeComponent();
            //Properties.Settings.
            blackColorPicker.SelectedColor = Properties.Settings.Default.BlackStoneColor;
            whiteColorPicker.SelectedColor = Properties.Settings.Default.WhiteStoneColor;
        }

        private void blackColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void whiteColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["BlackStoneColor"] = new SolidColorBrush((Color)blackColorPicker.SelectedColor);
            Properties.Settings.Default.BlackStoneColor = (Color)blackColorPicker.SelectedColor;

            Application.Current.Resources["WhiteStoneColor"] = new SolidColorBrush((Color)whiteColorPicker.SelectedColor);
            Properties.Settings.Default.WhiteStoneColor = (Color)whiteColorPicker.SelectedColor;

            Properties.Settings.Default.Save();
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }

}
