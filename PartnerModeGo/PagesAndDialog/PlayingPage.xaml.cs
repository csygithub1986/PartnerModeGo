﻿using Microsoft.WindowsAPICodePack.Dialogs;
using PartnerModeGo.WcfService;
using System;
using System.Collections.Generic;
using System.IO;
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
        //private Game m_Game;
        private LocalType m_LocalType;
        //private int m_MyPlayerID;//当是client的时候用
        private AI m_AI;
        //private int m_StepNum;
        //private StringBuilder sgfStringBuilder;
        private List<Step> m_StepHistory;

        public PlayingPage(Game game, LocalType localType, int myPlayerID, int[] blackIDs, int[] whiteIDs, int nextPlayerID)
        {
            InitializeComponent();

            DataContext = new PlayingViewModel(game, myPlayerID);
            m_LocalType = localType;

            m_AI = new AI(game.GameSetting.BoardSize);
            m_AI.Init();

            m_StepHistory = new List<Step>();

            //添加事件：服务器、AI、Board
            ServiceProxy.Instance.MoveCallback = MoveCallback;
            m_Board.MousePlayEvent += Board_MousePlayEvent;
            m_AI.OnAIMove += OnAIMove;

            //第一步
            DealNextMove(0, nextPlayerID);
        }


        private void OnAIMove(int x, int y, bool isPass, bool isResign, int color)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                string gameID = VM.Game.GameID;
                int currentStepNum = VM.CurrentStepNum;
                if (isPass)
                {
                    m_Board.Pass();
                    Task.Factory.StartNew(() => { ServiceProxy.Instance.ClientCommitMove(gameID, currentStepNum, -1, -1); });
                }
                else if (isResign)
                {
                    Task.Factory.StartNew(() => { ServiceProxy.Instance.ClientCommitMove(gameID, currentStepNum, -100, -100); });
                }
                else
                {
                    Console.WriteLine("自己下完，处理board和AI");
                    m_Board.Play(x, y);
                    m_AI.Play(x, y, color);
                    Task.Factory.StartNew(() => { ServiceProxy.Instance.ClientCommitMove(gameID, currentStepNum, x, y); });
                }
            }));
        }

        #region 服务器回调
        /// <summary>
        /// 棋步到来
        /// </summary>
        /// <param name="stepNum"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nextPlayerID"></param>
        private void MoveCallback(int stepNum, int lastPlayerID, int x, int y, int nextPlayerID)
        {
            Console.WriteLine("收到服务的Move stepNum=" + stepNum + " nextID=" + nextPlayerID);
            //m_StepNum = stepNum;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Player lastPlayer = VM.Game.Players.First(p => p.ID == lastPlayerID);


                if (x == -100 || y == -100)//resign
                {
                    MessageBoxResult r = MessageBox.Show(MainWindow.Instance, (lastPlayer.Color == 2 ? "黑棋" : "白棋") + "认输了，是否保存棋谱？", "消息", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (r == MessageBoxResult.Yes)
                    {
                        WriteSgf((lastPlayer.Color == 2 ? "黑棋" : "白棋") + "认输])");
                    }
                    return;
                }

                //如果收到的这步正是我刚才走的，就不需要在处理棋盘UI
                if ((m_LocalType == LocalType.Host && lastPlayer.Type == PlayerType.Internet) ||
                  (m_LocalType == LocalType.Client && lastPlayerID != VM.SelfPlayer.ID))
                {
                    if (x == -1 || y == -1)//pass
                    {
                        m_Board.Pass();
                    }
                    else
                    {
                        Console.WriteLine("刚才不是自己下，需要处理board和AI");
                        m_Board.Play(x, y);
                        m_AI.Play(x, y, lastPlayer.Color);
                    }
                }

                Step step = new Step() { StepNum = stepNum, Player = lastPlayer, Position = new Position(x, y), Comment = "落子：" + lastPlayer.Name };
                m_StepHistory.Add(step);
                DealNextMove(stepNum + 1, nextPlayerID);
            }));
        }

        #endregion

        private void DealNextMove(int stepNum, int nextID)
        {
            Console.WriteLine("处理下一步，该 " + VM.Game.Players.First(p => p.ID == nextID).Name + " 走棋");

            //m_StepNum++;
            //UI
            VM.DealNextPlayer(stepNum, nextID);


            if (m_LocalType == LocalType.Host)
            {
                switch (VM.CurrentPlayer.Type)
                {
                    case PlayerType.AI:
                        m_AI.AIThink(VM.CurrentPlayer);
                        break;
                    case PlayerType.RealBoard:
                        break;
                    case PlayerType.Host:
                        m_Board.IsHostTurn = true;
                        VM.SelfPlayer = VM.CurrentPlayer;
                        break;
                    case PlayerType.Internet:
                        break;
                }
            }
            else if (m_LocalType == LocalType.Client)
            {
                switch (VM.CurrentPlayer.Type)
                {
                    case PlayerType.AI:
                        break;
                    case PlayerType.RealBoard:
                        break;
                    case PlayerType.Host:
                        break;
                    case PlayerType.Internet:
                        if (nextID == VM.SelfPlayer.ID)
                        {
                            m_Board.IsHostTurn = true;
                        }
                        break;
                }
            }

        }


        private void Board_MousePlayEvent(int stepNum, int x, int y, int color)
        {
            m_Board.IsHostTurn = false;
            VM.SelfPlayer.Playing = false;
            m_AI.Play(x, y, color);
            string gameID = VM.Game.GameID;
            Task.Factory.StartNew(() => { ServiceProxy.Instance.ClientCommitMove(gameID, stepNum, x, y); });
        }

        public PlayingViewModel VM { get { return ((PlayingViewModel)DataContext); } }


        private void btnPass_Click(object sender, RoutedEventArgs e)
        {
            m_Board.Pass();
            m_Board.IsHostTurn = false;
            VM.SelfPlayer.Playing = false;

            //m_AI.Play(-3, 22, VM.CurrentPlayer.Color);
            //-1,-1表示pass
            string gameID = VM.Game.GameID;
            int currentStepNum = VM.CurrentStepNum;
            Task.Factory.StartNew(() => { ServiceProxy.Instance.ClientCommitMove(gameID, currentStepNum, -1, -1); });
        }

        private void btnResign_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult re = MessageBox.Show("是否认输？", "确认", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (re == MessageBoxResult.Cancel)
            {
                return;
            }

            m_Board.IsHostTurn = false;
            VM.SelfPlayer.Playing = false;
            //m_AI.Play(x, y, color);
            string gameID = VM.Game.GameID;
            int currentStepNum = VM.CurrentStepNum;
            Task.Factory.StartNew(() => { ServiceProxy.Instance.ClientCommitMove(gameID, currentStepNum, -100, -100); });
        }

        /// <summary>
        /// 系统设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSystemSetting_Click(object sender, RoutedEventArgs e)
        {
            SystemSettingDialog d = new SystemSettingDialog();
            d.Owner = MainWindow.Instance;
            d.ShowDialog();
        }

        private void WriteSgf(string lastComment)
        {
            CommonSaveFileDialog saveFileDialog = new CommonSaveFileDialog();
            saveFileDialog.DefaultExtension = "sgf";
            saveFileDialog.DefaultFileName = ServiceProxy.Instance.Session.UserName + DateTime.Now.ToString(" MM-dd HH-mm-ss");
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            //头
            StringBuilder sgfStringBuilder = new StringBuilder();
            sgfStringBuilder.Append("(;PB[");
            for (int i = 0; i < VM.BlackPlayers.Length; i++)
            {
                sgfStringBuilder.Append(VM.BlackPlayers[i].Name);
                if (i != VM.BlackPlayers.Length - 1)
                    sgfStringBuilder.Append("+");
            }
            sgfStringBuilder.Append("]PW[");
            for (int i = 0; i < VM.WhitePlayers.Length; i++)
            {
                sgfStringBuilder.Append(VM.WhitePlayers[i].Name);
                if (i != VM.WhitePlayers.Length - 1)
                    sgfStringBuilder.Append("+");
            }
            sgfStringBuilder.Append("]");
            //记录棋谱
            for (int i = 0; i < m_StepHistory.Count; i++)
            {
                if (m_StepHistory[i].Position.X == -1 || m_StepHistory[i].Position.Y == -1)
                    sgfStringBuilder.Append(";[" + (m_StepHistory[i].Player.Color == 2 ? "B" : "W") + "[]");
                else
                    sgfStringBuilder.Append(";[" + (m_StepHistory[i].Player.Color == 2 ? "B" : "W") + "[" + (char)('a' + m_StepHistory[i].Position.X) + (char)('a' + m_StepHistory[i].Position.Y) + "]");
                sgfStringBuilder.Append("C[落子者：" + m_StepHistory[i].Player.Name + "]");
            }
            //末尾
            sgfStringBuilder.Remove(sgfStringBuilder.Length - 1, 1);
            sgfStringBuilder.Append(lastComment + "]");
            sgfStringBuilder.Append(")");

            //Sgf.WriteSgf(sgfStringBuilder.ToString());


            FileInfo file = new FileInfo(saveFileDialog.FileName);
            if (!Directory.Exists(file.DirectoryName))
            {
                Directory.CreateDirectory(file.DirectoryName);
            }

            if (!System.IO.File.Exists(saveFileDialog.FileName))
            {
                System.IO.FileStream f = System.IO.File.Create(saveFileDialog.FileName);
                f.Close();
            }
            System.IO.StreamWriter f2 = new System.IO.StreamWriter(saveFileDialog.FileName, true, Encoding.UTF8);
            f2.Write(sgfStringBuilder.ToString());
            f2.Close();
            f2.Dispose();
        }
    }
}
