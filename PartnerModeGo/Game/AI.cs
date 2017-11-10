﻿using PartnerModeGo.WcfService;
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

                            //分析胜率和推荐（一共最多3个推荐）
                            List<Tuple<Position, float>> recommend = new List<Tuple<Position, float>>();
                            int x0 = 0, y0 = 0;
                            int count = 0;
                            float winRate0 = 0;
                            DllImport.GetTopMoveInfo(0, ref x0, ref y0, ref count, ref winRate0, null, 0);
                            recommend.Add(new Tuple<Position, float>(new Position(x0, y0), step.Player.Color == 1 ? winRate0 : 1 - winRate0));//第一个推荐


                            float winRate1 = 0;
                            int x1 = 0, y1 = 0;
                            DllImport.GetTopMoveInfo(0, ref x1, ref y1, ref count, ref winRate1, null, 0);
                            if (!float.IsNaN(winRate1) && winRate1 - winRate0 > -0.05f)//差别不到5%，就采纳
                            {
                                recommend.Add(new Tuple<Position, float>(new Position(x1, y1), step.Player.Color == 1 ? winRate1 : 1 - winRate1));//第二个推荐
                            }

                            float winRate2 = 0;
                            int x2 = 0, y2 = 0;
                            DllImport.GetTopMoveInfo(2, ref x2, ref y2, ref count, ref winRate2, null, 0);
                            if (!float.IsNaN(winRate2) && winRate2 - winRate0 > -0.05f)//差别不到5%，就采纳
                            {
                                recommend.Add(new Tuple<Position, float>(new Position(x2, y2), step.Player.Color == 1 ? winRate2 : 1 - winRate2));//第三个推荐
                            }

                            //形势估计
                            int[] territoryStatictics = new int[m_BoardSize * m_BoardSize];
                            DllImport.GetTerritoryStatictics(territoryStatictics);
                            //修改step
                            step.BlackWinRate = step.Player.Color == 1 ? winRate0 : 1 - winRate0;
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

                        int x = 0, y = 0;
                        bool isPass = false, isResign = false;
                        DllImport.ReadGeneratedMove(ref x, ref y, ref isPass, ref isResign);

                        if (lastStep != null)
                        {
                            //分析胜率和推荐（一共最多3个推荐）
                            int count = 0;
                            float winRate0 = 0;
                            int xx = 0, yy = 0;
                            DllImport.GetTopMoveInfo(0, ref xx, ref yy, ref count, ref winRate0, null, 0);
                            List<Tuple<Position, float>> recommend = new List<Tuple<Position, float>>();
                            recommend.Add(new Tuple<Position, float>(new Position(xx, yy), lastStep.Player.Color == 1 ? winRate0 : 1 - winRate0));//第一个推荐

                            float winRate1 = 0;
                            DllImport.GetTopMoveInfo(1, ref xx, ref yy, ref count, ref winRate1, null, 0);
                            if (!float.IsNaN(winRate1) && winRate1 - winRate0 > -0.05f)//差别不到5%，就采纳
                            {
                                recommend.Add(new Tuple<Position, float>(new Position(xx, yy), lastStep.Player.Color == 1 ? winRate1 : 1 - winRate1));//第二个推荐
                            }

                            float winRate2 = 0;
                            DllImport.GetTopMoveInfo(2, ref xx, ref yy, ref count, ref winRate2, null, 0);
                            if (!float.IsNaN(winRate2) && winRate2 - winRate0 > -0.05f)//差别不到5%，就采纳
                            {
                                recommend.Add(new Tuple<Position, float>(new Position(xx, yy), lastStep.Player.Color == 1 ? winRate2 : 1 - winRate2));//第三个推荐
                            }

                            //地域
                            int[] territoryStatictics = new int[m_BoardSize * m_BoardSize];
                            DllImport.GetTerritoryStatictics(territoryStatictics);


                            lastStep.BlackWinRate = lastStep.Player.Color == 1 ? winRate0 : 1 - winRate0;
                            lastStep.Territory = territoryStatictics;
                            lastStep.BlackLeadPoints = (float)(territoryStatictics.Sum() / 1000.0 - m_Komi);
                            lastStep.RecommendPoints = recommend;

                            OnAIMove?.Invoke(x, y, isPass, isResign, aiPlayer.Color, winRate0, territoryStatictics);
                        }
                        else
                        {
                            OnAIMove?.Invoke(x, y, isPass, isResign, aiPlayer.Color, 0, null);
                        }

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
