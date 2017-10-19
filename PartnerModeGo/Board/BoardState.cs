using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    //棋盘状态类
    public class BoardState
    {
        public int[,] State { get; set; }

        /// <summary>
        /// 2黑，1白
        /// </summary>
        public int Turn { get; set; }
        /// <summary>
        /// 最后一步的StepNum，没下时是-1
        /// </summary>
        public int LastSetpNum;
        public Position LastMove { get; set; }
        public int LastEatCount { get; set; }
        public int Size { get; set; }

        public BoardState(int size)
        {
            Size = size;
            State = new int[size, size];
            Turn = 2;
            LastSetpNum = -1;
        }

    }

    //坐标类
    public class Position
    {
        public int X;
        public int Y;

        public Position() { }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
