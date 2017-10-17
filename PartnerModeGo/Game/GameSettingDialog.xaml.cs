using PartnerModeGo.Game;
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

namespace PartnerModeGo.Game
{
    /// <summary>
    /// SelfVsConfigWin.xaml 的交互逻辑
    /// </summary>
    public partial class GameSettingDialog : Window
    {
        public GameSettingDialog()
        {
            InitializeComponent();

            Array array = Enum.GetValues(typeof(PlayerType));
            PlayerTypes = new PlayerType[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                PlayerTypes[i] = (PlayerType)array.GetValue(i);
            }
        }

        int m_step = 1;//1是配置完成，2是开始游戏
        public Action SettingDoneAction { get; set; }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            bool blackHasNet = (DataContext as MainWindowVM).BlackPlayers.Count(p => p.Type != PlayerType.AI && p.Type != PlayerType.Host) != 0;
            bool whiteHasNet = (DataContext as MainWindowVM).WhitePlayers.Count(p => p.Type != PlayerType.AI && p.Type != PlayerType.Host) != 0;

            if (m_step == 1 && (blackHasNet || whiteHasNet))//而且有网络的玩家
            {
                m_step = 2;
                SettingDoneAction?.Invoke();
                btnOk.Content = "等待连接...";
            }
            else
            {
                //开始游戏
                DialogResult = true;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


        public PlayerType[] PlayerTypes
        {
            get { return (PlayerType[])GetValue(PlayerTypesProperty); }
            set { SetValue(PlayerTypesProperty, value); }
        }
        public static readonly DependencyProperty PlayerTypesProperty = DependencyProperty.Register("PlayerTypes", typeof(PlayerType[]), typeof(GameSettingDialog), new PropertyMetadata(null));

    }
}
