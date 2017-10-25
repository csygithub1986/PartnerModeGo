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
        Waiting,Playing
    }
}
