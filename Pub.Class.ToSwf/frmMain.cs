using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.ServiceModel;
using Pub.Class;
using System.Threading;

namespace Pub.Class.ToSwf {
    public partial class frmMain : Form {
        private int isrun_index = 0;
        private Thread thread;
        const int WM_QUERYENDSESSION = 0x0011;
        ServiceHost serviceHost = null;
        private int ErrorTimer = (WebConfig.GetApp("ErrorTimer") ?? "360").ToInt(6);

        public frmMain() {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e) {
            ContextMenu notifyiconMnu;
            MenuItem[] mnuItms = new MenuItem[3];
            mnuItms[0] = new MenuItem();
            mnuItms[0].Text = "打开DocToSwf/VideoToFlv服务";
            mnuItms[0].Click += new System.EventHandler(this.NotifyIcon_DoubleClick);
            mnuItms[0].DefaultItem = true;

            mnuItms[1] = new MenuItem();
            mnuItms[1].Text = "-";

            mnuItms[2] = new MenuItem();
            mnuItms[2].Text = "退出系统";
            mnuItms[2].Click += new System.EventHandler(this.ExitSelect);
            notifyiconMnu = new ContextMenu(mnuItms);
            NotifyIcon.ContextMenu = notifyiconMnu;
            NotifyIcon.Visible = true;
            NotifyIcon.Icon = new Icon(Application.StartupPath + "\\icon.ico");
            this.Icon = new Icon(Application.StartupPath + "\\icon.ico");
            NotifyIcon.Text = this.Text;

            this.Hide();
            InitAutoRun(0);

            try {
                ToSwfWCF.ToSwfBase.OnNewOrDelete += new EventHandler(ToSwf_OnNewOrDelete);
                serviceHost = new ServiceHost(typeof(ToSwfWCF.ToSwfService));
                serviceHost.Open();

                timer1.Interval = 5000;
                timer1.Enabled = true;

                listBox1.Items.Add("启动服务成功！");
            } catch (Exception ex) {
                listBox1.Items.Add("启动服务失败 - " + ex.Message.ToString());
            }
        }
        private void ToSwf_OnNewOrDelete(object sender, EventArgs e) {
            listBox1.Items.Add(ToSwfWCF.ToSwfBase.Url);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }
        private void NotifyIcon_DoubleClick(object sender, EventArgs e) {
            this.Hide();
            this.WindowState = FormWindowState.Normal;
            this.Show();
        }
        public void ExitSelect(object sender, System.EventArgs e) {
            NotifyIcon.Visible = false;
            this.Close();
            Environment.Exit(1);
        }
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case WM_QUERYENDSESSION:
                    this.Close();
                    this.Dispose();
                    Application.Exit();
                    break;
                default:
                    break;
            }
            base.WndProc(ref   m);
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            this.Hide();
        }
        private void frmMain_Resize(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Minimized) {
                this.WindowState = FormWindowState.Normal;
                this.Hide();
            }
        }

        private void InitAutoRun(int iType) {
            RegistryKey keyCon = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string myKey = "DocToSwf";
            string myPath = Application.StartupPath + "\\" + @System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";

            switch (iType) {
                case 0:
                    if ((string)keyCon.GetValue(myKey, "no") == "no") chkAutoRun.Checked = false; else chkAutoRun.Checked = true;
                    break;
                case 1://set
                    if ((string)keyCon.GetValue(myKey, "no") == "no") {
                        string Path = myPath;
                        keyCon.SetValue(myKey, Path);
                    }
                    break;
                case 2://del
                    if ((string)keyCon.GetValue(myKey, "no") != "no") {
                        string Path = myPath;
                        keyCon.DeleteValue(myKey);
                    }
                    break;
            }
        }
        private void chkAutoRun_CheckedChanged(object sender, EventArgs e) {
            if (chkAutoRun.Checked) { InitAutoRun(1); } else { InitAutoRun(2); }
        }
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e) {
            IDisposable disposible = serviceHost as IDisposable;
            if (disposible != null) disposible.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (ToSwfWCF.ToSwfBase.IsRun) {
                isrun_index++;
                //listBox1.Items.Add(isrun_index + "|" + ErrorTimer);
                if (isrun_index > ErrorTimer) {
                    //listBox1.Items.Add("Kill FlashPrinter/EXCEL/WINWORD/POWERPNT/FoxitReader");
                    Safe.KillProcess("FlashPrinter");
                    Safe.KillProcess("EXCEL");
                    Safe.KillProcess("WINWORD");
                    Safe.KillProcess("POWERPNT");
                    Safe.KillProcess("FoxitReader");
                    Safe.KillProcess("pdf2swf");
                    Safe.KillProcess("office2pdf");
                    Safe.KillProcess("pdfbg");
                    isrun_index = 0;
                }
            } else {
                if (isrun_index > 0) listBox1.Items.Add("转换用时：" + (isrun_index * 5).ToTime());
                isrun_index = 0;
            }
            thread = new Thread(new ThreadStart(ToSwfWCF.ToSwfBase.DoRun));
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
