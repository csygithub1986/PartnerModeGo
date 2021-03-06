﻿using PartnerModeGo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PartnerModeGo
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            mainGrid.Children.Add(new HomePage());

            Application.Current.Resources.Add("BlackStoneColor", new SolidColorBrush(Properties.Settings.Default.BlackStoneColor));
            Application.Current.Resources.Add("WhiteStoneColor", new SolidColorBrush(Properties.Settings.Default.WhiteStoneColor));


        }

        public void ChangePageTo(UserControl uc)
        {
            mainGrid.Children.Clear();
            mainGrid.Children.Add(uc);
        }

        public void ShowProcessWindowAsync(String message, Func<Object[], Object> method, Action<Object> callback, params Object[] args)
        {
            Action action = new Action(() =>
            {
                this.Dispatcher.Invoke(new Action(() => { new WindowProcessing(this, message, method, callback, args).ShowDialog(); }));
                return;
            });
            action.BeginInvoke(null, null);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);
                FileInfo[] files = dir.GetFiles(".aitemp");
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i].FullName);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}