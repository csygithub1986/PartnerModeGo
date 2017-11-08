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

        public event Action<int, int, bool, bool, int> OnAIMove;
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
                try
                {
                    DllImport.SetNumberOfSimulations(aiPlayer.Layout); Console.WriteLine("设置ai layout " + aiPlayer.Layout);
                    DllImport.StartThinking(aiPlayer.Color); Console.WriteLine("ai StartThinking " + aiPlayer.Color);
                    Thread.Sleep(aiPlayer.TimePerMove * 1000); Console.WriteLine("ai sleep " + aiPlayer.TimePerMove);
                    DllImport.StopThinking(); Console.WriteLine("ai stopThink " + aiPlayer.Color);
                    //Thread.Sleep(500);
                    int x = 0, y = 0;
                    bool isPass = false, isResign = false;
                    Console.WriteLine("ReadGeneratedMove " + DateTime.Now.ToString("ss-fff"));
                    DllImport.ReadGeneratedMove(ref x, ref y, ref isPass, ref isResign); Console.WriteLine("ai readMove " + x + "," + y + "  " + DateTime.Now.ToString("ss-fff"));

                    int count = 0;
                    float winRate = 0;
                    Console.WriteLine("GetTopMoveInfo " + DateTime.Now.ToString("ss-fff"));
                    DllImport.GetTopMoveInfo(0, ref x, ref y, ref count, ref winRate, null, 0); Console.WriteLine("ai winRate " + winRate + "  " + DateTime.Now.ToString("ss-fff"));


                    int[] territoryStatictics = new int[m_BoardSize * m_BoardSize]; Console.WriteLine("ai territoryStatictics " + DateTime.Now.ToString("ss-fff"));
                    DllImport.GetTerritoryStatictics(territoryStatictics); Console.WriteLine("ai territory成功 " + DateTime.Now.ToString("ss-fff"));


                    OnAIMove?.Invoke(x, y, isPass, isResign, aiPlayer.Color);
                    OnWinRate?.Invoke(winRate);
                    OnTerritoryAnalized?.Invoke(territoryStatictics);
                }
                catch (Exception ex)
                {
                    throw;
                }

            });
        }

    }
}
