using PartnerModeGo.WcfService;
using System;
using System.Collections.Concurrent;
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
        private float m_Komi;
        private Game m_Game;
        public event Action<int, int, bool, bool, int, float, int[]> OnAIMove;
        //public event Action<float> OnWinRate;
        //public event Action<int[]> OnTerritoryAnalized;
        /// <summary>
        /// AI是进入或退出思考状态事件
        /// </summary>
        public event Action<bool> OnAiThinking;

        public AI(Game game)
        {
            m_Game = game;
            m_BoardSize = game.GameSetting.BoardSize;
            m_Komi = game.GameSetting.Komi;
        }


        #region 胜率分析、形势估计相关
        private ConcurrentQueue<Step> m_AnalyseQueue = new ConcurrentQueue<Step>();// 分析队列
        private AutoResetEvent m_AnalyseResetEvent = new AutoResetEvent(false);// 分析线程同步信号
        private object m_AnalyseLock = new object();
        #endregion

        public void Init()
        {
            DllImport.Initialize(DateTime.Now.Ticks + ".aitemp");
            DllImport.SetBoardSize(m_BoardSize);
            DllImport.SetNumberOfSimulations(30000);//大概1000步1s，最大应该不会超过30秒

            StartAnalyse();
        }

        #region 胜率、形势估计 相关函数

        private void StartAnalyse()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    OnAiThinking?.Invoke(true);
                    lock (m_AnalyseLock)
                    {
                        while (!m_AnalyseQueue.IsEmpty)
                        {
                            bool deq = m_AnalyseQueue.TryDequeue(out Step step);
                            if (!deq)
                            {
                                ClientLog.WriteLog("m_AnalyseQueue为空");
                            }
                            //分析胜率
                            DllImport.StartThinking(3 - step.Player.Color);
                            Thread.Sleep(2000); //固定分析2s
                            DllImport.StopThinking();
                            int x = 0, y = 0;
                            int count = 0;
                            float winRate = 0;
                            DllImport.GetTopMoveInfo(0, ref x, ref y, ref count, ref winRate, null, 0);
                            //形势估计
                            int[] territoryStatictics = new int[m_BoardSize * m_BoardSize];
                            DllImport.GetTerritoryStatictics(territoryStatictics);
                            //修改step
                            step.BlackWinRate = step.Player.Color == 1 ? winRate : 1 - winRate;
                            step.Territory = territoryStatictics;
                            step.BlackLeadPoints = (float)(territoryStatictics.Sum() / 1000.0 - m_Komi);
                        }
                    }
                    OnAiThinking?.Invoke(false);
                    m_AnalyseResetEvent.WaitOne();
                }
            });
        }

        public void AddAnalyseStep(Step step)
        {
            m_AnalyseQueue.Enqueue(step);
            m_AnalyseResetEvent.Set();
        }
        #endregion

        public void Play(int x, int y, int color)
        {
            DllImport.Play(x, y, color);
        }

        public void AIThink(Player aiPlayer, Step lastStep)
        {
            OnAiThinking?.Invoke(true);

            Task task = Task.Factory.StartNew(() =>
            {
                lock (m_AnalyseLock)
                {
                    try
                    {
                        //DllImport.SetNumberOfSimulations(aiPlayer.Layout); //Console.WriteLine("设置ai layout " + aiPlayer.Layout);
                        DllImport.StartThinking(aiPlayer.Color); //Console.WriteLine("ai StartThinking " + aiPlayer.Color);
                        Thread.Sleep(aiPlayer.TimePerMove * 1000); //Console.WriteLine("ai sleep " + aiPlayer.TimePerMove);
                        DllImport.StopThinking(); //Console.WriteLine("ai stopThink " + aiPlayer.Color);
                        //Thread.Sleep(500);
                        int x = 0, y = 0;
                        bool isPass = false, isResign = false;
                        DllImport.ReadGeneratedMove(ref x, ref y, ref isPass, ref isResign);

                        int count = 0;
                        float winRate = 0;
                        DllImport.GetTopMoveInfo(0, ref x, ref y, ref count, ref winRate, null, 0);


                        #region 测试
                        //DllImport.GetTopMoveInfo(0, ref x, ref y, ref count, ref winRate, null, 0);

                        //Console.Write("推荐：" + (winRate * 100).ToString("F1") + " , ");
                        //DllImport.GetTopMoveInfo(1, ref x, ref y, ref count, ref winRate, null, 0);
                        //Console.Write((winRate * 100).ToString("F1") + " , ");
                        //DllImport.GetTopMoveInfo(2, ref x, ref y, ref count, ref winRate, null, 0);
                        //Console.Write((winRate * 100).ToString("F1") + " , ");
                        //DllImport.GetTopMoveInfo(3, ref x, ref y, ref count, ref winRate, null, 0);
                        //Console.Write((winRate * 100).ToString("F1") + " , ");
                        //DllImport.GetTopMoveInfo(4, ref x, ref y, ref count, ref winRate, null, 0);
                        //Console.WriteLine((winRate * 100).ToString("F1"));

                        #endregion



                        int[] territoryStatictics = new int[m_BoardSize * m_BoardSize];
                        DllImport.GetTerritoryStatictics(territoryStatictics);

                        if (lastStep != null)
                        {
                            lastStep.BlackWinRate = lastStep.Player.Color == 1 ? winRate : 1 - winRate;
                            lastStep.Territory = territoryStatictics;
                            lastStep.BlackLeadPoints = (float)(territoryStatictics.Sum() / 1000.0 - m_Komi);
                        }

                        OnAIMove?.Invoke(x, y, isPass, isResign, aiPlayer.Color, winRate, territoryStatictics);
                        //OnWinRate?.Invoke(winRate);
                        //OnTerritoryAnalized?.Invoke(territoryStatictics);
                        OnAiThinking?.Invoke(false);
                    }
                    catch (Exception ex)
                    {
                        ClientLog.WriteLog(ex.ToString());
                        throw;
                    }
                }



            });
        }

    }
}
