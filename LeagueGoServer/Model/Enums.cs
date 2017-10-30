using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueGoServer.Model
{
    public enum PlayerType
    {
        AI, RealBoard, Host, Internet
    }

    /// <summary>
    /// Client的状态
    /// </summary>
    public enum ClientState
    {
        Idel, Waiting, Playing
    }

    public enum GameState
    {
        Waiting, Playing
    }

    /// <summary>
    /// 往客户端发送游戏信息类型
    /// </summary>
    public enum GameDistributeType
    {
        Add, Update, Delete
    }
}
