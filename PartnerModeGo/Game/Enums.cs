using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    public enum PlayerType
    {
        /// <summary>
        /// AI
        /// </summary>
        AI,
        /// <summary>
        /// 本机鼠标
        /// </summary>
        Host,
        /// <summary>
        /// 真实棋盘，来自局域网Phone
        /// </summary>
        RealBoard,
        ///// <summary>
        ///// 局域网
        ///// </summary>
        //LAN,
        ///// <summary>
        ///// 互联网
        ///// </summary>
        //Internet
    }
}
