using PartnerModeGo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo.WcfService
{
    /// <summary>
    /// 比赛参数设置
    /// </summary>
    public partial class Player
    {
        public event Action<Player, bool> ConnectChanged;
        public event Action<Player, bool> RecognizeChanged;

        public Player()
        {
            TimePerMove = 2;
            Layout = 50000;
        }

        public bool Playing
        {
            get { return _Playing; }
            set
            {
                if (_Playing != value)
                {
                    _Playing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Playing"));
                }
            }
        }
        private bool _Playing;


        //#region 网络共同属性
        ///// <summary>
        ///// 是否连接
        ///// </summary>
        //public bool IsConnected
        //{
        //    get { return _IsConnected; }
        //    set
        //    {
        //        if (_IsConnected != value)
        //        {
        //            _IsConnected = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsConnected"));
        //            ConnectChanged?.Invoke(this, value);
        //        }
        //    }
        //}
        //private bool _IsConnected;
        //#endregion

        //#region AI属性
        ///// <summary>
        ///// 电脑一步设定的时间，如果未配置则无用，单位s
        ///// </summary>
        //public int TimePerMove
        //{
        //    get { return _TimePerMove; }
        //    set
        //    {
        //        if (_TimePerMove != value)
        //        {
        //            _TimePerMove = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TimePerMove"));
        //        }
        //    }
        //}
        //private int _TimePerMove = 2;

        ///// <summary>
        ///// 电脑一步搜索的节点（因为zen.dll没有回调机制，所以这个只是为了预留，目前没有实际作用，这里是超额设定，50000大概会用50秒）
        ///// </summary>
        //public int Layout
        //{
        //    get { return _Layout; }
        //    set
        //    {
        //        if (_Layout != value)
        //        {
        //            _Layout = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Layout"));
        //        }
        //    }
        //}
        //private int _Layout = 50000;

        //#endregion

        //#region 真实棋盘属性
        ///// <summary>
        ///// 是否被识别了
        ///// </summary>
        //public bool IsBoardRecognized
        //{
        //    get { return _IsBoardRecognized; }
        //    set
        //    {
        //        if (_IsBoardRecognized != value)
        //        {
        //            _IsBoardRecognized = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsBoardRecognized"));
        //            RecognizeChanged?.Invoke(this, value);
        //        }
        //    }
        //}
        //private bool _IsBoardRecognized;
        //#endregion

        //#region Lan、RealBoard共同属性
        ///// <summary>
        ///// 对方IP TODO：暂时定为，这个属性，以后会修改
        ///// </summary>
        //public string Ip
        //{
        //    get { return _Ip; }
        //    set
        //    {
        //        if (_Ip != value)
        //        {
        //            _Ip = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ip"));
        //        }
        //    }
        //}
        //private string _Ip;
        //#endregion
    }
}
