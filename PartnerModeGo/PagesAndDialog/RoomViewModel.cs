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
    public class RoomViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region 构造函数和初始化函数

        /// <summary>
        /// Host进入的构造函数。
        /// 最开始各初始化两个Player，等确定好后，发送到服务器，等待回调
        /// </summary>
        public RoomViewModel()
        {
            Players = new ObservableCollection<Player>();
            Players.Add(new Player() { Color = 2, ID = 1, Name = ServiceProxy.Instance.Session.UserName, Type = PlayerType.Host });
            Players.Add(new Player() { Color = 2, ID = 2, Name = "", Type = PlayerType.Internet });
            Players.Add(new Player() { Color = 1, ID = 3, Name = "AI", Type = PlayerType.AI, TimePerMove = 2, Layout = 50000 });
            //Players.Add(new Player() { Color = 1, ID = 4, Name = "AI白", Type = PlayerType.AI, TimePerMove = 2, Layout = 50000 });
            CurrentGame = new Game()
            {
                Players = Players.ToArray(),
                Name = ServiceProxy.Instance.Session.UserName + "的房间",
                GameSetting = new GameSetting() { Name = ServiceProxy.Instance.Session.UserName + "的房间", BoardSize = 19, Handicap = 0, Komi = 6 }
            };
            //LocalType = LocalType.Host;

            AddBlackPlayerCommand = new CommandBase() { CanExecuteAction = CanAddBlackExecute, ExecuteAction = AddBlackExecute };
            DeleteBlackPlayerCommand = new CommandBase() { CanExecuteAction = CanDeleteBlackExecute, ExecuteAction = DeleteBlackExecute };
            AddWhitePlayerCommand = new CommandBase() { CanExecuteAction = CanAddWhiteExecute, ExecuteAction = AddWhiteExecute };
            DeleteWhitePlayerCommand = new CommandBase() { CanExecuteAction = CanDeleteWhiteExecute, ExecuteAction = DeleteWhiteExecute };
        }

        //public RoomViewModel(Game game)
        //{
        //    //Players = new ObservableCollection<Player>(game.Players);
        //    CurrentGame = game;
        //    Players = new ObservableCollection<Player>(game.Players);
        //    LocalType = LocalType.Client;
        //    AddEventHandler();
        //}

        #endregion

        #region 属性
        public Game CurrentGame
        {
            get { return _CurrentGame; }
            set { _CurrentGame = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentGame")); }
        }
        private Game _CurrentGame;


        public ObservableCollection<Player> Players
        {
            get { return _Players; }
            set { _Players = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Players")); }
        }
        private ObservableCollection<Player> _Players;

        //public LocalType LocalType
        //{
        //    get { return _LocalType; }
        //    set { _LocalType = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LocalType")); }
        //}
        //private LocalType _LocalType;


        public Player SelectedPlayer { get; set; }


        #endregion

        #region 命令
        public CommandBase AddBlackPlayerCommand { get; set; }
        public CommandBase DeleteBlackPlayerCommand { get; set; }
        public CommandBase AddWhitePlayerCommand { get; set; }
        public CommandBase DeleteWhitePlayerCommand { get; set; }
        #endregion

        #region Host更改Player设置
        private bool CanAddBlackExecute(object param)
        {
            return Players.Where(p => p.Color == 2).Count() < 3;
        }
        private bool CanDeleteBlackExecute(object arg)
        {
            return (Players.Where(p => p.Color == 2).Count() > 1 && SelectedPlayer != null && SelectedPlayer.Color == 2);
        }
        private bool CanAddWhiteExecute(object arg)
        {
            return Players.Where(p => p.Color == 1).Count() < 3;
        }
        private bool CanDeleteWhiteExecute(object arg)
        {
            return Players.Where(p => p.Color == 1).Count() > 1 && SelectedPlayer != null && SelectedPlayer.Color == 1;
        }

        private void AddBlackExecute(object obj)
        {
            Player player = new Player() { Color = 2, ID = Players.Max(p => p.ID) + 1, Type = PlayerType.Internet };
            Players.Add(player);
            CurrentGame.Players = Players.ToArray();
        }
        private void DeleteBlackExecute(object obj)
        {
            Players.Remove(SelectedPlayer);
            CurrentGame.Players = Players.ToArray();
        }
        private void AddWhiteExecute(object obj)
        {
            Player player = new Player() { Color = 1, ID = Players.Max(p => p.ID) + 1, Type = PlayerType.Internet };
            Players.Add(player);
            CurrentGame.Players = Players.ToArray();
        }

        private void DeleteWhiteExecute(object obj)
        {
            Players.Remove(SelectedPlayer);
            CurrentGame.Players = Players.ToArray();
        }

        #endregion
    }
}
