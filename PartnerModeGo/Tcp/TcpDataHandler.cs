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
        private TcpDataHandler() { }
        static TcpDataHandler()
        {
            Instance = new TcpDataHandler();
        }
        public static TcpDataHandler Instance;
        #endregion



        public event Action<int, int, int[]> ReceivePhoneStepData;
        public event Action<byte[], bool, int[]> ReceivePhonePreviewData;

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
            TcpServer.Instance.SendData(data.ToArray());
        }

        /// <summary>
        /// 发送新棋步
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="boardState"></param>
        public void SendStepData(int x, int y, int[] boardState)
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(CommonDataDefine.ServerStepData));
            data.AddRange(BitConverter.GetBytes(boardState.Length + 4 + 2));//坐标和状态的总长度

            data.Add((byte)x);
            data.Add((byte)y);
            data.AddRange(BitConverter.GetBytes(boardState.Length));
            data.AddRange(boardState.Select(p => (byte)p).ToArray());
            TcpServer.Instance.SendData(data.ToArray());
        }

        public void SendScan()
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(CommonDataDefine.Scan));
            data.AddRange(BitConverter.GetBytes(0));
            TcpServer.Instance.SendData(data.ToArray());
        }

        public void SendPreview(bool isPreview)
        {
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(CommonDataDefine.SendPreview));
            data.AddRange(BitConverter.GetBytes(1));
            data.AddRange(BitConverter.GetBytes(isPreview));
            TcpServer.Instance.SendData(data.ToArray());
        }
        #endregion

        #region Phone->PC
        public void AnalysePhoneData(byte[] data)
        {
            int index = 0;
            int head = BitConverter.ToInt32(data, index); index += 4;
            int len = BitConverter.ToInt32(data, index); index += 4;
            if (head == CommonDataDefine.PhoneStepData)
            {
                int x = data[index]; index++;
                int y = data[index]; index++;
                int boardLen = BitConverter.ToInt32(data, index); index += 4;
                int[] boardState = new int[boardLen];
                for (int i = 0; i < boardState.Length; i++)
                {
                    boardState[i] = data[index]; index++;
                }
                ReceivePhoneStepData?.Invoke(x, y, boardState);
            }
            else if (head == CommonDataDefine.PreviewData)
            {
                //图像
                int imagelen = BitConverter.ToInt32(data, index); index += 4;
                byte[] image = new byte[imagelen];
                for (int i = 0; i < image.Length; i++)
                {
                    image[i] = data[index]; index++;
                }
                //识别成功与否
                bool isOk = data[index] == 1;
                //解析的棋盘数据
                int boardLen = BitConverter.ToInt32(data, index); index += 4;
                int[] boardState = new int[boardLen];
                for (int i = 0; i < boardState.Length; i++)
                {
                    boardState[i] = data[index]; index++;
                }
                ReceivePhonePreviewData?.Invoke(image, isOk, boardState);
            }
        }


        #endregion
    }
}
