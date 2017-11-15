using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PartnerModeGo
{
    /// <summary>
    /// 只用于接收来自Phone的连接，只有一个连接，单例。目前未加入UDP搜索
    /// </summary>
    public class TcpServer
    {
        #region 单例
        private TcpServer() { }
        static TcpServer()
        {
            Instance = new TcpServer();
        }
        public static TcpServer Instance;
        #endregion

        #region 属性、字段
        public bool IsConnected { get; set; }

        private TcpClient client;
        private BinaryWriter bw;
        #endregion

        #region 事件
        ///// <summary>
        ///// 接受数据事件
        ///// </summary>
        //public event Action<byte[]> HandlerOnDataArrived;

        /// <summary>
        /// 连接成功
        /// </summary>
        public event Action<bool> PhoneConnectedChanged;

        public event Action<int, int, int> WindowReceivePhoneStepData;
        public event Action<byte[], int, int[]> WindowReceivePhonePreviewData;
        public event Action<int> WindowReceivePhoneScanState;

        #endregion

        #region 底层方法
        public void Start()
        {
            Thread thread = new Thread(Listen);
            thread.Start();
        }

        private void Listen()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 12123);
            tcpListener.Start();
            client = tcpListener.AcceptTcpClient();

            IsConnected = true;
            PhoneConnectedChanged?.Invoke(true);

            NetworkStream nStream = client.GetStream();
            bw = new BinaryWriter(nStream);

            Thread thread = new Thread(ReceiveMessage);
            thread.Start();
        }

        /// <summary>
        /// 发送数据 TODO：发送之前检查网络状态
        /// </summary>
        /// <param name="data"></param>
        private void SendData(byte[] data)
        {
            //bw.Write(data.Length);
            bw.Write(data);
            bw.Flush();
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
                                //HandlerOnDataArrived?.Invoke(data);
                                AnalysePhoneData(data);
                            }
                            else
                            {
                                restByteCount = restByteCount - actualLen2;
                                ////Console.WriteLine("dataLen:  " + restByteCount);
                            }
                            continue;
                        }

                        dataLen = br.ReadInt32();//长度
                        //Console.WriteLine("dataLen:  " + dataLen);
                        data = new byte[dataLen];
                        int actualLen = br.Read(data, 0, data.Length);
                        if (actualLen == dataLen)
                        {
                            //添加消息至消息处理类
                            //HandlerOnDataArrived?.Invoke(data);
                            AnalysePhoneData(data);
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
        #endregion


        #region PC->Phone 发送数据
        public void SendScan(int[] state, int color)
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(TcpHeaderDefine.StartScan));
            data.AddRange(BitConverter.GetBytes((short)state.Length));
            data.AddRange(state.Select(p => (byte)p));
            data.Add((byte)color);
            //再加上有效数据量总长度
            data.InsertRange(0, BitConverter.GetBytes(data.Count));
            SendData(data.ToArray());
        }


        /// <summary>
        /// 结束指令
        /// </summary>
        public void SendGameOver(byte[] fileNameBytes, byte[] fileData)
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(TcpHeaderDefine.GameOver));
            data.AddRange(BitConverter.GetBytes((short)fileNameBytes.Length));
            data.AddRange(fileNameBytes);
            data.AddRange(BitConverter.GetBytes(fileData.Length));
            data.AddRange(fileData);
            //再加上有效数据量总长度
            data.InsertRange(0, BitConverter.GetBytes(data.Count));
            TcpServer.Instance.SendData(data.ToArray());
        }

        /// <summary>
        /// 发送新棋步
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="boardState"></param>
        public void SendStepData(int x, int y, int color)
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(TcpHeaderDefine.HostStepData));
            data.Add((byte)x);
            data.Add((byte)y);
            data.Add((byte)color);
            //再加上有效数据量总长度
            data.InsertRange(0, BitConverter.GetBytes(data.Count));
            TcpServer.Instance.SendData(data.ToArray());
        }


        public void SendPreviewCommand(bool isPreview)
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(TcpHeaderDefine.SendPreviewCommand));
            data.AddRange(BitConverter.GetBytes(isPreview));
            //再加上有效数据量总长度
            data.InsertRange(0, BitConverter.GetBytes(data.Count));
            TcpServer.Instance.SendData(data.ToArray());
        }
        #endregion

        #region Phone->PC 接收数据
        public void AnalysePhoneData(byte[] data)
        {
            int index = 0;
            short head = BitConverter.ToInt16(data, index); index += 2;
            if (head == TcpHeaderDefine.PhoneStepData)
            {
                byte x = data[index]; index++;
                byte y = data[index]; index++;
                byte color = data[index]; index++;
                //int boardLen = BitConverter.ToInt32(data, index); index += 4;
                //int[] boardState = new int[boardLen];
                //for (int i = 0; i < boardState.Length; i++)
                //{
                //    boardState[i] = data[index]; index++;
                //}
                WindowReceivePhoneStepData?.Invoke(x, y, color);
            }
            else if (head == TcpHeaderDefine.PhonePreviewData)
            {
                //识别状态
                byte state = data[index]; index += 1;
                //图像
                int imagelen = BitConverter.ToInt32(data, index); index += 4;
                //Console.WriteLine("图像大小: " + imagelen);
                byte[] image = new byte[imagelen];
                for (int i = 0; i < image.Length; i++)
                {
                    image[i] = data[index]; index++;
                }
                //解析的棋盘数据
                short boardLen = BitConverter.ToInt16(data, index); index += 2;
                int[] boardState = new int[boardLen];
                for (int i = 0; i < boardState.Length; i++)
                {
                    boardState[i] = data[index]; index++;
                }
                WindowReceivePhonePreviewData?.Invoke(image, state, boardState);
            }
            else if (head == TcpHeaderDefine.PhoneScanState)
            {
                //识别状态
                byte state = data[index];
                WindowReceivePhoneScanState?.Invoke(state);
            }
        }


        #endregion
    }
}
