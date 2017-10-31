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
                FileInfo file = new FileInfo(FilePath);
                if (!Directory.Exists(file.DirectoryName))
                {
                    Directory.CreateDirectory(file.DirectoryName);
                }

                if (!System.IO.File.Exists(FilePath))
                {
                    System.IO.FileStream f = System.IO.File.Create(FilePath);
                    f.Close();
                }
                System.IO.StreamWriter f2 = new System.IO.StreamWriter(FilePath, true, Encoding.UTF8);
                f2.Write(info);
                f2.Close();
                f2.Dispose();
            }
        }

    }
}
