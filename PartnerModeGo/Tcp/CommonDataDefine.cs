using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo.Tcp
{
    public class CommonDataDefine
    {
        //PC->Phone数据头
        public const int GameStart = 0x1001;
        public const int GameOver = 0x1002;
        public const int ServerStepData = 0x1003;
        public const int Scan = 0x1004;
        public const int SendPreview = 0x1004;

        //Phone->PC数据头
        public const int PhoneStepData = 0x2001;
        public const int PreviewData = 0x2004;

        //文件信息 TODO：今后再丰富
        public const string FileName = "FileName";
        public const string BlackPlayerName = "BlackPlayerName";
        public const string WhitePlayerName = "WhitePlayerName";

    }
}
