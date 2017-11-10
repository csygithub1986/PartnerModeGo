using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PartnerModeGo
{
    /// <summary>
    /// 日志处理类
    /// </summary>
    public class Sgf
    {
        /// <summary>
        /// 计算Multigo等软件中，UI的坐标。与程序中不同的是，纵坐标相反，横坐标跳过i
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static string GetUICoordinate(int x, int y, int boardSize)
        {
            char xChar = (char)('a' + x);
            if (xChar >= 'i')
            {
                xChar++;
            }
            int yUI = boardSize - y;
            return "" + xChar + yUI;
        }





        //public static string FilePath;
        //private static object O_LockLog = new object();
    }
}
