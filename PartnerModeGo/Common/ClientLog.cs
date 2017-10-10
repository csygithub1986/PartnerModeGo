using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Concurrent;

namespace PartnerModeGo
{
    /// <summary>
    /// 日志处理类
    /// </summary>
    public class ClientLog
    {
        public static string FilePath;
        private static object O_LockLog = new object();
        public static void WriteLog(string info)
        {
            lock (O_LockLog)
            {
                //string filePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Log.txt";

                if (!Directory.Exists(FilePath))
                {
                    P_creatpath(FilePath);
                }

                if (!System.IO.File.Exists(FilePath))
                {
                    System.IO.FileStream f = System.IO.File.Create(FilePath);
                    f.Close();
                }
                System.IO.StreamWriter f2 = new System.IO.StreamWriter(FilePath, true, System.Text.Encoding.GetEncoding("gb2312"));
                f2.Write(info);
                f2.Close();
                f2.Dispose();
            }
        }


        //多级目录判断是否存在目录路径，如果不存在则新建
        private static void P_creatpath(string path)
        {
            string tpath = path.Substring(0, path.LastIndexOf("\\"));
            if (!Directory.Exists(tpath))
            {
                P_creatpath(tpath);
                Directory.CreateDirectory(tpath);
            }
        }

    }
}
