using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PartnerModeGo
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
            this.DispatcherUnhandledException += Current_DispatcherUnhandledException;
#endif
            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException -= this.CurrentDomain_UnhandledException;
            this.DispatcherUnhandledException -= Current_DispatcherUnhandledException;
            base.OnExit(e);
        }

        private void Current_DispatcherUnhandledException(Object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ClientLog.WriteLog(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
            MessageBox.Show("程序异常", "程序异常", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
        private void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            Exception exc = e.ExceptionObject as Exception;
            ClientLog.WriteLog(exc.Message + Environment.NewLine + exc.StackTrace);
            MessageBox.Show("程序异常", "程序异常", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
