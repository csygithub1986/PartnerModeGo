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
        public static string FilePath;
        private static object O_LockLog = new object();

        //static Sgf()
        //{
        //    FilePath = Environment.CurrentDirectory + "/Sgf棋谱/" + ServiceProxy.Instance.Session.UserName + DateTime.Now.ToString("MM-dd HH-mm-ss") + ".sgf";
        //}

        //public static void WriteSgf(string info)
        //{
        //    lock (O_LockLog)
        //    {
        //        FileInfo file = new FileInfo(FilePath);
        //        if (!Directory.Exists(file.DirectoryName))
        //        {
        //            Directory.CreateDirectory(file.DirectoryName);
        //        }

        //        if (!System.IO.File.Exists(FilePath))
        //        {
        //            System.IO.FileStream f = System.IO.File.Create(FilePath);
        //            f.Close();
        //        }
        //        System.IO.StreamWriter f2 = new System.IO.StreamWriter(FilePath, true, Encoding.UTF8);
        //        f2.Write(info);
        //        f2.Close();
        //        f2.Dispose();
        //    }
        //}


    }
}
