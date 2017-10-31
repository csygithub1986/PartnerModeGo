using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using System.IO;
using LeagueGoServer.Model;
using System.ServiceModel;

namespace LeagueGoServer
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract(CallbackContract = typeof(ICallback), SessionMode = SessionMode.Required)]
    public interface IWcfService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [OperationContract]
        string Login(string userName);

        /// <summary>
        /// 请求所有列表
        /// </summary>
        [OperationContract]
        void GetAllGames();

        [OperationContract]
        void CreateGame(Player[] players, GameSetting gameSettign);


        [OperationContract]
        void ApplyToJoinGame(string gameID, int playerID);

        /// <summary>
        /// Host发送开始游戏
        /// </summary>
        [OperationContract]
        void GameStart();

        /// <summary>
        /// client提交Move
        /// </summary>
        /// <param name="stepNum"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [OperationContract]
        void ClientCommitMove(string gameID,int stepNum, int x, int y);
    }
}
