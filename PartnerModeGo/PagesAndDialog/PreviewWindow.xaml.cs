using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

        //phone链接状态改变时
        private void Instance_PhoneConnectedChanged(bool isConnected)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (isConnected)
                {
                    Task.Factory.StartNew(() => { TcpServer.Instance.SendPreviewCommand(true); });
                    lbState.Content = RecognizeState.未识别.ToString();
                    lbState.Background = Brushes.Red;
                }
                else
                {
                    lbState.Content = "未连接";
                    lbState.Background = Brushes.Gray;
                }
            }));
        }

        //收到phone预览数据时
        private void Instance_WindowReceivePhonePreviewData(byte[] imageData, int recognizedState, int[] boardState)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                //显示状态
                lbState.Content = ((RecognizeState)recognizedState).ToString();
                if (recognizedState == 0)
                {
                    lbState.Background = Brushes.Red;
                }
                else if (recognizedState == 1)
                {
                    lbState.Background = Brushes.Orange;
                }
                else if (recognizedState == 2)
                {
                    lbState.Background = Brushes.ForestGreen;
                }
                //显示图像
                imageOrigin.Source = ImageHelper.ByteArrayToBitmapImage(imageData);
                //显示棋盘
                if (recognizedState!=0)
                {
                    goBoard.Visibility = Visibility.Visible;
                    goBoard.SetStones(boardState);
                }
                else
                {
                    goBoard.Visibility = Visibility.Hidden;
                }
            }));

        }

    }
}
