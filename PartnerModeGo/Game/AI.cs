using PartnerModeGo.WcfService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    public class AI
    {
        private int m_BoardSize;

        public event Action<int, int, bool, bool> OnAIMove;
        public event Action<float> OnWinRate;
        public event Action<int[]> OnTerritoryAnalized;


        public AI(int boardSize) { m_BoardSize = boardSize; }

        public void Init()
        {
            DllImport.Initialize(DateTime.Now.Ticks + ".aitemp");
            DllImport.SetBoardSize(m_BoardSize);
        }

        public void Play(int x, int y, int color)
        {
            DllImport.Play(x, y, color);
        }

        public void AIThink(Player aiPlayer)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                DllImport.SetNumberOfSimulations(aiPlayer.Layout);
                DllImport.StartThinking(aiPlayer.Color);
                Thread.Sleep(aiPlayer.TimePerMove * 1000);
                DllImport.StopThinking();
                //Thread.Sleep(500);
                int x = 0, y = 0;
                bool isPass = false, isResign = false;
                DllImport.ReadGeneratedMove(ref x, ref y, ref isPass, ref isResign);
                OnAIMove?.Invoke(x, y, isPass, isResign);

                int count = 0;
                float winRate = 0;
                DllImport.GetTopMoveInfo(0, ref x, ref y, ref count, ref winRate, null, 0);
                OnWinRate?.Invoke(winRate);

                int[] territoryStatictics = new int[m_BoardSize * m_BoardSize];
                DllImport.GetTerritoryStatictics(territoryStatictics);
                OnTerritoryAnalized?.Invoke(territoryStatictics);
            });
        }

    }
}
