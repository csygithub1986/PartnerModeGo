using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueGoServer.Model
{
    public class Game
    {
        /// <summary>
        /// Game的ID，跟新建游戏的Client SessionID一致
        /// </summary>
        public string GameID { get; set; }
        public string Name { get; set; }
        public Player[] Players { get; set; }
        public GameSetting GameSetting { get; set; }
        public GameState State { get; set; }


        public int[] BoardState { get; set; }

        /// <summary>
        /// 准备发送给Client的，这一步的StepNum，收到回复的步数也是这个值
        /// </summary>
        public int StepNum { get; set; }
        /// <summary>
        /// 准备发送给Client的，这一步的PlayerID，收到回复的步数也是这个PlayerID
        /// </summary>
        public Player CurrentPlayer { get; set; }
        public List<MovePoint> MoveHistory { get; set; }

        /// <summary>
        /// 黑棋玩家。游戏开始后，将ID发送给client，以决定玩家的显示和落子顺序
        /// </summary>
        private Player[] m_BlackPlayerIDs;

        /// <summary>
        /// 白棋玩家。游戏开始后，将ID发送给client，以决定玩家的显示和落子顺序
        /// </summary>
        private Player[] m_WhitePlayerIDs;

        public Game()
        {
        }

        public void Init()
        {
            MoveHistory = new List<MovePoint>();
            BoardState = new int[GameSetting.BoardSize * GameSetting.BoardSize];

            m_BlackPlayerIDs = Players.Where(p => p.Color == 2).ToArray();
            m_WhitePlayerIDs = Players.Where(p => p.Color == 1).ToArray();
            StepNum = -1;
        }

        public void DealArrivedMove(int x, int y)
        {
            MoveHistory.Add(new MovePoint() { X = x, Y = y });
        }

        public void PrepareNextMove()
        {
            //准备分配下一步
            StepNum++;
            Player[] players = StepNum % 2 == 0 ? m_BlackPlayerIDs : m_WhitePlayerIDs;//2的倍数是黑棋。TODO：让子的话就不一定了
            CurrentPlayer = players[StepNum / 2 % players.Length];
        }

        public Player[] GetBlackPlayers()
        {
            return m_BlackPlayerIDs;
        }
        public Player[] GetWhitePlayers()
        {
            return m_WhitePlayerIDs;
        }
    }
}
