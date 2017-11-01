using PartnerModeGo.WcfService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    public class PlayingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public PlayingViewModel(Game game, int selfPlayerID)
        {
            Game = game;
            BlackPlayers = game.Players.Where(p => p.Color == 2).ToArray();
            WhitePlayers = game.Players.Where(p => p.Color == 1).ToArray();
            SelfPlayer = game.Players.First(p => p.ID == selfPlayerID);
            //默认分别初始2个玩家
            //_Players = new ObservableCollection<Player>();
            //for (int i = 0; i < 2; i++)
            //{
            //    _Players.Add(new Player() { Type = PlayerType.AI, Color = 2 });
            //    _Players.Add(new Player() { Type = PlayerType.AI, Color = 1 });
            //}
        }

        #region 属性

        #endregion
        public Game Game
        {
            get { return _Game; }
            set
            {
                if (_Game != value) { _Game = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Game")); }
            }
        }
        private Game _Game;

        public Player[] BlackPlayers
        {
            get { return _BlackPlayers; }
            set
            {
                if (_BlackPlayers != value)
                {
                    _BlackPlayers = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BlackPlayers"));
                }
            }
        }
        private Player[] _BlackPlayers;

        public Player[] WhitePlayers
        {
            get { return _WhitePlayers; }
            set
            {
                if (_WhitePlayers != value)
                {
                    _WhitePlayers = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WhitePlayers"));
                }
            }
        }
        private Player[] _WhitePlayers;

        public Player SelfPlayer
        {
            get { return _SelfPlayer; }
            set
            {
                if (_SelfPlayer != value)
                {
                    _SelfPlayer = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelfPlayer"));
                }
            }
        }
        private Player _SelfPlayer;

        public Player CurrentPlayer
        {
            get { return _CurrentPlayer; }
            set
            {
                if (_CurrentPlayer != value)
                {
                    _CurrentPlayer = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPlayer"));
                }
            }
        }
        private Player _CurrentPlayer;

        public int CurrentStepNum
        {
            get { return _CurrentStepNum; }
            set
            {
                if (_CurrentStepNum != value)
                {
                    _CurrentStepNum = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentStepNum"));
                }
            }
        }
        private int _CurrentStepNum;




        //public ObservableCollection<Player> Players
        //{
        //    get { return _Players; }
        //    set { if (_Players != value) { _Players = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Players")); } }
        //}
        //private ObservableCollection<Player> _Players;

        /// <summary>
        /// 用于棋力自测或者和其他软件对测时，测试的盘数。（测试用，界面不显示）
        /// </summary>
        public int GameLoopTimes
        {
            get { return _GameLoopTimes; }
            set
            {
                if (_GameLoopTimes != value)
                {
                    _GameLoopTimes = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GameLoopTimes"));
                }
            }
        }
        private int _GameLoopTimes = 1;//(测试修改这里)


        public void DealNextPlayer(int stepNum,int nextPlayerID)
        {
            CurrentStepNum = stepNum;
            foreach (var item in Game.Players)
            {
                if (item.ID == nextPlayerID)
                {
                    item.Playing = true;
                    CurrentPlayer = item;
                }
                else
                {
                    item.Playing = false;
                }
            }
        }
    }
}
