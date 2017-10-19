using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PartnerModeGo.Tcp
{
    /// <summary>
    /// 只用于接收来自Phone的连接，只有一个连接，单例。目前未加入UDP搜索
    /// </summary>
    public class TcpServer
    {
        private TcpServer() { }
        static TcpServer()
        {
            Instance = new TcpServer();
        }
        public static TcpServer Instance;

        /// <summary>
        /// 接受数据事件
        /// </summary>
        public event Action<byte[]> HandlerOnDataArrived;

        /// <summary>
        /// 连接成功
        /// </summary>
        public event Action<string> WindowOnPhoneConnected;

        private TcpClient client;
        private BinaryWriter bw;

        /// <summary>
        /// 发送数据 TODO：发送之前检查网络状态
        /// </summary>
        /// <param name="data"></param>
        public void SendData(byte[] data)
        {
            //bw.Write(data.Length);
            bw.Write(data);
            bw.Flush();
        }

        public void Start()
        {
            Thread thread = new Thread(Listen);
            thread.Start();
        }

        public void Listen()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Parse("192.168.1.111"), 12121);
            tcpListener.Start();
            client = tcpListener.AcceptTcpClient();
           
            WindowOnPhoneConnected?.Invoke(client.Client.RemoteEndPoint.ToString());

            NetworkStream nStream = client.GetStream();
            bw = new BinaryWriter(nStream);

            Thread thread = new Thread(ReceiveMessage);
            thread.Start();
        }

        /// <summary>
        /// 接受消息  
        /// 先读数据长度，然后全部截取
        /// </summary>
        /// <param name="tcpClient"></param>
        private void ReceiveMessage()
        {
            //取得网络流  
            //TcpClient client = tcpClient as TcpClient;
            string ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            NetworkStream nStream = client.GetStream();

            using (BinaryReader br = new BinaryReader(nStream))
            {
                //读取消息  
                int dataLen = 0;//总共的消息长度
                int restByteCount = 0;//剩余的字节数
                byte[] data = null;

                while (true)
                {
                    try
                    {
                        if (restByteCount != 0)
                        {
                            int actualLen2 = br.Read(data, dataLen - restByteCount, restByteCount);
                            if (actualLen2 == restByteCount)
                            {
                                restByteCount = 0;
                                //添加消息至消息处理类
                                HandlerOnDataArrived?.Invoke(data);
                            }
                            else
                            {
                                restByteCount = restByteCount - actualLen2;
                                //Console.WriteLine("dataLen:  " + restByteCount);
                            }
                            continue;
                        }

                        dataLen = br.ReadInt32();//长度
                        Console.WriteLine("dataLen:  " + dataLen);
                        data = new byte[dataLen];
                        int actualLen = br.Read(data, 0, data.Length);
                        if (actualLen == dataLen)
                        {
                            //添加消息至消息处理类
                            HandlerOnDataArrived?.Invoke(data);
                        }
                        else
                        {
                            restByteCount = dataLen - actualLen;
                        }
                    }
                    catch (Exception)
                    {
                        //网络异常，通知系统删除本socket数据
                        client.Close();
                        return;
                    }
                }
            }


        }
    }
}
