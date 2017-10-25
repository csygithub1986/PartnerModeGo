using LeagueGoServer.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueGoServer
{
    public class Common
    {
        static Common()
        {
            Player p1 = new Player() { Name = "王", Color = 2, Type = PlayerType.Host, ID = 1 };
            Player p2 = new Player() { Name = "李", Color = 2, Type = PlayerType.AI, ID = 2 };
            Player p3 = new Player() { Name = "刘", Color = 1, Type = PlayerType.Internet, ID = 3 };
            Player p4 = new Player() { Name = "林", Color = 1, Type = PlayerType.Internet, ID = 4 };
            GameList.TryAdd("1111", new Game() { Name = "bbb", Players = new Player[] { p1, p2, p3, p4 }, GameSetting = new GameSetting() { BoardSize = 19, Handicap = 4, Komi = 0 } });
        }

        /// <summary>
        /// 客户端集合，键为[SessionId]；值为[ClientInfo]
        /// </summary>
        public static ConcurrentDictionary<string, ClientInfo> ClientList = new ConcurrentDictionary<string, ClientInfo>();
        /// <summary>
        /// 游戏集合，键为Host的[SessionId]；值为[ClientInfo]
        /// </summary>
        public static ConcurrentDictionary<string, Game> GameList = new ConcurrentDictionary<string, Game>();


        #region 集合的增删改查

        /// <summary>
        /// 向客户端集合添加一个客户端
        /// </summary>
        /// <param name="key">键，客户端会话SSID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ClientListAdd(string key, ClientInfo value)
        {
            bool res = ClientList.TryAdd(key, value);
            return res;
        }

        /// <summary>
        /// 修改客户端集合中的一个客户端信息
        /// </summary>
        /// <param name="key">键，客户端会话SSID</param>
        /// <param name="value"></param>
        public static void ClientListModify(string key, ClientInfo value)
        {

            ClientList.AddOrUpdate(key, value,
                (oldkey, oldvalue) =>
                {
                    return value;
                });
        }

        /// <summary>
        /// 从客户端集合中获取某个连接会话ID对应的客户端信息
        /// </summary>
        /// <param name="key">键，客户端连接的会话SSID</param>
        /// <returns></returns>
        public static ClientInfo ClientListGet(string key)
        {
            try
            {
                ClientInfo info = new ClientInfo();
                bool res = ClientList.TryGetValue(key, out info);
                if (!res)
                    return null;
                return info;
            }
            catch (Exception ex)
            {
                //Trace.TraceError(String.Format("方法[ClientListGet]执行失败 {0}", ex));
                //Log.AddErrorInfo(String.Format("方法[ClientListGet]执行失败 {0}", ex), DateTime.Now);
                return null;
            }
        }

        ///// <summary>
        ///// 从客户端集合中获取客户端
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //public static ClientInfo ClientListGet(int userId)
        //{
        //    try
        //    {
        //        ClientInfo info = ClientList.Values.FirstOrDefault(i => i.UserID == userId);
        //        return info;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.AddErrorInfo(String.Format("方法[ClientListGet]执行失败 {0}", ex), DateTime.Now);
        //        return null;
        //    }
        //}

        /// <summary>
        /// 从客户端集合中删除某个客户端
        /// </summary>
        /// <param name="key">键，客户端会话SSID</param>
        /// <returns></returns>
        public static bool ClientListDelete(string key)
        {
            ClientInfo info = new ClientInfo();
            bool res = ClientList.TryRemove(key, out info);
            return res;
        }

        #endregion
    }
}
