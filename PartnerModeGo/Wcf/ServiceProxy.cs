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

namespace PartnerModeGo.Wcf
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
        private ServiceProxy() { }
        #endregion

        private IWcfService m_wcfClient;

        public void ClientOpen()
        {
            String url = String.Format(CultureInfo.CurrentCulture, "net.tcp://localhost:12121/LeagueGoServer/WcfService/", "127.0.0.1", 12121);
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
            var client = new WcfServiceClient(new InstanceContext(new ServiceCallback()), tcpBinding, new EndpointAddress(url));
            //client.InnerDuplexChannel.Closed += (s, e) => m_isConnected = false;
            //client.InnerDuplexChannel.Faulted += (s, e) =>
            //{
            //    if (m_isConnected != false)
            //    {
            //        m_isConnected = false; this.OnServerConnectedChanged(new ServerConnectedChangedEventArgs(false, String.Empty));
            //    }
            //};
            m_wcfClient = client;
            Console.WriteLine("连接成功");
        }

        public void Login(object userName)
        {
            int result = m_wcfClient.Login(userName.ToString());

            Console.WriteLine("登录成功");
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

        public void ClientCommitMove(int stepNum, int x, int y)
        {
            m_wcfClient.ClientCommitMove(stepNum, x, y);
        }

    }
}
