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
        private Timer _timer = null;
        public SMS_Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timer = new Timer();
            this._timer.Interval = 1000; // every 30 Secs
            this._timer.Elapsed += new System.Timers.ElapsedEventHandler(this._timer_Tick);
            _timer.Enabled = true;
            SvcProcess.WriteMessageLog("Start Service");
        }

        private void _timer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
               var result = SvcProcess.RegisterINSqlServer(SvcProcess.ReaderSQL());
               // send sms error Save
            }
            catch 
            {
               
                // send sms Error Process
            }
           
        }

        protected override void OnStop()
        {
            _timer.Enabled = false;
            SvcProcess.WriteMessageLog("Service Stopped");
        }
    }
}
