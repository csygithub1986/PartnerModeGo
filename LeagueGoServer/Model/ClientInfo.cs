using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LeagueGoServer.Model
{
    /// <summary>
    /// 服务器内部使用
    /// 客户端连接类
    /// </summary>
    public class ClientInfo : System.ComponentModel.INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName) { if (PropertyChanged != null) { PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName)); } }
        #endregion
        ///// <summary>
        ///// 用户ID
        ///// </summary>
        //public int UserID { get { return m_UserID; } set { if (m_UserID != value) { m_UserID = value; NotifyPropertyChanged("UserID"); } } }
        //private int m_UserID;
        /// <summary>
        /// 客户端连接的SessionID
        /// </summary>
        public string SessionID { get { return m_SessionID; } set { if (m_SessionID != value) { m_SessionID = value; NotifyPropertyChanged("SessionID"); } } }
        private string m_SessionID;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get { return m_UserName; } set { if (m_UserName != value) { m_UserName = value; NotifyPropertyChanged("UserName"); } } }
        private string m_UserName;
        ///// <summary>
        ///// 主机标识
        ///// </summary>
        //public string HostIdentity { get { return m_HostIdentity; } set { if (m_HostIdentity != value) { m_HostIdentity = value; NotifyPropertyChanged("HostIdentity"); } } }
        //private string m_HostIdentity;
        /// <summary>
        /// 客户端回调使用
        /// </summary>
        public ICallback ClientCallback { get { return m_ClientCallback; } set { if (m_ClientCallback != value) { m_ClientCallback = value; NotifyPropertyChanged("ClientCallback"); } } }
        private ICallback m_ClientCallback;
        /// <summary>
        /// 客户端连接的通道
        /// </summary>
        public IContextChannel ClientChannel { get { return m_ClientChannel; } set { if (m_ClientChannel != value) { m_ClientChannel = value; NotifyPropertyChanged("ClientChannel"); } } }
        private IContextChannel m_ClientChannel;

        /// <summary>
        /// 心跳时间
        /// </summary>
        public DateTime HeartbeatTime { get { return m_StationID; } set { if (m_StationID != value) { m_StationID = value; NotifyPropertyChanged("StationID"); } } }
        private DateTime m_StationID;


        public ClientState PlayingState { get { return m_PlayingState; } set { if (m_PlayingState != value) { m_PlayingState = value; NotifyPropertyChanged("PlayingState"); } } }
        private ClientState m_PlayingState;
    }
}
