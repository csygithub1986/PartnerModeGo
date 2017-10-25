using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueGoServer.Model
{
    public class GameSetting
    {
        public string Name { get; set; }
        public int BoardSize { get; set; }//棋盘大小
        public int Handicap { get; set; }//让子
        public int Komi { get; set; }//贴目
        public int TimeSetting { get; set; }
    }
}
