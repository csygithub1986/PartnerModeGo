using PartnerModeGo.Game;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo.Game
{
    /// <summary>
    /// 比赛参数设置
    /// </summary>
    public class Player : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Player()
        {
        }

        /// <summary>
        /// 玩家类型
        /// </summary>
        public PlayerType Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Type"));
                }
            }
        }
        private PlayerType _Type;

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

        #region AI属性
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
        private int _TimePerMove = 2;

        /// <summary>
        /// 电脑一步搜索的节点（因为zen.dll没有回调机制，所以这个只是为了预留，目前没有实际作用，这里是超额设定，50000大概会用50秒）
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
        private int _Layout = 50000;
        #endregion

        #region Lan、RealBoard共同属性
        /// <summary>
        /// 对方IP TODO：暂时定为，这个属性，以后会修改
        /// </summary>
        public string Ip
        {
            get { return _Ip; }
            set
            {
                if (_Ip != value)
                {
                    _Ip = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ip"));
                }
            }
        }
        private string _Ip;
        #endregion
    }
}
