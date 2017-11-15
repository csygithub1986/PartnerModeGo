using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    public class TcpHeaderDefine
    {
        #region PC->Phone数据头

        /// <summary>
        /// 开始扫描
        /// 头+棋盘状态byte[]+color
        /// 2          2+n             1
        /// </summary>
        public const short StartScan = 0x1001;

        /// <summary>
        /// PC给Phone的棋步信息
        /// 头+x+y+color
        /// 2    1  1    1
        /// </summary>
        public const short HostStepData = 0x1002;

        /// <summary>
        /// 棋局结束，发送棋谱
        /// 头+文件名+文件
        /// 2    2+n     4+n
        /// </summary>
        public const short GameOver = 0x1003;


        /// <summary>
        /// 是否发送图像预览
        /// 头+bool是否
        /// 2        1
        /// </summary>
        public const short SendPreviewCommand = 0x1004;

        #endregion


        #region Phone->PC数据头
        /// <summary>
        /// phone发来棋步
        /// 头+x+y+color
        /// 2   1   1     1
        /// </summary>
        public const short PhoneStepData = 0x2001;

        /// <summary>
        /// phone识别状态
        /// 头+状态
        /// 2      1
        /// </summary>
        public const short PhoneScanState = 0x2002;//0：未识别  1：识别但状态不正确  2：识别且状态正确

        /// <summary>
        /// 预览图像
        /// 头+状态+图像byte[]+棋盘数据byte[]
        /// 2      1        4+n                2+n
        /// </summary>
        public const short PhonePreviewData = 0x2003;
        #endregion

        //文件信息 TODO：今后再丰富
        public const string FileName = "FileName";
        public const string BlackPlayerName = "BlackPlayerName";
        public const string WhitePlayerName = "WhitePlayerName";

    }
}
