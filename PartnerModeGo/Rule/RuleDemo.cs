using System.Collections.Generic;

namespace PartnerModeGo
{
    class RuleDemo : IRule
    {
        public List<Position> GetMoveResult(BoardState state, Position selectPosition)
        {
            //为了不影响原状态，复制一个棋盘状态
            int size = state.State.GetUpperBound(0) + 1;
            int[,] copyState = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    copyState[i, j] = state.State[i,j];
                }
            }
            //改变刚落子点的状态
            copyState[selectPosition.X, selectPosition.Y] = state.Turn;

            //吃子列表
            List<Position> eat = new List<Position>();

            int[,] history = new int[size, size];//历史表，搜索过的点都加入此表，今后不再搜索

            /*判断落子点的上下左右四个方向上是否有对方的子，如果有，
             * 再判断此子所在棋块是否还有气，如果没有气，则加入吃子列表 */
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i + j != 1 && i + j != -1)
                    {
                        continue;// 排除斜向
                    }
                    int x = selectPosition.X + i;
                    int y = selectPosition.Y + j;
                    if (x >= 0 && x < size && y >= 0 && y < size)//未超出棋盘边界
                    {
                        if (copyState[x, y] ==3 -state.Turn && history[x, y] == 0)//对方子、且不再历史表
                        {
                            List<Position> list = GetConnectedPieces(copyState, new Position(x, y));//获得棋块
                            foreach (var item in list)
                            {
                                history[item.X, item.Y] = 1;//加入历史表
                            }
                            if (GetBreathArea(copyState, new Position(x, y)).Count == 0)//气为0
                            {
                                eat.AddRange(list);
                            }
                        }
                    }
                }
            }

            if (eat.Count == 0 && GetBreathArea(copyState, selectPosition).Count == 0)
            {
                return null;//不吃子且自身无气，则禁入
            }
            if (eat.Count == 1 && state.LastEatCount == 1 && eat[0].Equals(state.LastMove))
            {
                return null;//打劫，禁入
            }
            return eat;
        }

        public float CalculateScore(int[,] state, float compensation)
        {
            return -3.75f;
        }

        public float CalculateScore(int[,] state, float compensation, int blackDead, int whiteDead)
        {
            return 0;
        }

        /// <summary>
        /// 查找和输入点相连的棋块坐标集合
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="p">搜索起始点</param>
        /// <returns></returns>
        public List<Position> GetConnectedPieces(int[,] state, Position p)
        {
            List<Position> list = new List<Position>();
            int size = state.GetUpperBound(0) + 1;//棋盘大小
            int[,] history = new int[size, size];//1表示搜索过，0表示未搜索
            int color = state[p.X, p.Y];
            SearchPoint(state, p, list, history);//把list传入该方法，递归搜索
            return list;
        }

        //递归搜索
        private void SearchPoint(int[,] state, Position p, List<Position> list, int[,] history)
        {
            history[p.X, p.Y] = 1;
            int size = state.GetUpperBound(0) + 1;
            int color = state[p.X, p.Y];
            if (state[p.X, p.Y] == color)
            {
                list.Add(p);
            }
            //上下左右四个方向，如果和起始点同色，则加入列表
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i + j != 1 && i + j != -1)
                    {
                        continue;
                    }
                    int x = p.X + i;
                    int y = p.Y + j;
                    if (x >= 0 && x < size && y >= 0 && y < size)
                    {
                        if (state[x, y] == color && history[x, y] == 0)
                        {
                            SearchPoint(state, new Position(x, y), list, history);
                        }
                    }
                }
            }
        }

        //求气的数量
        public List<Position> GetBreathArea(int[,] state, Position p)
        {
            List<Position> aline = GetConnectedPieces(state, p);/*这里求了两次，可以优化*/
            int size = state.GetUpperBound(0) + 1;
            int[,] history = new int[size, size];
            List<Position> breathArea = new List<Position>();
            foreach (var item in aline)
            {
                //上下左右四个方向，如果有气，则加入列表
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i + j != 1 && i + j != -1)
                        {
                            continue;
                        }
                        int x = item.X + i;
                        int y = item.Y + j;
                        if (x >= 0 && x < size && y >= 0 && y < size)
                        {
                            if (state[x, y] == 0 && history[x, y] == 0)
                            {
                                breathArea.Add(new Position(x, y));
                                history[x, y] = 1;
                            }
                        }
                    }
                }
            }
            return breathArea;
        }
    }
}
