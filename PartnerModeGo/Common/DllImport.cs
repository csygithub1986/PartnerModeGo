using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace PartnerModeGo
{
    public class DllImport
    {
        const string DLLNAME = "Zen.dll";

        [DllImport(DLLNAME, EntryPoint = "?ZenAddStone@@YA_NHHH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AddStone(int param0, int param1, int param2);

        [DllImport(DLLNAME, EntryPoint = "?ZenClearBoard@@YAXXZ", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClearBoard();

        [DllImport(DLLNAME, EntryPoint = "?ZenFixedHandicap@@YAXH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FixedHandicap(int param0);

        [DllImport(DLLNAME, EntryPoint = "?ZenGetBestMoveRate@@YAHXZ", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetBestMoveRate();

        [DllImport(DLLNAME, EntryPoint = "?ZenGetBoardColor@@YAHHH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetBoardColor(int param0, int param1);

        [DllImport(DLLNAME, EntryPoint = "?ZenGetHistorySize@@YAHXZ", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetHistorySize();

        [DllImport(DLLNAME, EntryPoint = "?ZenGetNextColor@@YAHXZ", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetNextColor();

        [DllImport(DLLNAME, EntryPoint = "?ZenGetNumBlackPrisoners@@YAHXZ", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetNumBlackPrisoners();

        [DllImport(DLLNAME, EntryPoint = "?ZenGetNumWhitePrisoners@@YAHXZ", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetNumWhitePrisoners();

        [DllImport(DLLNAME, EntryPoint = "?ZenInitialize@@YAXPBD@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Initialize(string param0);

        [DllImport(DLLNAME, EntryPoint = "?ZenIsInitialized@@YA_NXZ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsInitialized();

        [DllImport(DLLNAME, EntryPoint = "?ZenIsLegal@@YA_NHHH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsLegal(int param0, int param1, int param2);

        [DllImport(DLLNAME, EntryPoint = "?ZenIsSuicide@@YA_NHHH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsSuicide(int param0, int param1, int param2);

        [DllImport(DLLNAME, EntryPoint = "?ZenIsThinking@@YA_NXZ", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsThinking();

        [DllImport(DLLNAME, EntryPoint = "?ZenMakeShapeName@@YAXHHHPADH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MakeShapeName(int param0, int param1, int param2, byte[] param3, int param4);

        [DllImport(DLLNAME, EntryPoint = "?ZenPass@@YAXH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Pass(int param0);

        [DllImport(DLLNAME, EntryPoint = "?ZenPlay@@YA_NHHH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Play(int param0, int param1, int param2);

        [DllImport(DLLNAME, EntryPoint = "?ZenSetAmafWeightFactor@@YAXM@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetAmafWeightFactor(float param0);

        [DllImport(DLLNAME, EntryPoint = "?ZenSetBoardSize@@YAXH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetBoardSize(int param0);

        [DllImport(DLLNAME, EntryPoint = "?ZenSetDCNN@@YAX_N@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDCNN(bool param0);

        [DllImport(DLLNAME, EntryPoint = "?ZenSetKomi@@YAXM@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetKomi(float param0);

        [DllImport(DLLNAME, EntryPoint = "?ZenSetMaxTime@@YAXM@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMaxTime(float param0);

        [DllImport(DLLNAME, EntryPoint = "?ZenSetNextColor@@YAXH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNextColor(int param0);

        [DllImport(DLLNAME, EntryPoint = "?ZenSetNumberOfSimulations@@YAXH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNumberOfSimulations(int param0);

        [DllImport(DLLNAME, EntryPoint = "?ZenSetNumberOfThreads@@YAXH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNumberOfThreads(int param0);

        /// <summary>
        /// 不知道干什么的，设到500后，走棋和胜率都判断不正常了
        /// </summary>
        /// <param name="param0"></param>
        [DllImport(DLLNAME, EntryPoint = "?ZenSetPriorWeightFactor@@YAXM@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetPriorWeightFactor(float param0);

        [DllImport(DLLNAME, EntryPoint = "?ZenStartThinking@@YAXH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StartThinking(int param0);

        [DllImport(DLLNAME, EntryPoint = "?ZenStopThinking@@YAXXZ", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopThinking();

        [DllImport(DLLNAME, EntryPoint = "?ZenTimeLeft@@YAXHHH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TimeLeft(int param0, int param1, int param2);

        [DllImport(DLLNAME, EntryPoint = "?ZenTimeSettings@@YAXHHH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TimeSettings(int param0, int param1, int param2);

        [DllImport(DLLNAME, EntryPoint = "?ZenUndo@@YA_NH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Undo(int param0);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="order">zen分析的落点顺序</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="count"></param>
        /// <param name="winRate"></param>
        /// <param name="param5"></param>
        /// <param name="param6"></param>
        [DllImport(DLLNAME, EntryPoint = "?ZenGetTopMoveInfo@@YAXHAAH00AAMPADH@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetTopMoveInfo(int order, ref int x, ref int y, ref int count, ref float winRate, byte[] param5, int param6);

        [DllImport(DLLNAME, EntryPoint = "?ZenReadGeneratedMove@@YAXAAH0AA_N1@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReadGeneratedMove(ref int x, ref int y, ref bool isPass, ref bool isResign);


        [DllImport(DLLNAME, EntryPoint = "?ZenGetPriorKnowledge@@YAXQAY0BD@H@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetPriorKnowledge(int[] param0);


        /// <summary>
        /// 此函数需要接近1s
        /// </summary>
        /// <param name="param0"></param>
        [DllImport(DLLNAME, EntryPoint = "?ZenGetTerritoryStatictics@@YAXQAY0BD@H@Z", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetTerritoryStatictics(int[] param0);

    }
}
