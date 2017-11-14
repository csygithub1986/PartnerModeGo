using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PartnerModeGo
{
    /// <summary>
    /// PreviewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewWindow : Window
    {
        #region 单例
        private PreviewWindow()
        {
            InitializeComponent();
        }

        private static PreviewWindow _Instance;
        public static PreviewWindow Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new PreviewWindow();
                }
                return _Instance;
            }
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TcpServer.Instance.PhoneConnectedChanged += Instance_PhoneConnectedChanged;
            TcpServer.Instance.WindowReceivePhonePreviewData += Instance_WindowReceivePhonePreviewData;

            Task.Factory.StartNew(() =>
            {
                if (TcpServer.Instance.IsConnected)
                {
                    TcpServer.Instance.SendPreviewCommand(true);
                }
            });
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            TcpServer.Instance.PhoneConnectedChanged -= Instance_PhoneConnectedChanged;
            TcpServer.Instance.WindowReceivePhonePreviewData -= Instance_WindowReceivePhonePreviewData;
            Task.Factory.StartNew(() =>
            {
                if (TcpServer.Instance.IsConnected)
                {
                    TcpServer.Instance.SendPreviewCommand(false);
                }
            });
        }

        private void Instance_PhoneConnectedChanged(bool isConnected)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (isConnected)
                {
                    Task.Factory.StartNew(() => { TcpServer.Instance.SendPreviewCommand(true); });
                    lbState.Content = "未识别";
                    lbState.Background = Brushes.Red;
                }
                else
                {
                    lbState.Content = "未连接";
                    lbState.Background = Brushes.Gray;
                }
            }));
        }

        private void Instance_WindowReceivePhonePreviewData(byte[] imageData, int recognizedState, int[] boardState)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (recognizedState == 0)
                {
                    lbState.Content = "未识别";
                    lbState.Background = Brushes.Red;
                }
                else if (recognizedState == 1)
                {
                    lbState.Content = "状态不正确";
                    lbState.Background = Brushes.Orange;
                }
                else if (recognizedState == 2)
                {
                    lbState.Content = "状态正确";
                    lbState.Background = Brushes.ForestGreen;
                }
                imageOrigin.Source = ImageHelper.ByteArrayToBitmapImage(imageData);
            }));

        }

    }
}
