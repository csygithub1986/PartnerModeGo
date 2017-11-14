using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueGoServer.Model
{
    public class Player
    {
        public Player()
        {
            TimePerMove = 2;
            //Layout = 50000;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public PlayerType Type { get; set; }
        /// <summary>
        /// 2黑色，1白色
        /// </summary>
        public int Color { get; set; }

        //是否已经被占用
        public bool Occupied { get; set; }
        /// <summary>
        /// 玩家对应的链接，可以多个玩家对应同一个链接。例如Host本身和RealBoard都是Host的链接
        /// </summary>
        public ClientInfo Client { get; set; }


        #region 网络共同属性
        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnected { get; set; }
        #endregion

        #region AI属性
        /// <summary>
        /// 电脑一步设定的时间，如果未配置则无用，单位s
        /// </summary>
        public int TimePerMove { get; set; }

        /// <summary>
        /// 电脑一步搜索的节点（因为zen.dll没有回调机制，所以这个只是为了预留，目前没有实际作用，这里是超额设定，50000大概会用50秒）
        /// </summary>
        public int Layout { get; set; }

        #endregion

        #region 真实棋盘属性
        /// <summary>
        /// 棋盘被识别的状态
        /// </summary>
        public int RecognizedState { get; set; }
        #endregion

        #region Lan、RealBoard共同属性
        /// <summary>
        /// 对方IP TODO：暂时定为，这个属性，以后会修改
        /// </summary>
        public string Ip { get; set; }
        #endregion
    }
}
