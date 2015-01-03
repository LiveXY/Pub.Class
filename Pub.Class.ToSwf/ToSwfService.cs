using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Collections;
using System.Threading;
using System.Xml;
using System.IO;
using System.Net.Mail;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using Pub.Class;
using System.Text;
using System.Diagnostics;
using System.ServiceModel;
using Pub.Class.ToSwfWCF;

namespace Pub.Class.ToSwf {
    [RunInstaller(true)]
    public partial class ToSwfServiceBase : ServiceBase {
        private int isrun_index = 0;
        private Thread thread;
        ServiceHost serviceHost = null;
        private int ErrorTimer = (WebConfig.GetApp("ErrorTimer") ?? "360").ToInt(6);
        private string[] strList = new string[] { "ToSwfServiceBase 启动成功！", "ToSwfServiceBase 停止！" };

        public ToSwfServiceBase() {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            if (!System.Diagnostics.EventLog.SourceExists("ToSwfServiceBase")) System.Diagnostics.EventLog.CreateEventSource("ToSwfServiceBase", "ToSwfServiceBaseLog");
            this.eventLog1.Source = "ToSwfServiceBase";
            this.eventLog1.Log = "ToSwfServiceBaseLog";
        }

        private void ToSwf_OnNewOrDelete(object sender, EventArgs e) {
            if (ToSwfWCF.ToSwfBase.Url.IndexOf("计数") >= 0) return;
            this.eventLog1.WriteEntry(ToSwfWCF.ToSwfBase.Url, System.Diagnostics.EventLogEntryType.SuccessAudit);
        }

        protected override void OnStart(string[] args) {
            try {
                this.eventLog1.WriteEntry(strList[0], System.Diagnostics.EventLogEntryType.SuccessAudit);

                ToSwfWCF.ToSwfBase.OnNewOrDelete += new EventHandler(ToSwf_OnNewOrDelete);
                if (serviceHost == null) {
                    serviceHost = new ServiceHost(typeof(ToSwfService));
                    serviceHost.Open();
                }
                timer1.Interval = 5000;
                timer1.Enabled = true;
            } catch (Exception e) {
                this.eventLog1.WriteEntry(e.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
        }

        protected override void OnStop() {
            if (serviceHost != null) {
                IDisposable disposible = serviceHost as IDisposable;
                if (disposible != null) disposible.Dispose();
            }

            this.eventLog1.WriteEntry(strList[1], System.Diagnostics.EventLogEntryType.SuccessAudit);
            
            this.timer1.Enabled = false;
            GC.Collect();
        }

        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (ToSwfWCF.ToSwfBase.IsRun) { 
                isrun_index++;
                if (isrun_index > ErrorTimer) {
                    this.eventLog1.WriteEntry("KillProcess FlashPrinter/EXCEL/WINWORD/POWERPNT/FoxitReader", System.Diagnostics.EventLogEntryType.SuccessAudit);
                    Safe.KillProcess("FlashPrinter");
                    Safe.KillProcess("EXCEL");
                    Safe.KillProcess("WINWORD");
                    Safe.KillProcess("POWERPNT");
                    Safe.KillProcess("FoxitReader");
                    isrun_index = 0;
                }
            } else isrun_index = 0;
            thread = new Thread(new ThreadStart(ToSwfWCF.ToSwfBase.DoRun)); 
            thread.IsBackground = true;
            thread.Start();
        }
    }
}