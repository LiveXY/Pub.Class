using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Pub.Class;
using System.Threading;

namespace SendEmailTest {
    public partial class Form1 : Form {
        private string config1 = WebConfig.GetApp("config1") ?? "";
        private string config2 = WebConfig.GetApp("config2") ?? "";
        private string subject = WebConfig.GetApp("subject") ?? "";
        private string body = WebConfig.GetApp("body") ?? "";
        private SmtpServer server1;
        private SmtpServer server2;

        public Form1() {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e) {
            string[] s1 = config1.Split('|');
            string[] s2 = config2.Split('|');

            server1 = new SmtpServer() { Host = s1[0], Port = s1[1].ToInt(), From = s1[2], To = s1[3], UserName = s1[4], Passwrod = s1[5] };
            server2 = new SmtpServer() { Host = s2[0], Port = s2[1].ToInt(), From = s2[2], To = s2[3], UserName = s2[4], Passwrod = s2[5] };

            property1.SelectedObject = server1;
            property2.SelectedObject = server2;
        }

        private void button1_Click(object sender, EventArgs e) {
            string text = ((Button)sender).Text;
            string code = "SmtpClient";
            if (text.StartsWith("SmtpClient")) code = "SmtpClient";
            if (text.StartsWith("SmtpMail")) code = "SmtpMail";
            if (text.StartsWith("CDO.Message")) code = "CDOMessage";
            if (text.StartsWith("TcpClient")) code = "TcpClient";
            if (text.StartsWith("Blat")) code = "Blat";

            ((Button)sender).Enabled = false;
            new Thread(() => {
                var status = new Email("Pub.Class.Email.{0}.SendEmail,Pub.Class.Email.{0}".FormatWith(code))
                    .Server(server1.Host, server1.Port)
                    .From(server1.From)
                    .Body(body.FormatWith(text))
                    .Subject(subject.FormatWith(text))
                    .IsBodyHtml(false)
                    .Credentials(server1.UserName, server1.Passwrod)
                    .To(to => to.Add(server1.To))
                    .Send();
                MessageBox.Show(subject.FormatWith(text) + "发送{0}！".FormatWith(status ? "成功" : "失败"));
                ((Button)sender).Enabled = true;
            }).Start();
        }

        private void button2_Click(object sender, EventArgs e) {
            string text = ((Button)sender).Text;
            string code = "SmtpClient";
            if (text.StartsWith("SmtpClient")) code = "SmtpClient";
            if (text.StartsWith("SmtpMail")) code = "SmtpMail";
            if (text.StartsWith("CDO.Message")) code = "CDOMessage";
            if (text.StartsWith("TcpClient")) code = "TcpClient";
            if (text.StartsWith("Blat")) code = "Blat";

            ((Button)sender).Enabled = false;
            new Thread(() => {
                var status = new Email("Pub.Class.Email.{0}.SendEmail,Pub.Class.Email.{0}".FormatWith(code))
                    .Server(server2.Host, server2.Port)
                    .From(server2.From)
                    .Body(body.FormatWith(text))
                    .Subject(subject.FormatWith(text))
                    .IsBodyHtml(false)
                    .UseDefaultCredentials(true)
                    .To(to => to.Add(server2.To))
                    .Send();
                MessageBox.Show(subject.FormatWith(text) + "发送{0}！".FormatWith(status ? "成功" : "失败"));
                ((Button)sender).Enabled = true;
            }).Start();
        }
    }
    [DefaultPropertyAttribute("Insert")]
    public class SmtpServer {
        [CategoryAttribute("设置"), DefaultValueAttribute(true),]
        public string Host { get; set; }

        [CategoryAttribute("设置"), DefaultValueAttribute(true),]
        public int Port { get; set; }

        [CategoryAttribute("设置"), DefaultValueAttribute(true),]
        public string From { get; set; }

        [CategoryAttribute("设置"), DefaultValueAttribute(true),]
        public string To { get; set; }

        [CategoryAttribute("设置"), DefaultValueAttribute(false)]
        public string UserName { get; set; }

        [CategoryAttribute("设置"), DefaultValueAttribute(true)]
        public string Passwrod { get; set; }
    }
}
