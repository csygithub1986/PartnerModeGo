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
    /// 目前未加入UDP搜索
    /// </summary>
    public class TcpServer
    {
        //TcpListener
        /// <summary>
        /// 接受数据事件
        /// </summary>
        public event Action<byte[]> OnDataArrivedEvent;

        private TcpClient client;

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public void SendData(byte[] data)
        {
            NetworkStream nStream = client.GetStream();
            using (BinaryWriter bw = new BinaryWriter(nStream))
            {
                bw.Write(data.Length);
                bw.Write(data);
                bw.Flush();
            }
        }

        public void Start()
        {
            Thread thread = new Thread(Listen);
            thread.Start();
        }

        public void Listen()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 12121);
            tcpListener.Start();

            client = tcpListener.AcceptTcpClient();
            Thread thread = new Thread(ReceiveMessage);
            thread.Start(client);
        }

        /// <summary>
        /// 接受消息 TODO：未考虑粘包
        /// </summary>
        /// <param name="tcpClient"></param>
        private void ReceiveMessage()
        {
            //取得网络流  
            //TcpClient client = tcpClient as TcpClient;
            string ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            NetworkStream nStream = client.GetStream();
            BinaryReader br = new BinaryReader(nStream);

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
                        }
                        else
                        {
                            restByteCount = restByteCount - actualLen2;
                        }
                        continue;
                    }


                    dataLen = br.ReadInt32();//长度
                    data = new byte[dataLen];
                    int actualLen = br.Read(data, 0, data.Length);
                    if (actualLen == dataLen)
                    {
                        //添加消息至消息处理类
                        OnDataArrivedEvent?.Invoke(data);
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
