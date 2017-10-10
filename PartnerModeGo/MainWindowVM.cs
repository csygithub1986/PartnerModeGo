using System;
using System.Collections.Generic;
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
            _PlayerSettings = new PlayerSetting[4];
            string[] headerNames = { "黑1", "白1", "黑2", "白2" };
            int[] colors = { 2, 1, 2, 1 };
            for (int i = 0; i < 4; i++)
            {
                _PlayerSettings[i] = new PlayerSetting() { HeaderName = headerNames[i], Color = colors[i] };
            }
            GameLoopTimes = 1;
        }

        public PlayerSetting[] PlayerSettings
        {
            get { return _PlayerSettings; }
            set
            {
                if (_PlayerSettings != value)
                {
                    _PlayerSettings = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PlayerSettings"));
                }
            }
        }
        private PlayerSetting[] _PlayerSettings;

        /// <summary>
        /// 用于棋力自测或者和其他软件对测时，测试的盘数。与人下请设定为1
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
        private int _GameLoopTimes;

    }
}
