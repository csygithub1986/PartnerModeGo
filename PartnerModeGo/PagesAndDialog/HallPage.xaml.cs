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
            ServiceProxy.Instance.JoinGameCallback = JoinGameCallback;//进入棋局的通知
            ServiceProxy.Instance.GameStartCallback = GameStartCallback;
        }

        private void GameStartCallback(int[] blackIDs, int[] whiteIDs, int currentID)
        {
            Dispatcher.Invoke(() =>
            {
                PlayingPage page = new PlayingPage(VM.SelectedGame, blackIDs, whiteIDs, currentID);
                MainWindow.Instance.ChangePageTo(page);
            });
        }

        private void JoinGameCallback(bool success, Game game)
        {
            if (success)
            {
                Dispatcher.Invoke(() =>
                {
                    this.labelWait.Visibility = Visibility.Visible;
                    //PlayingPage page = new PlayingPage(game);
                    //MainWindow.Instance.ChangePageTo(page);
                });
            }
            else
            {
                MessageBox.Show("加入棋局失败");
            }
        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowProcessWindowAsync("正在进入加入......", Join, null, VM.SelectedGame.GameID, VM.SelectedPlayerID);
        }

        private object Join(object[] obj)
        {
            string gameID = obj[0].ToString();
            int playerID = int.Parse(obj[1].ToString());
            ServiceProxy.Instance.ApplyToJoinGame(gameID, playerID);
            return null;
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //控件加载以后，向服务器请求一次所有列表
        }

        private void blackListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckCanJoin();
            if (blackListbox.SelectedItem != null)
            {
                VM.SelectedPlayerID = (blackListbox.SelectedItem as Player).ID;
                whiteListbox.SelectedItem = null;
            }
        }

        private void whiteListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckCanJoin();
            if (whiteListbox.SelectedItem != null)
            {
                VM.SelectedPlayerID = (whiteListbox.SelectedItem as Player).ID;
                blackListbox.SelectedItem = null;
            }
        }

        private void CheckCanJoin()
        {
            if (blackListbox.SelectedItem == null && whiteListbox.SelectedItem == null)
            {
                btn_Join.IsEnabled = false;
            }
            else if (blackListbox.SelectedItem != null)
            {
                btn_Join.IsEnabled = (blackListbox.SelectedItem as Player).Type == WcfService.PlayerType.Internet && !(blackListbox.SelectedItem as Player).Occupied;
            }
            else
            {
                btn_Join.IsEnabled = (whiteListbox.SelectedItem as Player).Type == WcfService.PlayerType.Internet && !(whiteListbox.SelectedItem as Player).Occupied;
            }
        }

    }
}
