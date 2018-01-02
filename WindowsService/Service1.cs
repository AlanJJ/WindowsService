using System;
using System.ServiceProcess;
using System.Threading;

namespace WindowsService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        Thread _serviceThread = null;

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            _serviceThread = new Thread(WritenLog)
            {
                IsBackground = true
            };
            _serviceThread.Start();
        }

        /// <summary>
        /// 终止服务
        /// </summary>
        protected override void OnStop()
        {
            _serviceThread.Abort();
        }

        const string LogPath = @"D:\ServiceLog\Log.txt";
        readonly LogHelper _serviceLogHelper = new LogHelper(LogPath);
        /// <summary>
        /// 记日志
        /// </summary>
        private void WritenLog()
        {
            while (true)
            {
                string logText = string.Format("现在时间是：{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                Thread.Sleep(10000);//线程休眠10s
                _serviceLogHelper.WriteLine(logText);
            }
        }
    }
}
