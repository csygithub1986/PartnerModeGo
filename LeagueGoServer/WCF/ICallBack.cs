using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using LeagueGoServer.Model;

namespace LeagueGoServer
{
    [ServiceContract]
    public interface ICallback
    {
        /// <summary>
        /// 发送给空闲用户新增的游戏信息
        /// </summary>
        /// <param name="game"></param>
        [OperationContract(IsOneWay = true)]
        void DistributeNewGame(Game game);

        /// <summary>
        /// 在登录时发送给用户所有游戏信息
        /// </summary>
        /// <param name="game"></param>
        [OperationContract(IsOneWay = true)]
        void DistributeAllGameInfo(Game[] game);

        /// <summary>
        /// 返回给用户加入游戏的结果
        /// </summary>
        /// <param name="success">成功返回true，失败返回false（例如申请的game或player刚刚被占用或失效了）</param>
        /// <param name="game">游戏信息</param>
        [OperationContract(IsOneWay = true)]
        void DistributeApplyGameResult(bool success, Game game);

        /// <summary>
        /// 发送开始游戏
        /// </summary>
        /// <param name="blackPlayerIDs"></param>
        /// <param name="whitePlayerIDs"></param>
        /// <param name="currentPlayerID"></param>
        [OperationContract(IsOneWay = true)]
        void DistributeGameStart(int[] blackPlayerIDs, int[] whitePlayerIDs, int currentPlayerID);

        [OperationContract(IsOneWay = true)]
        void DistributeMove(int stepNum, int x, int y, int nextPlayerID);
    }
}
