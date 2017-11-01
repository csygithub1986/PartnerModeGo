using PartnerModeGo.WcfService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PartnerModeGo
{
    public class ServiceProxy
    {
        #region 单例
        private static ServiceProxy m_Instance = null;
        public static ServiceProxy Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new ServiceProxy();
                }
                return m_Instance;
            }
        }
        private ServiceProxy()
        {
            Session = new Session();
        }
        #endregion

        #region Callback委托
        public Action<bool, string, int> JoinGameCallback;//加入棋局
        public Action<int[], int[], int> GameStartCallback;   //游戏开始
        public Action<GameDistributeType, Game> DistributeGameInfoCallback;
        public Action<int, int, int, int, int> MoveCallback;

        #endregion
        public Session Session { get; set; }

        private IWcfService m_wcfClient;

        public bool ClientOpen(string ip)
        {
            String url = String.Format(CultureInfo.CurrentCulture, "net.tcp://{0}:{1}/LeagueGoServer/WcfService/", ip, 12121);
            NetTcpBinding tcpBinding = new NetTcpBinding()
            {
                Name = "netTcpBindConfig",
                ReaderQuotas = XmlDictionaryReaderQuotas.Max,
                ListenBacklog = Int32.MaxValue,

                MaxBufferPoolSize = Int32.MaxValue,
                MaxConnections = 10,
                MaxBufferSize = Int32.MaxValue,
                MaxReceivedMessageSize = Int32.MaxValue,
                PortSharingEnabled = true,
                CloseTimeout = TimeSpan.FromSeconds(10),
                OpenTimeout = TimeSpan.FromSeconds(10),
                ReceiveTimeout = TimeSpan.FromMinutes(10),
                SendTimeout = TimeSpan.FromMinutes(10),
                TransactionFlow = false,
                TransferMode = TransferMode.Buffered,
                TransactionProtocol = TransactionProtocol.OleTransactions,
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                ReliableSession = new OptionalReliableSession
                {
                    Enabled = false,
                    Ordered = true,
                    InactivityTimeout = TimeSpan.FromMinutes(10),
                },
                Security = new NetTcpSecurity
                {
                    Mode = SecurityMode.None,
                    Message = new MessageSecurityOverTcp
                    {
                        ClientCredentialType = MessageCredentialType.Windows,
                    },
                    Transport = new TcpTransportSecurity
                    {
                        ClientCredentialType = TcpClientCredentialType.Windows,
                        ProtectionLevel = ProtectionLevel.EncryptAndSign
                    }
                }
            };
            //this.m_instanceContext = new InstanceContext(OccClientServicesCallBack.Instance);
            //this.m_duplexChannel = new DuplexChannelFactory<IPtlcWcfService>(this.m_instanceContext, tcpBinding);
            //this.m_wcfClient = this.m_duplexChannel.CreateChannel(new EndpointAddress(url));

            try
            {
                var client = new WcfServiceClient(new InstanceContext(new ServiceCallback()), tcpBinding, new EndpointAddress(url));
                if (client == null)
                {
                    return false;
                }
                m_wcfClient = client;
                client.InnerDuplexChannel.Closed += InnerDuplexChannel_Closed;
                client.InnerDuplexChannel.Faulted += InnerDuplexChannel_Faulted;
                Console.WriteLine("连接成功");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            //client.InnerDuplexChannel.Closed += (s, e) => m_isConnected = false;
            //client.InnerDuplexChannel.Faulted += (s, e) =>
            //{
            //    if (m_isConnected != false)
            //    {
            //        m_isConnected = false; this.OnServerConnectedChanged(new ServerConnectedChangedEventArgs(false, String.Empty));
            //    }
            //};
        }

        private void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            Console.WriteLine("客户端：  InnerDuplexChannel_Faulted " + DateTime.Now.ToString("mm-ss-fff"));
        }

        private void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("客户端：  InnerDuplexChannel_Closed " + DateTime.Now.ToString("mm-ss-fff"));
        }

        public string Login(object userName)
        {
            string ssid = m_wcfClient.Login(userName.ToString());
            return ssid;
        }

        public void GetAllGames()
        {
            Console.WriteLine("客户端：向server请求getallgame " + DateTime.Now.ToString("mm-ss-fff"));
            m_wcfClient.GetAllGames();
            Console.WriteLine("客户端：向server请求getallgame 返回" + DateTime.Now.ToString("mm-ss-fff"));
        }

        public void GameStart()
        {
            m_wcfClient.GameStart();
        }

        public void CreateGame(Player[] players, GameSetting setting)
        {
            m_wcfClient.CreateGame(players, setting);
        }

        public void ApplyToJoinGame(string gameID, int playerID)
        {
            m_wcfClient.ApplyToJoinGame(gameID, playerID);
        }

        public void ClientCommitMove(string gameID, int stepNum, int x, int y)
        {
            Console.WriteLine("发送自己下的到Server");

            //try
            //{
            m_wcfClient.ClientCommitMove(gameID, stepNum, x, y);
            //}
            //catch (Exception ex)
            //{

            //}
        }

    }
}
