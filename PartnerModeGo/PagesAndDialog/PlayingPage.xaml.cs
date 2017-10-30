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
    /// PlayingPage.xaml 的交互逻辑
    /// </summary>
    public partial class PlayingPage : UserControl
    {
        private Game m_Game;
        private LocalType m_LocalType;
        private int m_MyPlayerID;//当是client的时候用
        private AI m_AI;
        private int m_StepNum;

        public PlayingPage(Game game, LocalType localType, int myPlayerID, int[] blackIDs, int[] whiteIDs, int nextPlayerID)
        {
            InitializeComponent();

            m_Game = game;
            m_MyPlayerID = myPlayerID;
            m_LocalType = localType;

            m_AI = new AI(game.GameSetting.BoardSize);
            m_AI.Init();

            //添加事件：服务器、AI、Board
            ServiceProxy.Instance.MoveCallback = MoveCallback;
            m_Board.MousePlayEvent += Board_MousePlayEvent;
            m_AI.OnAIMove += OnAIMove;

            //日志
            ClientLog.FilePath = "D:/" + ServiceProxy.Instance.Session.UserName + DateTime.Now.ToString("MM-dd HH-mm-ss") + ".sgf";
            ClientLog.WriteLog("(;PB[xyz]PW[abc]");

            DealNextMove(nextPlayerID);
        }


        private void OnAIMove(int x, int y, bool isPass, bool isResign)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                m_Board.Play(x, y);
            }));
            Task.Factory.StartNew(() => { ServiceProxy.Instance.ClientCommitMove(m_StepNum, x, y); });
        }

        #region 服务器回调
        /// <summary>
        /// 棋步到来
        /// </summary>
        /// <param name="stepNum"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nextPlayerID"></param>
        private void MoveCallback(int stepNum, int currentPlayerID, int x, int y, int nextPlayerID)
        {
            m_StepNum = stepNum;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //如果收到的这步正是我刚才走的，就不需要在处理棋盘UI
                Player currentPlayer = m_Game.Players.First(p => p.ID == currentPlayerID);
                if ((m_LocalType == LocalType.Host && currentPlayer.Type == PlayerType.Internet) ||
                  (m_LocalType == LocalType.Client && currentPlayerID != m_MyPlayerID))
                {
                    m_Board.Play(x, y);
                    m_AI.Play(x, y, currentPlayer.Color);
                }

                DealNextMove(nextPlayerID);
            }));
        }

        #endregion

        private void DealNextMove(int nextID)
        {
            m_StepNum++;
            //UI

            Player nextPlayer = m_Game.Players.First(p => p.ID == nextID);
            if (m_LocalType == LocalType.Host)
            {
                switch (nextPlayer.Type)
                {
                    case PlayerType.AI:
                        m_AI.AIThink(nextPlayer);
                        break;
                    case PlayerType.RealBoard:
                        break;
                    case PlayerType.Host:
                        m_Board.IsHostTurn = true;
                        break;
                    case PlayerType.Internet:
                        break;
                }
            }
            else if (m_LocalType == LocalType.Client)
            {
                switch (nextPlayer.Type)
                {
                    case PlayerType.AI:
                        break;
                    case PlayerType.RealBoard:
                        break;
                    case PlayerType.Host:
                        break;
                    case PlayerType.Internet:
                        if (nextID == m_MyPlayerID)
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
            m_AI.Play(x, y, color);
            Task.Factory.StartNew(() => { ServiceProxy.Instance.ClientCommitMove(stepNum, x, y); });
        }

        public PlayingViewModel ViewModel { get { return ((PlayingViewModel)DataContext); } }

    }
}
