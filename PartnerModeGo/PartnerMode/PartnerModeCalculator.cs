using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PartnerModeGo
{
    public class PartnerModeCalculator
    {
        private int m_TotalGameLoopTimes;
        private int m_CurrentGameTimes;
        private PlayerSetting[] aiSettings;
        private int m_BoardSize;

        public Action StartCallback;
        public Action<int, int, int, bool, bool> UICallback;
        public Action<int, int, int, bool, bool> GameOverCallback;
        public Action<int[]> TerritoryCallback;
        public Action<float> WinRateCallback;

        public Action<int> HandTurnCallback;//移交顺序

        /// <summary>
        /// 历史记录 分别记录 步数、x坐标、y坐标、ispass、isresign
        /// </summary>
        private List<Tuple<int, int, bool, bool>> m_History;

        public PartnerModeCalculator(PlayerSetting[] settings, int totalGameLoopTimes, int boardSize)
        {
            aiSettings = settings;
            m_BoardSize = boardSize;
            m_TotalGameLoopTimes = totalGameLoopTimes;
            m_CurrentGameTimes = 1;
        }

        public void InitGame()
        {
            DllImport.Initialize("ZenInit-" + m_CurrentGameTimes + " " + DateTime.Now.ToString("yyMMddHHmmss") + ".txt");//TODO:文件名，在界面中加入playerName，然后命名
            m_History = new List<Tuple<int, int, bool, bool>>();

            ClientLog.FilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + DateTime.Now.ToString("MM-dd HH-mm-ss") + "~ZenVsZen.sgf";//TODO，命名
            ClientLog.WriteLog("(;PB[xyz]PW[abc]");
        }

        public void Start()
        {
            InitGame();
            StartCallback?.Invoke();
            if (aiSettings[0].IsZen)
            {
                GetZenMove(0);
            }
            else
            {
                HandTurnCallback?.Invoke(0);
            }
        }

        /// <summary>
        /// Zen走棋（线程）
        /// </summary>
        /// <param name="stepNum"></param>
        public void GetZenMove(int stepNum)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                int turn = stepNum % 4;
                DllImport.SetNumberOfSimulations(aiSettings[turn].Layout);
                DllImport.StartThinking(aiSettings[turn].Color);
                Thread.Sleep(aiSettings[turn].TimePerMove * 1000);
                DllImport.StopThinking();
                //Thread.Sleep(500);
                int x = 0, y = 0;
                bool isPass = false, isResign = false;
                DllImport.ReadGeneratedMove(ref x, ref y, ref isPass, ref isResign);
                int count = 0;
                float winRate = 0;
                DllImport.GetTopMoveInfo(0, ref x, ref y, ref count, ref winRate, null, 0);


                DealZenResult(stepNum, x, y, isPass, isResign, count, winRate);
            });
        }

        /// <summary>
        /// 获得外部提交的一步棋步（非主线程）
        /// </summary>
        /// <param name="stepNum"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isPass"></param>
        /// <param name="isResign"></param>
        public void OutsiderMoveArrived(int stepNum, int x, int y, bool isPass, bool isResign)
        {
            m_History.Add(new Tuple<int, int, bool, bool>(x, y, isPass, isResign));

            if (isPass)
            {
                if (m_History.Count > 1 && m_History[m_History.Count - 2].Item3)//两手pass
                {
                    ClientLog.WriteLog(")");
                    if (m_CurrentGameTimes == m_TotalGameLoopTimes)//全部比赛完成
                    {
                        GameOverCallback?.Invoke(stepNum, x, y, isPass, isResign);
                    }
                    else
                    {
                        m_CurrentGameTimes++;
                        Start();//又重新开始
                    }
                    return;
                }
            }

            if (isResign)
            {
                ClientLog.WriteLog(")");
                if (m_CurrentGameTimes == m_TotalGameLoopTimes)//全部比赛完成
                {
                    GameOverCallback?.Invoke(stepNum, x, y, isPass, isResign);
                }
                else
                {
                    m_CurrentGameTimes++;
                    Start();//又重新开始
                }
                return;
            }


            DllImport.Play(x, y, 2 - stepNum % 2);
            UICallback?.Invoke(stepNum, x, y, isPass, isResign);

            if (TerritoryCallback != null)
            {
                int[] territoryStatictics = new int[m_BoardSize * m_BoardSize];
                DllImport.GetTerritoryStatictics(territoryStatictics);
                TerritoryCallback.Invoke(territoryStatictics);
            }

            ClientLog.WriteLog(";" + (stepNum % 2 == 1 ? "W" : "B") + "[" + (char)('a' + x) + (char)('a' + y) + "]");
            //Console.WriteLine((stepNum % 2 == 0 ? "黑" : "白") + "  WinRate: " + x + " " + y);


            stepNum++;
            int turn = stepNum % 4;
            if (aiSettings[turn].IsZen)
            {
                //如果下一步还是Zen
                GetZenMove(stepNum);
            }
            else
            {
                //如果不是zen，交出控制
                HandTurnCallback?.Invoke(stepNum);
            }
        }

        /// <summary>
        /// 处理Zen结果（非主线程）
        /// </summary>
        /// <param name="stepNum"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isPass"></param>
        /// <param name="isResign"></param>
        private void DealZenResult(int stepNum, int x, int y, bool isPass, bool isResign, int count, float winRate)
        {
            m_History.Add(new Tuple<int, int, bool, bool>(x, y, isPass, isResign));
            if (isPass)
            {
                if (m_History.Count > 1 && m_History[m_History.Count - 2].Item3)//两手pass
                {
                    ClientLog.WriteLog(")");
                    if (m_CurrentGameTimes == m_TotalGameLoopTimes)//全部比赛完成
                    {
                        GameOverCallback?.Invoke(stepNum, x, y, isPass, isResign);
                    }
                    else
                    {
                        m_CurrentGameTimes++;
                        Start();//又重新开始
                    }
                    return;
                }
            }

            if (isResign)
            {
                ClientLog.WriteLog(")");
                if (m_CurrentGameTimes == m_TotalGameLoopTimes)//全部比赛完成
                {
                    GameOverCallback?.Invoke(stepNum, x, y, isPass, isResign);
                }
                else
                {
                    m_CurrentGameTimes++;
                    Start();//又重新开始
                }
                return;
            }

            DllImport.Play(x, y, 2 - stepNum % 2);

            //因为GetTerritoryStatictics耗时，所以这样处理两次，让UI连续
            int[] territoryStatictics = null;
            if (TerritoryCallback != null)
            {
                territoryStatictics = new int[m_BoardSize * m_BoardSize];
                DllImport.GetTerritoryStatictics(territoryStatictics);
            }

            UICallback?.Invoke(stepNum, x, y, isPass, isResign);

            if (TerritoryCallback != null)
            {
                TerritoryCallback.Invoke(territoryStatictics);
                Console.WriteLine((stepNum % 2 == 0 ? "黑" : "白") + "走棋  黑胜率: " + (stepNum % 2 == 0 ? winRate : 1 - winRate).ToString("F2") + "  黑领先目数：" + (territoryStatictics.Sum() / 1000.0 - 6.5).ToString("F1"));
            }

            ClientLog.WriteLog(";" + (stepNum % 2 == 1 ? "W" : "B") + "[" + (char)('a' + x) + (char)('a' + y) + "]" + "C[胜率：" + winRate.ToString("F2") + "% count=" + count + "]");
            WinRateCallback?.Invoke(stepNum % 2 == 0 ? winRate : 1 - winRate);


            stepNum++;
            int turn = stepNum % 4;
            if (aiSettings[turn].IsZen)
            {
                //如果下一步还是Zen
                GetZenMove(stepNum);
            }
            else
            {
                //如果不是zen，交出控制
                HandTurnCallback?.Invoke(stepNum);
            }
        }

    }
}
