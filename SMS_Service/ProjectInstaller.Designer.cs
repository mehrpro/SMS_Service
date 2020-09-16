namespace SMS_Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.smsserviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.smsserviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // smsserviceProcessInstaller
            // 
            this.smsserviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.smsserviceProcessInstaller.Password = null;
            this.smsserviceProcessInstaller.Username = null;
            // 
            // smsserviceInstaller
            // 
            this.smsserviceInstaller.DelayedAutoStart = true;
            this.smsserviceInstaller.Description = "Copier MySQL";
            this.smsserviceInstaller.DisplayName = "CMSQL";
            this.smsserviceInstaller.ServiceName = "SMS_Service";
            this.smsserviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.smsserviceProcessInstaller,
            this.smsserviceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller smsserviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller smsserviceInstaller;
    }
}