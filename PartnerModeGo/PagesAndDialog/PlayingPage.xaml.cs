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
    /// PlayingPage.xaml 的交互逻辑
    /// </summary>
    public partial class PlayingPage : UserControl
    {
        public PlayingPage()
        {
            InitializeComponent();
            m_Board.MousePlayEvent += Board_MousePlayEvent;
        }

        HostServer cal;

        private void Board_MousePlayEvent(int stepNum, int x, int y)
        {
            cal.OutsiderMoveArrived(stepNum, x, y, false, false);
        }


        public PlayingViewModel ViewModel { get { return ((PlayingViewModel)DataContext); } }

        //private void Menu_VsSelfClick(object sender, RoutedEventArgs e)
        //{
        //    //m_Board.BoardMode = BoardMode.AutoPlaying;
        //    m_Board.BoardMode = BoardMode.Playing;
        //    GameSettingDialog w = new GameSettingDialog();
        //    w.DataContext = this.DataContext;
        //    w.Owner = this;
        //    w.WaitingConnect = WaitingConnect;
        //    w.ShowDialog();
        //    if (w.DialogResult == true)
        //    {
        //        cal = new HostServer(ViewModel.Players.ToArray(), ViewModel.GameLoopTimes, 19);
        //        cal.GameOverCallback = GameOver;
        //        cal.UICallback = Play;
        //        cal.TerritoryCallback = ShowTerritory;
        //        cal.WinRateCallback = ShowWinRate;
        //        cal.HandTurnCallback = HandTurnCallback;
        //        cal.Start();
        //    }
        //}

        private void HandTurnCallback(int stepNum, Player2 player)
        {
            switch (player.Type)
            {
                case PlayerType.AI:
                    break;
                case PlayerType.Host:
                    m_Board.IsHostTurn = true;
                    break;
                case PlayerType.RealBoard:
                    break;
            }
        }

        private void WaitingConnect()
        {
            throw new NotImplementedException();
        }

        private void ShowTerritory(int[] territory)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                m_BoardAnalyse.ShowAffects(territory);
            }));
        }

        /// <summary>
        /// stepNum, x, y, isPass, isResign
        /// </summary>
        /// <param name="stepNum"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isPass"></param>
        /// <param name="isResign"></param>
        private void Play(int stepNum, int x, int y, bool isPass, bool isResign)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (stepNum == 0)
                {
                    m_Board.InitGame();
                }
                m_Board.Play(x, y, 2 - stepNum % 2);
            }));
        }

        private void GameOver(int stepNum, int x, int y, bool isPass, bool isResign)
        {
            MessageBox.Show("Over");
        }

        private void ShowWinRate(float bRate)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                blackRate.Width = new GridLength(bRate, GridUnitType.Star);
                whiteRate.Width = new GridLength(1 - bRate, GridUnitType.Star);
                txtBlack.Text = bRate.ToString("F2");
                txtWhite.Text = (1 - bRate).ToString("F2");
            }));
        }

        //打开服务
        private void Menu_OpenServerClick(object sender, RoutedEventArgs e)
        {
            TcpServer.Instance.WindowOnPhoneConnected += OnPhoneConnected;

            TcpDataHandler.Instance.WindowReceivePhoneStepData += ReceivePhoneStepData;
            TcpDataHandler.Instance.WindowReceivePhonePreviewData += ReceivePhonePreviewData;
            TcpServer.Instance.Start();
        }

        private void OnPhoneConnected(string obj)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                foreach (var player in ViewModel.Players)
                {
                    if (player.Type == PlayerType.RealBoard)
                    {
                        player.IsConnected = true;
                    }
                }
            }));
        }

        private void ReceivePhoneStepData(int x, int y, int color, int[] boardState)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                m_Board.Play(x, y, color);
            }));
        }

        private void ReceivePhonePreviewData(byte[] imageBytes, bool isOk, int[] boardState)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                m_Image.Source = ImageHelper.ByteArrayToBitmapImage(imageBytes);

                if (isOk)
                {
                    foreach (var player in ViewModel.Players)
                    {
                        if (player.Type == PlayerType.RealBoard)
                        {
                            player.IsBoardRecognized = true;
                        }
                    }

                    //m_Board.Visibility = Visibility.Visible;
                    //m_Board.InitGame();
                    //m_Board.SetStones(boardState);
                }
                else
                {
                    //m_Board.Visibility = Visibility.Hidden;
                }

                Console.WriteLine("   boardState len: " + boardState.Count());
            }));
        }


        private void Menu_SendStartTestClick(object sender, RoutedEventArgs e)
        {
            TcpDataHandler.Instance.SendGameStart(CommonDataDefine.FileName + "=abc.sgf;" + CommonDataDefine.BlackPlayerName + "=BBB;" + CommonDataDefine.WhitePlayerName + "=WWW;");
        }
    }
}
