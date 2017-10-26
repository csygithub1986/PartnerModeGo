﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartnerModeGo.WcfService;

namespace PartnerModeGo
{
    public class ServiceCallback : WcfService.IWcfServiceCallback
    {




        public void DistributeAllGameInfo(WcfService.Game[] games)
        {
            Console.WriteLine("收到AllGameInfo");
            ServiceProxy.Instance.GlobalData.GameList = new System.Collections.ObjectModel.ObservableCollection<Game>(games);
            //Common.GameList = games.ToList();
        }

        public void DistributeApplyGameResult(bool success, WcfService.Game game)
        {
            Console.WriteLine("收到ApplyGameResult  " + success);

            ServiceProxy.Instance.JoinGameCallback?.Invoke(success, game);
        }

        public void DistributeGameStart(int[] blackPlayerIDs, int[] whitePlayerIDs, int currentPlayerID)
        {
            Console.WriteLine("收到GameStart");
            ServiceProxy.Instance.GameStartCallback?.Invoke(blackPlayerIDs, whitePlayerIDs, currentPlayerID);
        }

        public void DistributeMove(int stepNum, int x, int y, int nextPlayerID)
        {
            Console.WriteLine("收到Move");
            //Common.StepNum++;
        }

        public void DistributeNewGame(WcfService.Game game)
        {
            //Common.GameList.Add(game);
            Console.WriteLine("收到NewGame");
        }

    }
}
