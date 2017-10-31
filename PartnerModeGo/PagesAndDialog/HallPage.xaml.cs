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
        private int m_MyPlayerID;
        private Game m_JoinedGame;

        public HallPage()
        {
            InitializeComponent();
            VM = new HallViewModel();
            DataContext = VM;

        }

        private void GameStartCallback(int[] blackIDs, int[] whiteIDs, int currentID)
        {
            Dispatcher.Invoke(() =>
            {
                PlayingPage page = new PlayingPage(m_JoinedGame, LocalType.Client, m_MyPlayerID, blackIDs, whiteIDs, currentID);
                MainWindow.Instance.ChangePageTo(page);
            });
        }

        private void JoinGameCallback(bool success, string gameID, int playerID)
        {
            if (success)
            {
                Dispatcher.Invoke(() =>
                {
                    Player player = ServiceProxy.Instance.Session.GameList.First(p => p.GameID == gameID).Players.First(p => p.ID == playerID);
                    player.Name = ServiceProxy.Instance.Session.UserName;
                    //player.Name=
                    this.labelWait.Visibility = Visibility.Visible;
                    //PlayingPage page = new PlayingPage(game);
                    //MainWindow.Instance.ChangePageTo(page);
                    listBoxGameList.IsEnabled = false;
                    m_MyPlayerID = playerID;
                });
            }
            else
            {
                MessageBox.Show("加入棋局失败");
            }
        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            m_JoinedGame = VM.SelectedGame;
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
            RoomPage page = new RoomPage();
            MainWindow.Instance.ChangePageTo(page);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            //控件加载以后，向服务器请求一次所有列表
            MainWindow.Instance.ShowProcessWindowAsync("正在进入加入......", GetAllGame, null);

            //ServiceProxy.Instance.DistributeGameInfoCallback = DistributeGameInfoCallback;
            ServiceProxy.Instance.JoinGameCallback = JoinGameCallback;//进入棋局的通知
            ServiceProxy.Instance.GameStartCallback = GameStartCallback;
        }


        private object GetAllGame(object[] arg)
        {
            ServiceProxy.Instance.GetAllGames();
            return null;
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
