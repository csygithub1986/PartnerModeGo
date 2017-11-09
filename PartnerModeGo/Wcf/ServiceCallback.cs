using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartnerModeGo.WcfService;
using System.Threading;

namespace PartnerModeGo
{
    public class ServiceCallback : WcfService.IWcfServiceCallback
    {
        public void DistributeAllGameInfo(WcfService.Game[] games)
        {
            //Console.WriteLine("客户端：收到AllGameInfo " + DateTime.Now.ToString("mm-ss-fff"));
            ServiceProxy.Instance.Session.GameList = new System.Collections.ObjectModel.ObservableCollection<Game>(games);
            //Thread.Sleep(2000);
            //Console.WriteLine("客户端：收到AllGameInfo处理完毕 " + DateTime.Now.ToString("mm-ss-fff"));
        }

        public void DistributeApplyGameResult(bool success, string gameID, int playerID)
        {
            //Console.WriteLine("收到ApplyGameResult  " + success);

            ServiceProxy.Instance.JoinGameCallback?.Invoke(success, gameID, playerID);
        }

        /// <summary>
        /// 游戏新增、更改和退出的通知
        /// </summary>
        /// <param name="type"></param>
        /// <param name="game"></param>
        public void DistributeGameInfo(GameDistributeType type, Game game)
        {
            //Console.WriteLine("收到DistributeGameInfo");
            ServiceProxy.Instance.DistributeGameInfoCallback?.Invoke(type, game);
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                switch (type)
                {
                    case GameDistributeType.Add:
                        ServiceProxy.Instance.Session.GameList.Add(game);
                        break;
                    case GameDistributeType.Update:
                        ServiceProxy.Instance.Session.GameList.Remove(ServiceProxy.Instance.Session.GameList.FirstOrDefault(p => p.GameID == game.GameID));
                        ServiceProxy.Instance.Session.GameList.Add(game);
                        break;
                    case GameDistributeType.Delete:
                        ServiceProxy.Instance.Session.GameList.Remove(ServiceProxy.Instance.Session.GameList.FirstOrDefault(p => p.GameID == game.GameID));
                        break;
                }
            });
        }

        public void DistributeGameStart(int[] blackPlayerIDs, int[] whitePlayerIDs, int currentPlayerID)
        {
            //Console.WriteLine("收到GameStart");
            ServiceProxy.Instance.GameStartCallback?.Invoke(blackPlayerIDs, whitePlayerIDs, currentPlayerID);
        }

        public void DistributeMove(int stepNum, int currentPlayerID, int x, int y, int nextPlayerID)
        {
            //Console.WriteLine("收到Move");
            ServiceProxy.Instance.MoveCallback?.Invoke(stepNum, currentPlayerID, x, y, nextPlayerID);
            //Common.StepNum++;
        }


    }
}
