using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerModeGo.Tcp
{
    public class TcpDataHandler
    {
        #region 单例
        private TcpDataHandler()
        {
            TcpServer.Instance.HandlerOnDataArrived += AnalysePhoneData;
        }
        static TcpDataHandler()
        {
            Instance = new TcpDataHandler();
        }
        public static TcpDataHandler Instance;
        #endregion


        public event Action<int, int, int, int[]> WindowReceivePhoneStepData;
        public event Action<byte[], bool, int[]> WindowReceivePhonePreviewData;

        #region PC->Phone
        /// <summary>
        /// 开始指令
        /// </summary>
        public void SendGameStart(string gameInfo)
        {
            //数据格式：命令头+数据总长度+棋谱信息string
            //棋谱信息string格式：aaa=bbb;ccc=ddd;
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(CommonDataDefine.GameStart));
            int len = gameInfo.Length;
            data.AddRange(BitConverter.GetBytes(len));
            data.AddRange(Encoding.UTF8.GetBytes(gameInfo));
            //再加上有效数据量总长度
            data.InsertRange(0, BitConverter.GetBytes(data.Count));
            TcpServer.Instance.SendData(data.ToArray());
        }

        /// <summary>
        /// 结束指令
        /// </summary>
        public void SendGameOver()
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(CommonDataDefine.GameOver));
            data.AddRange(BitConverter.GetBytes(0));
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
        public void SendStepData(int x, int y, int color, int[] boardState)
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(CommonDataDefine.ServerStepData));
            data.Add((byte)x);
            data.Add((byte)y);
            data.Add((byte)color);
            data.AddRange(BitConverter.GetBytes(boardState.Length));
            data.AddRange(boardState.Select(p => (byte)p).ToArray());
            //再加上有效数据量总长度
            data.InsertRange(0, BitConverter.GetBytes(data.Count));
            TcpServer.Instance.SendData(data.ToArray());
        }

        public void SendScan()
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(CommonDataDefine.Scan));
            data.AddRange(BitConverter.GetBytes(0));
            //再加上有效数据量总长度
            data.InsertRange(0, BitConverter.GetBytes(data.Count));
            TcpServer.Instance.SendData(data.ToArray());
        }

        public void SendPreviewCommand(bool isPreview)
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(CommonDataDefine.SendPreviewCommand));
            data.AddRange(BitConverter.GetBytes(isPreview));
            //再加上有效数据量总长度
            data.InsertRange(0, BitConverter.GetBytes(data.Count));
            TcpServer.Instance.SendData(data.ToArray());
        }
        #endregion

        #region Phone->PC
        public void AnalysePhoneData(byte[] data)
        {
            int index = 0;
            int head = BitConverter.ToInt32(data, index); index += 4;
            if (head == CommonDataDefine.PhoneStepData)
            {
                int x = data[index]; index++;
                int y = data[index]; index++;
                int color = data[index]; index++;
                int boardLen = BitConverter.ToInt32(data, index); index += 4;
                int[] boardState = new int[boardLen];
                for (int i = 0; i < boardState.Length; i++)
                {
                    boardState[i] = data[index]; index++;
                }
                WindowReceivePhoneStepData?.Invoke(x, y, color, boardState);
            }
            else if (head == CommonDataDefine.PreviewData)
            {
                //识别成功与否
                bool isOk = data[index] == 1; index += 1;
                //图像
                int imagelen = BitConverter.ToInt32(data, index); index += 4;
                Console.WriteLine("图像大小: " + imagelen);
                byte[] image = new byte[imagelen];
                for (int i = 0; i < image.Length; i++)
                {
                    image[i] = data[index]; index++;
                }
                //解析的棋盘数据
                int boardLen = BitConverter.ToInt32(data, index); index += 4;
                int[] boardState = new int[boardLen];
                for (int i = 0; i < boardState.Length; i++)
                {
                    boardState[i] = data[index]; index++;
                }
                WindowReceivePhonePreviewData?.Invoke(image, isOk, boardState);
            }
        }


        #endregion
    }
}
