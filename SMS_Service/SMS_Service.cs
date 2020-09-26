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
        private Timer timer2;
        public SMS_Service()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            timer5 = new Timer();
            timer5.Interval = 5000; // every 3 Secs
            timer5.Elapsed += new ElapsedEventHandler(this.timer5_Tick);
            timer5.Enabled = true;

            timer2 = new Timer();
            timer2.Interval = 2000; // every 2 Secs
            timer2.Elapsed += new ElapsedEventHandler(this.timer2_Tick);
            timer2.Enabled = true;

            Logger.WriteMessageLog(" Start Service");
            Logger.WriteMessageLog(" Start SMS Service");
        }

        private void timer2_Tick(object sender, ElapsedEventArgs e)
        {
            //var result = SMSSenderProcess.TagFinder();// ثبت تگ های جدید
            //if (result)
            //{
            //    SMSSenderProcess.DubleTagRemover();//حذف موارد تکراری و تعیین وضعیت 
            //}
           
        }

        private void timer5_Tick(object sender, ElapsedEventArgs e)
        {
            DatabaseTransFormerProcess.TransformDataBase();// انتقال داده های بین دو سرور
            SMSSenderProcess.SMSSender();// پردازش . ارسال پیامک ها
        }
        protected override void OnStop()
        {
            timer5.Enabled = false;
            timer2.Enabled = false;
            Logger.WriteMessageLog("All Service Stopped");
        }
    }
}
