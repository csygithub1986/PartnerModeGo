using System.Collections.Generic;

namespace PartnerModeGo
{
    /// <summary>
    /// 规则接口
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// 判断是否可以落子，并计算落子后被提子坐标
        /// </summary>
        /// <param name="state">棋盘状态数组，1表示黑，-1表示白，0表示空</param>
        /// <param name="color">当前行棋方的颜色</param>
        /// <param name="selectPosition">当前尝试落子坐标</param>
        /// <param name="lastMove">上一次落子的坐标，如果为null表示当前为第一步棋或对方刚才弃着</param>
        /// <param name="lastEatCount">上一次落字吃了几颗子</param>
        /// <returns>3中情况：不能落子返回null，能吃子返回被吃子的坐标集，不能吃子返回空集合</returns>
        List<Position> GetMoveResult(BoardState state, Position selectPosition);

        /// <summary>
        /// 计算胜负（中国规则）
        /// </summary>
        /// <param name="state">棋盘状态</param>
        /// <param name="compensation">贴目</param>
        /// <returns>返回黑棋赢的子数，负数表示白剩</returns>
        float CalculateScore(int[,] state, float compensation);

        /// <summary>
        /// 计算胜负（日韩规则）
        /// </summary>
        /// <param name="state">棋盘状态</param>
        /// <param name="compensation">贴目</param>
        /// <param name="blackDead">黑棋被提的子数</param>
        /// <param name="whiteDead">白棋被提的子数</param>
        /// <returns></returns>
        float CalculateScore(int[,] state, float compensation, int blackDead, int whiteDead);

        //返回相连区域
        List<Position> GetConnectedPieces(int[,] state, Position p);

        //返回气区域
        List<Position> GetBreathArea(int[,] state, Position p);
    }

    ////棋盘状态类
    //public class BoardState
    //{
    //    public int[,] State { get; set; }
    //    public int Turn { get; set; }
    //    public Position LastMove { get; set; }
    //    public int LastEatCount { get; set; }
    //    public int Size { get; set; }

    //    public BoardState(int size)
    //    {
    //        Size = size;
    //        State = new int[size, size];
    //        Turn = 1;
    //    }
    //}


    ////坐标类
    //public class Position
    //{
    //    public int X;
    //    public int Y;

    //    public Position() { }

    //    public Position(int x, int y)
    //    {
    //        X = x;
    //        Y = y;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return X + Y * 19;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj == null)
    //        {
    //            return false;
    //        }
    //        Position p = obj as Position;
    //        if (p.X == X && p.Y == Y)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    //public static bool operator ==(Position a, Position b)
    //    //{
    //    //    if (a == null || b == null)
    //    //    {
    //    //        return false;
    //    //    }
    //    //    return a.Equals(b);
    //    //}


    //    //public static bool operator !=(Position a, Position b)
    //    //{
    //    //    if (a == null || b == null)
    //    //    {
    //    //        return true;
    //    //    }
    //    //    return !a.Equals(b);
    //    //}
    //}
}
