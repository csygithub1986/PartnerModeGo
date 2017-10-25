//using Ptlc.OccClient.Base;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace PartnerModeGo
{
    /// <summary>
    /// 操作等待窗口。
    /// </summary>
    internal partial class WindowProcessing : Window
    {
        #region 字段
        private Boolean m_isRunning;
        private TaskProcessor m_processor;
        private Func<Object[], Object> m_targetDelegate;
        private Action<Object> m_targetCallback;
        private Object[] m_targetParams;
        #endregion

        #region 属性
        /// <summary>
        /// 获取一个值，该值指示呈现等级，此属性为只读。
        /// </summary>
        public static Int32 PresentGrade
        {
            get
            {
                return 4;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 实例化 WindowProcessing 类的新实例。
        /// </summary>
        /// <param name="owner">父窗口。</param>
        /// <param name="message">显示的字符串。</param>
        /// <param name="method">异步执行的委托。</param>
        /// <param name="callback">异步执行的委托的回掉函数。</param>
        /// <param name="args">异步执行的委托的参数。</param>
        public WindowProcessing(Window owner, String message, Func<Object[], Object> method, Action<Object> callback, params Object[] args)
        {
            this.m_isRunning = false;
            this.m_processor = null;
            this.m_targetDelegate = method;
            this.m_targetCallback = callback;
            this.m_targetParams = args;
            this.InitializeComponent();
            this.Owner = owner;
            this.lblText.Content = message;
            this.lblProcessing.Visibility = Visibility.Hidden;
            this.btnCancel.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// 实例化 WindowProcessing 类的新实例。
        /// </summary>
        /// <param name="owner">父窗口。</param>
        /// <param name="message">显示的字符串。</param>
        /// <param name="method">异步执行的委托。</param>
        /// <param name="callback">异步执行的委托的回掉函数。</param>
        /// <param name="args">异步执行的委托的参数。</param>
        public WindowProcessing(Window owner, String message, TaskProcessor processor, Func<Object[], Object> method, Action<Object> callback, params Object[] args)
        {
            this.m_isRunning = false;
            this.m_processor = processor;
            this.m_targetDelegate = method;
            this.m_targetCallback = callback;
            this.m_targetParams = args;
            this.InitializeComponent();
            this.Owner = owner;
            this.lblText.Content = message;
            this.lblProcessing.Visibility = Visibility.Visible;
            this.btnCancel.Visibility = Visibility.Visible;
        }
        #endregion

        #region 事件函数
        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            if (this.m_targetDelegate != null)
            {
                this.m_targetDelegate.BeginInvoke(this.m_targetParams, new AsyncCallback(this.TaskCallbackMethod), null);
            }
            if (this.m_processor != null)
            {
                this.m_isRunning = true;
                new Action(this.TaskProcessingRefreshing).BeginInvoke(null, null);
            }
        }
        private void Window_Closing(Object sender, CancelEventArgs e)
        {
            this.m_isRunning = false;
        }
        private void Window_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            if (e.GetPosition(this).Y <= 80)
            {
                this.DragMove();
            }
        }
        private void btnCancel_Click(Object sender, RoutedEventArgs e)
        {
            if (this.m_processor != null)
            {
                this.m_processor.TaskCancel = true;
            }
            this.Close();
        }
        #endregion

        #region 公共函数
        /// <summary>
        /// 异步关闭等待窗口。
        /// </summary>
        public void CloseProcessing()
        {
            this.Dispatcher.Invoke(new Action(() => { this.Close(); }));
        }
        #endregion

        #region 私有函数
        private void TaskProcessingRefreshing()
        {
            while (this.m_isRunning)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.lblProcessing.Content = String.Format(CultureInfo.CurrentCulture, "{0}%", this.m_processor.TaskProgress);
                }));
                Thread.Sleep(200);
            }
        }
        private void TaskCallbackMethod(IAsyncResult asyncResult)
        {
            Object result = null;
            try
            {
                result = this.m_targetDelegate.EndInvoke(asyncResult);
            }
            catch (Exception ex)
            {
            }
            this.Dispatcher.Invoke(new Action(() => { this.Close(); }));
            if (this.m_targetCallback != null)
            {
                this.m_targetCallback(result);
            }
        }
        #endregion
    }


    /// <summary>
    /// 任务进程管理。
    /// </summary>
    public class TaskProcessor : System.ComponentModel.INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName) { if (PropertyChanged != null) { PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName)); } }
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置一个值，该值指示任务是否被取消。
        /// </summary>
        public Boolean TaskCancel { get { return m_TaskCancel; } set { if (m_TaskCancel != value) { m_TaskCancel = value; NotifyPropertyChanged("TaskCancel"); } } }
        private Boolean m_TaskCancel;
        /// <summary>
        /// 获取或设置一个值，该值指示任务进度，该值位于 0 和 100 之间。
        /// </summary>
        public Int32 TaskProgress { get { return m_TaskProgress; } set { if (m_TaskProgress != value) { m_TaskProgress = value; NotifyPropertyChanged("TaskProgress"); } } }
        private Int32 m_TaskProgress;
        #endregion

        #region 构造函数
        /// <summary>
        /// 实例化 TaskProcessor 类的新实例。
        /// </summary>
        public TaskProcessor()
        {
            this.TaskCancel = false;
            this.TaskProgress = 0;
        }
        #endregion
    }
}