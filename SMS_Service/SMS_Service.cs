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
        private Timer timer5;

        public SMS_Service()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            timer5 = new Timer();
            timer5.Interval = 3000; // every 5 Secs
            timer5.Elapsed += new ElapsedEventHandler(this.timer5_Tick);
            timer5.Enabled = true;
            
            Logger.WriteMessageLog(" Start Service");
            Logger.WriteMessageLog(" Start SMS Service");
        }


        private void timer5_Tick(object sender, ElapsedEventArgs e)
        {
           
            DatabaseTransFormerProcess.TransformDataBase();// انتقال داده های بین دو سرور
            SMSSenderProcess.NewTags();
            SMSSenderProcess.UpdateTagID();
            SMSSenderProcess.SMSSender();// پردازش . ارسال پیامک ها
        }
        protected override void OnStop()
        {
            timer5.Enabled = false;     
            Logger.WriteMessageLog("All Service Stopped");
        }
    }
}
