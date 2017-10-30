using PartnerModeGo.WcfService;
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
        public Action WaitingConnect { get; set; }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
        //    bool hasNet = (DataContext as MainWindowVM).Players.Count(p => p.Type == PlayerType.RealBoard) != 0;

        //    if (m_step == 1 && hasNet)//而且有网络的玩家
        //    {
        //        m_step = 2;
        //        WaitingConnect?.Invoke();
        //        btnOk.Content = "等待连接...";
        //        btnOk.IsEnabled = false;


        //        //让连接和识别后，界面改变
        //        foreach (Player player in (DataContext as MainWindowVM).Players)
        //        {
        //            player.ConnectChanged += Player_RecognizeChangedOrConnectChanged;
        //        }
        //    }
        //    else
        //    {
        //        //开始游戏
        //        DialogResult = true;
        //    }
        //}

        ////使能或禁用确定按钮
        //private void Player_RecognizeChangedOrConnectChanged(Player player, bool value)
        //{
        //    foreach (Player p in (DataContext as MainWindowVM).Players)
        //    {
        //        //目前只有真实棋盘，以后做局域网和互联网
        //        if (p.Type == PlayerType.RealBoard)
        //        {
        //            if (!p.IsConnected || !p.IsBoardRecognized)
        //            {
        //                btnOk.Content = "等待连接...";
        //                btnOk.IsEnabled = false;
        //                return;
        //            }
        //        }
        //        btnOk.Content = "开始游戏";
        //        btnOk.IsEnabled = true;
        //    }
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
