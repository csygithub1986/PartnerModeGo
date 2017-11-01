﻿using PartnerModeGo.WcfService;
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
    /// 进入Room以后，如果是Host：
    /// 1、监听游戏更改事件，并判断是否可以开始游戏
    /// 2、加入游戏、退出游戏
    /// 如果是CLinet：
    /// 1、监听游戏更改和退出事件
    /// 2、监听游戏开始事件
    /// </summary>
    public partial class RoomPage : UserControl
    {
        public RoomPage()
        {
            InitializeComponent();
            //if (game != null)
            //{
            //    VM = new RoomViewModel(game);
            //}
            //else
            //{
            VM = new RoomViewModel();
            //}
            DataContext = VM;
            AddEventHandler();

            Array array = Enum.GetValues(typeof(PlayerType));
            PlayerTypes = new PlayerType[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                PlayerTypes[i] = (PlayerType)array.GetValue(i);
            }
        }

        private RoomViewModel VM { get; set; }

        private void AddEventHandler()
        {
            ServiceProxy.Instance.DistributeGameInfoCallback = DistributeGameInfoCallback;
            ServiceProxy.Instance.GameStartCallback = GameStartCallback;
        }

        #region 服务器回调函数
        private void DistributeGameInfoCallback(GameDistributeType type, Game game)
        {
            if (type == GameDistributeType.Update)
            {
                if (game.GameID == ServiceProxy.Instance.Session.SessionID)
                {
                    VM.CurrentGame = game;
                    VM.Players = new System.Collections.ObjectModel.ObservableCollection<Player>(game.Players);
                    CheckCanStart();
                }
            }
            else if (type == GameDistributeType.Add)
            {
                if (game.GameID == ServiceProxy.Instance.Session.SessionID)
                {
                    Dispatcher.Invoke(() =>
                    {
                        VM.CurrentGame = game;
                        VM.Players = new System.Collections.ObjectModel.ObservableCollection<Player>(game.Players);
                        blackListbox.Margin = new Thickness(0);
                        whiteListbox.Margin = new Thickness(0);
                        btnSetting.Visibility = Visibility.Hidden;
                        btnStart.Visibility = Visibility.Visible;
                        CheckCanStart();
                    });
                }
            }
        }
        private void GameStartCallback(int[] blackIDs, int[] whiteIDs, int currentID)
        {
            Dispatcher.Invoke(() =>
            {
                PlayingPage page = new PlayingPage(VM.CurrentGame, LocalType.Host, 0, blackIDs, whiteIDs, currentID);
                MainWindow.Instance.ChangePageTo(page);
            });
        }

        #endregion

        private void CheckCanStart()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var player in VM.CurrentGame.Players)
                {
                    if (player.Type == PlayerType.Internet)
                    {
                        if (player.Occupied == false)
                        {
                            btnStart.IsEnabled = false;
                            return;
                        }
                    }
                    else if (player.Type == PlayerType.RealBoard)
                    {
                        if (player.IsBoardRecognized == false)
                        {
                            btnStart.IsEnabled = false;
                            return;
                        }
                    }
                }
                btnStart.IsEnabled = true;
            });

        }

        private void blackListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (blackListbox.SelectedItem != null)
            {
                VM.SelectedPlayer = blackListbox.SelectedItem as Player;
                whiteListbox.SelectedItem = null;
            }
        }

        private void whiteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (whiteListbox.SelectedItem != null)
            {
                VM.SelectedPlayer = whiteListbox.SelectedItem as Player;
                blackListbox.SelectedItem = null;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowProcessWindowAsync("正在确认棋局......", SendGame, null);
        }

        private object SendGame(object[] arg)
        {
            ServiceProxy.Instance.CreateGame(VM.CurrentGame.Players, VM.CurrentGame.GameSetting);
            return null;
        }



        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ShowProcessWindowAsync("正在开始棋局......", StartGame, null);

        }

        private object StartGame(object[] arg)
        {
            ServiceProxy.Instance.GameStart();
            return null;
        }

        public PlayerType[] PlayerTypes
        {
            get { return (PlayerType[])GetValue(PlayerTypesProperty); }
            set { SetValue(PlayerTypesProperty, value); }
        }
        public static readonly DependencyProperty PlayerTypesProperty = DependencyProperty.Register("PlayerTypes", typeof(PlayerType[]), typeof(RoomPage), new PropertyMetadata(null));


    }
}