using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SMS_Service
{
    public partial class SMS_Service : ServiceBase
    {
        private Timer _timerTransformer;
        public SMS_Service()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            _timerTransformer = new Timer();
            _timerTransformer.Interval = 3000; // every 3 Secs
            _timerTransformer.Elapsed += new System.Timers.ElapsedEventHandler(this._timerTransFormer_Tick);
            _timerTransformer.Enabled = true;
            Logger.WriteMessageLog(" Start Service");
            Logger.WriteMessageLog(" Start SMS Service");
        }
        private void _timerTransFormer_Tick(object sender, ElapsedEventArgs e)
        {
            DatabaseTransFormerProcess.TransformDataBase();// انتقال داده های بین دو سرور
            SMSSenderProcess.SMSSender();// پردازش . ارسال پیامک ها
        }
        protected override void OnStop()
        {
            _timerTransformer.Enabled = false;
            Logger.WriteMessageLog("All Service Stopped");
        }
    }
}
