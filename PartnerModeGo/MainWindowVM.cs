using PartnerModeGo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowVM()
        {
            //默认分别初始2个玩家
            _Players = new ObservableCollection<Player>();
            for (int i = 0; i < 2; i++)
            {
                _Players.Add(new Player() { Type = PlayerType.AI, Color = 2 });
                _Players.Add(new Player() { Type = PlayerType.AI, Color = 1 });
            }
            GameLoopTimes = 1;
        }


        public ObservableCollection<Player> Players
        {
            get { return _Players; }
            set
            {
                if (_Players != value)
                {
                    _Players = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Players"));
                }
            }
        }
        private ObservableCollection<Player> _Players;

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

    }
}
