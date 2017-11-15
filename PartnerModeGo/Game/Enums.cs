using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    /// <summary>
    /// 本机在互联网游戏中的类型
    /// </summary>
    public enum LocalType
    {
        Host, Client
    }

    public enum RecognizeState
    {
        未识别, 状态不正确, 状态正确
    }
}
