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

namespace ToSwfService {
    [RunInstaller(true)]
    public partial class ToSwfServiceBase : ServiceBase {
        private int RunTime = (WebConfig.GetApp("RunTime") ?? "600").ToInt(600); //10分钟

        public ToSwfServiceBase() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            timer1.Interval = RunTime * 1000;
            timer1.Enabled = true;
            timer1_Elapsed(null, null);
            FileDirectory.FileWrite("ToSwfService.log".GetMapPath(), "[{0}] - ToSwfService服务已启动!".FormatWith(DateTime.Now.ToDateTime()));
        }

        protected override void OnStop() {
            this.timer1.Enabled = false;
            Safe.KillProcess("Pub.Class.ToSwf");
            FileDirectory.FileWrite("ToSwfService.log".GetMapPath(), "[{0}] - ToSwfService服务已关闭!".FormatWith(DateTime.Now.ToDateTime()));
        }

        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (Safe.IsExistProcess("Pub.Class.ToSwf") == 0) {
                Safe.RunAsync("Pub.Class.ToSwf.exe", "");
                FileDirectory.FileWrite("ToSwfService.log".GetMapPath(), "[{0}] - Pub.Class.ToSwf.exe程序已启动!".FormatWith(DateTime.Now.ToDateTime()));
            }
        }
    }
}