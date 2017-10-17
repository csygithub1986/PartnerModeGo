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

namespace PartnerModeGo.Game
{
    /// <summary>
    /// GameSettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class GameSettingPage : UserControl
    {
        public Action OkAction { get; set; }
        public Action CancelAction { get; set; }

        public GameSettingPage()
        {
            InitializeComponent();

            Array array = Enum.GetValues(typeof(PlayerType));
            PlayerTypes = new PlayerType[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                PlayerTypes[i] = (PlayerType)array.GetValue(i);
            }
        }


        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            OkAction?.Invoke();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelAction?.Invoke();
        }


        public PlayerType[] PlayerTypes
        {
            get { return (PlayerType[])GetValue(PlayerTypesProperty); }
            set { SetValue(PlayerTypesProperty, value); }
        }
        public static readonly DependencyProperty PlayerTypesProperty = DependencyProperty.Register("PlayerTypes", typeof(PlayerType[]), typeof(GameSettingDialog), new PropertyMetadata(null));

    }
}
