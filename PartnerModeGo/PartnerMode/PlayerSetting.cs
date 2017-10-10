using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    /// <summary>
    /// 比赛参数设置
    /// </summary>
    public class PlayerSetting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public PlayerSetting()
        {
            TimePerMove = 2;
            Layout = 50000;
            IsZen = true;
        }

        /// <summary>
        /// 2：黑色，1：白色
        /// </summary>
        public int Color
        {
            get { return _Color; }
            set
            {
                if (_Color != value)
                {
                    _Color = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Color"));
                }
            }
        }
        private int _Color;

        /// <summary>
        /// 本玩家是否启用Zen引擎
        /// </summary>
        public bool IsZen
        {
            get { return _IsZen; }
            set
            {
                if (_IsZen != value)
                {
                    _IsZen = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsZen"));
                }
            }
        }
        private bool _IsZen;


        /// <summary>
        /// 电脑一步设定的时间，如果未配置则无用，单位s
        /// </summary>
        public int TimePerMove
        {
            get { return _TimePerMove; }
            set
            {
                if (_TimePerMove != value)
                {
                    _TimePerMove = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TimePerMove"));
                }
            }
        }
        private int _TimePerMove;

        /// <summary>
        /// 电脑一步搜索的节点（因为zen.dll没有回调机制，所以这个只是为了预留，目前没有实际作用）
        /// </summary>
        public int Layout
        {
            get { return _Layout; }
            set
            {
                if (_Layout != value)
                {
                    _Layout = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Layout"));
                }
            }
        }
        private int _Layout;

        /// <summary>
        /// 用于界面显示分组名称
        /// </summary>
        public string HeaderName
        {
            get { return _HeaderName; }
            set
            {
                if (_HeaderName != value)
                {
                    _HeaderName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HeaderName"));
                }
            }
        }
        private string _HeaderName;

        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName
        {
            get { return _PlayerName; }
            set
            {
                if (_PlayerName != value)
                {
                    _PlayerName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PlayerName"));
                }
            }
        }
        private string _PlayerName;
    }
}
