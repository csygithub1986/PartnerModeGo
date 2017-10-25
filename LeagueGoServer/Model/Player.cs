using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueGoServer.Model
{
    public class Player
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public PlayerType Type { get; set; }
        /// <summary>
        /// 2黑色，1白色
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// 玩家对应的链接，可以多个玩家对应同一个链接。例如Host本身和RealBoard都是Host的链接
        /// </summary>
        public ClientInfo Client { get; set; }
    }
}
