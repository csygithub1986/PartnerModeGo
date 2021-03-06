﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Collections;
using System.ServiceModel.Channels;

using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using LeagueGoServer.Model;
using System.Threading.Tasks;

namespace LeagueGoServer
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class WcfService : IWcfService
    {
        /// <summary>
        /// 客户端登录
        /// 登录以后，给客户端发送所有游戏信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string Login(string userName)
        {
            try
            {
                string ssid = OperationContext.Current.SessionId;
                //获取传进的消息属性  
                MessageProperties properties = OperationContext.Current.IncomingMessageProperties;
                //获取消息发送的远程终结点IP和端口  
                RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                string ip = endpoint.Address;
                int port = endpoint.Port;
                //Console.WriteLine(userName + " " + ssid + " " + ip + " " + port);

                //将此用户加入集合
                ICallback callBack = OperationContext.Current.GetCallbackChannel<ICallback>();
                //添加客户端信息到客户端集合
                ClientInfo currentClient = new ClientInfo();
                currentClient.SessionID = ssid;
                currentClient.UserName = userName;
                currentClient.ClientCallback = callBack;
                currentClient.ClientChannel = OperationContext.Current.Channel;
                currentClient.HeartbeatTime = DateTime.Now;
                GlobalData.ClientListAdd(ssid, currentClient);
                OperationContext.Current.Channel.Closing += new EventHandler(ClientChannel_Closing);
                OperationContext.Current.Channel.Faulted += Channel_Faulted;
                return ssid;
            }
            catch (Exception)
            {
                return null;
            }


            //发送所有Game
            //currentClient.ClientCallback.DistributeAllGameInfo(Common.GameList.Values.ToArray());
        }

        private void Channel_Faulted(object sender, EventArgs e)
        {
            //Console.WriteLine("服务端：     Channel_Faulted " + DateTime.Now.ToString("mm-ss-fff"));
            ClientInfo info = GetClientInfo((ICallback)sender);
            if (info == null)
                return;
            GlobalData.ClientListDelete(info.SessionID);
        }

        /// <summary>
        /// 请求所有游戏列表
        /// </summary>
        public void GetAllGames()
        {
            //Console.WriteLine("服务器：client请求getallgame " + DateTime.Now.ToString("mm-ss-fff"));
            string sessionID = OperationContext.Current.SessionId;
            ClientInfo client = GlobalData.ClientListGet(sessionID);
            //Console.WriteLine("服务器：发送给client  allgame " + DateTime.Now.ToString("mm-ss-fff"));
            client.ClientCallback.DistributeAllGameInfo(GlobalData.GameList.Values.ToArray());
            //Console.WriteLine("服务器：发送给client  allgame完成 " + DateTime.Now.ToString("mm-ss-fff"));
        }

        /// <summary>
        /// 新建游戏
        /// </summary>
        /// <param name="players"></param>
        /// <param name="gameSetting"></param>
        public void CreateGame(Player[] players, GameSetting gameSetting)
        {
            //收到请求后，1、如果Host或Realboard在玩家列表里，添加Client信息。
            //2、添加游戏列表
            //3、给所有client发送消息
            string sessionID = OperationContext.Current.SessionId;
            ClientInfo client = GlobalData.ClientListGet(sessionID);
            foreach (Player player in players)
            {
                //除了Internet，其他三个，AI、Realboard、Host的连接都是Host本身的连接
                if (player.Type != PlayerType.Internet)
                {
                    player.Client = client;
                }
            }

            Game game = new Game() { Name = gameSetting.Name, GameID = sessionID, Players = players, GameSetting = gameSetting };
            game.Init();
            GlobalData.GameList.TryAdd(sessionID, game);

            foreach (ClientInfo c in GlobalData.ClientList.Values)
            {
                //if (c.PlayingState == ClientState.Idel)
                //{
                Task.Factory.StartNew(() =>
                 {
                     ICallback callback = c.ClientCallback;
                     callback.DistributeGameInfo(GameDistributeType.Add, game);
                 });
                //}
            }
        }

        /// <summary>
        /// 用户申请加入游戏
        /// 成功后调用callback给所有用户推送结果
        /// </summary>
        /// <param name="gameID"></param>
        /// <param name="playerID"></param>
        /// <returns>成功返回true，失败返回false（例如申请的game或player刚刚被占用或失效了）</returns>
        public void ApplyToJoinGame(string gameID, int playerID)
        {
            string sessionID = OperationContext.Current.SessionId;
            ClientInfo currentClient = GlobalData.ClientListGet(sessionID);
            if (GlobalData.GameList.ContainsKey(gameID))
            {
                lock (GlobalData.GameList[gameID])//???????????????????想要锁住Game对象的状态，是否可以这么用？
                {
                    if (GlobalData.GameList[gameID].State == GameState.Waiting)
                    {
                        Player player = GlobalData.GameList[gameID].Players.FirstOrDefault(p => p.ID == playerID);
                        if (player != null && player.Occupied == false)//后者判断此player位置还未被占用
                        {
                            player.Client = currentClient;
                            player.Occupied = true;
                            player.Name = currentClient.UserName;
                            currentClient.ClientCallback.DistributeApplyGameResult(true, gameID, player.ID);
                            foreach (var client in GlobalData.ClientList.Values)
                            {
                                client.ClientCallback.DistributeGameInfo(GameDistributeType.Update, GlobalData.GameList[gameID]);
                            }
                            return;
                        }
                    }
                }
            }
            //如果没有成功，只给申请的client发送
            currentClient.ClientCallback.DistributeApplyGameResult(false, null, 0);
        }

        /// <summary>
        /// Host发送开始命令，转发给game中的player，并启动游戏
        /// </summary>
        public void GameStart()
        {
            string sessionID = OperationContext.Current.SessionId;
            ClientInfo currentClient = GlobalData.ClientListGet(sessionID);
            Game game = GlobalData.GameList[sessionID];
            game.NextPlayer = game.GetBlackPlayers()[0];
            //Host和其他的internet玩家分开发送
            foreach (var player in GlobalData.GameList[sessionID].Players)
            {
                if (player.Client != null && player.Client != currentClient)
                {
                    player.Client.ClientCallback.DistributeGameStart(game.GetBlackPlayers().Select(p => p.ID).ToArray(), game.GetWhitePlayers().Select(p => p.ID).ToArray(), game.NextPlayer.ID);
                }
            }
            currentClient.ClientCallback.DistributeGameStart(game.GetBlackPlayers().Select(p => p.ID).ToArray(), game.GetWhitePlayers().Select(p => p.ID).ToArray(), game.NextPlayer.ID);
            //启动游戏
            game.PrepareNextMove();
        }

        /// <summary>
        /// 收到客户端上传的Move
        /// </summary>
        /// <param name="stepNum">第几步，从0开始</param>
        /// <param name="x">坐标</param>
        /// <param name="y">坐标</param>
        public void ClientCommitMove(string gameID, int stepNum, int x, int y)
        {
            //Console.WriteLine("         服务端：收到move");
            string sessionID = OperationContext.Current.SessionId;
            ClientInfo currentClient = GlobalData.ClientListGet(sessionID);
            Game game = GlobalData.GameList[gameID];
            if (game.NextPlayer.Client != currentClient || game.StepNum != stepNum)
            {
                //不是应该提交的client提交了数据，错误。 TODO:记录日志
                //Console.WriteLine("         服务端：不是应该提交的client提交了数据，错误。 TODO:记录日志");

                return;
            }

            game.DealArrivedMove(x, y);
            game.PrepareNextMove();

            //发送Move相应给所有人，Host单独发。（当前客户端走棋已经落子，但不能落下一子）
            Player host = game.Players.First(p => p.Client.SessionID == gameID);
            //Console.WriteLine("         服务端：发送move给client");

            host.Client.ClientCallback.DistributeMove(stepNum, game.LastPlayer.ID, x, y, game.NextPlayer.ID);
            foreach (var player in game.Players)
            {
                if (player.Client != null && player.Client.SessionID != game.GameID)
                {
                    player.Client.ClientCallback.DistributeMove(stepNum, game.LastPlayer.ID, x, y, game.NextPlayer.ID);
                }
            }
        }

        #region 私有方法
        private void ClientChannel_Closing(object sender, EventArgs e)
        {
            //Console.WriteLine("服务端：     ClientChannel_Closing " + DateTime.Now.ToString("mm-ss-fff"));
            ClientInfo info = GetClientInfo((ICallback)sender);
            if (info == null)
                return;
            GlobalData.ClientListDelete(info.SessionID);
        }

        /// <summary>
        /// 查询某个回调对应的用户
        /// </summary>
        private ClientInfo GetClientInfo(ICallback callBack)
        {
            foreach (var info in GlobalData.ClientList)
            {
                if (info.Value.ClientCallback == callBack)
                    return info.Value;
            }
            return null;
        }
        #endregion
    }
}
