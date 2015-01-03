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
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using Pub.Class;
using System.Diagnostics;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif

namespace EmailService {
    [RunInstaller(true)]
    public partial class MService : ServiceBase {
        private IList<EmailList> emailList = new List<EmailList>();
        private Thread readEmailThread;
        private string[] strList = new string[] { "启动EmailService服务！", "EmailService 停止！" };
        private readonly ConfigInfo config = new ConfigInfo();
        private Email email;

        /// <summary>
        /// 初始化数据
        /// </summary>
        public MService() {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }
        /// <summary>
        /// 服务开始
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args) {
            WriteLog(strList[0]);

            config.LogPath = Pub.Class.WebConfig.GetApp("LogPath").GetMapPath().TrimEnd('\\') + "\\";
            config.SmtpServer = Pub.Class.WebConfig.GetApp("SmtpServer");
            config.UserName = Pub.Class.WebConfig.GetApp("UserName");
            config.Password = Pub.Class.WebConfig.GetApp("Password");
            config.FromAddress = Pub.Class.WebConfig.GetApp("FromAddress");
            config.AmountThread = Pub.Class.WebConfig.GetApp("AmountThread").ToInt(1).IfLessThan(1, 1);
            config.RecordCount = Pub.Class.WebConfig.GetApp("RecordCount").ToInt(5).IfLessThan(5, 5);
            config.TimeInterval = Pub.Class.WebConfig.GetApp("TimeInterval").ToInt(5).IfLessThan(5, 5);
            config.SmtpPort = Pub.Class.WebConfig.GetApp("SmtpPort").ToInt(25).IfLessThan(0, 25);
            config.IsBodyHtml = Pub.Class.WebConfig.GetApp("IsBodyHtml").ToBool(true);
            config.UseLog = Pub.Class.WebConfig.GetApp("UseLog").ToBool(true);
            config.Ssl = Pub.Class.WebConfig.GetApp("Ssl").ToBool(false);
            config.Retries = Pub.Class.WebConfig.GetApp("Retries").ToInt(1).IfLessThan(1, 1);
            config.Timeout = Pub.Class.WebConfig.GetApp("Timeout").ToInt(1000).IfLessThan(1000, 1000);
            config.ExpireDay = Pub.Class.WebConfig.GetApp("ExpireDay").ToInt(0).IfLessThan(0, 0);
            config.SelectListByTop = Pub.Class.WebConfig.GetApp("SelectListByTop");
            config.DeleteByIDList = Pub.Class.WebConfig.GetApp("DeleteByIDList");
            config.InsertSendHistry = Pub.Class.WebConfig.GetApp("InsertSendHistry");
            config.ClearExpireEmail = Pub.Class.WebConfig.GetApp("ClearExpireEmail").Replace("&lt;", "<");
            config.EmailProviderName = Pub.Class.WebConfig.GetApp("EmailProviderName");

            config.LogPath += "log\\";
            FileDirectory.DirectoryCreate(config.LogPath);
            config.LogPath += DateTime.Now.ToDate() + ".log";

            if (config.ExpireDay > 0) {
                WriteLog("开始清理{0}天前的过期邮件。".FormatWith(config.ExpireDay));
                WriteLog("清理{0}条记录成功。".FormatWith(ClearExpireEmail()));
            }

            if (config.EmailProviderName.IsNullEmpty())
                email = new Email(config.SmtpServer, config.SmtpPort);
            else
                email = new Email(config.EmailProviderName).Server(config.SmtpServer, config.SmtpPort);

            email = email.From(config.FromAddress)
                .IsBodyHtml(config.IsBodyHtml)
                .Ssl(config.Ssl)
                .Credentials(config.UserName, config.Password);

            readEmailThread = new Thread(new ThreadStart(ReadEmailList));
            readEmailThread.Name = "ReadDBThread";
            readEmailThread.Start();

            this.timer1.Interval = config.TimeInterval * 1000;
            this.timer1.Enabled = true;
        }
        /// <summary>
        /// 服务停止
        /// </summary>
        protected override void OnStop() {
            this.timer1.Enabled = false;
            this.timer1.Dispose();
            WriteLog(strList[1]);
        }
        /// <summary>
        /// 定时执行器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (readEmailThread.ThreadState == System.Threading.ThreadState.SuspendRequested || readEmailThread.ThreadState == System.Threading.ThreadState.Suspended) {
                WriteLog("线程激活。");
                readEmailThread.Resume();
            }
        }
        /// <summary>
        /// 读数据
        /// </summary>
        private void ReadEmailList() {
            while (true) {
                try {
                    WriteLog("开始读前{0}条记录。".FormatWith(config.RecordCount));
                    emailList = SelectListByTop(config.RecordCount);
                    WriteLog("读到{0}条记录成功。".FormatWith(emailList.Count));

                    if (emailList.IsNull() || emailList.Count == 0) {
                        if (readEmailThread.ThreadState == System.Threading.ThreadState.Running) {
                            WriteLog("线程挂起。");
                            readEmailThread.Suspend();
                        }
                    } else {
                        string idlist = emailList.Select(p => p.EmailID).Join<int?>(",");
                        if (!idlist.IsNullEmpty()) {
                            WriteLog("开始删除{0}记录。".FormatWith(idlist));
                            WriteLog("删除{0}条记录成功。".FormatWith(DeleteByIDList(idlist)));

                            foreach (var mail in emailList) {
                                string to = mail.Email;
                                string subject = mail.Subject;
                                string body = mail.Body;
                                if (SendEmail(to, subject, body)) {
                                    WriteLog("发送{0}成功。".FormatWith(to));
                                } else {
                                    WriteLog("发送{0}失败。".FormatWith(to));
                                    InsertSendHistry(new EmailList() { Email = to, Subject = subject, Body = body });
                                    WriteLog("发送失败记录已写入到历史表。");
                                }

                            }
                        }
                    }
                } catch (Exception ex) {
                    WriteLog("读数据错误：" + ex.ToExceptionDetail(), true);
                }
            }
        }
        /// <summary>
        /// 发送EMAIL
        /// </summary>
        /// <param name="to">接收email地址</param>
        /// <param name="subject">主题</param>
        /// <param name="body">内容</param>
        /// <returns></returns>
        public bool SendEmail(string to, string subject, string body) {
            if (!to.IsEmail()) return false;
            bool isTrue = false;
            ActionExtensions.Retry(() => {
                if (!email.ClearTo().Body(body).Subject(subject).To(t => t.Add(to)).Send()) {
                    WriteLog("{0}发送失败：{1}".FormatWith(to, email.ErrorMessage), true);
                } else isTrue = true;
            }, config.Retries, config.Timeout, false, ex => {
                WriteLog("{0}发送失败：{1}".FormatWith(to, ex.ToExceptionDetail()), true);
            });
            return isTrue;
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message"></param>
        public void WriteLog(string message, bool iswrite = false) {
            if (config.UseLog || iswrite) {
                FileDirectory.FileWrite(config.LogPath, DateTime.Now.ToDateTime() + " - " + message);
            }
        }
        /// <summary>
        /// 取前多少条记录
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public IList<EmailList> SelectListByTop(int top) {
            string strSql = config.SelectListByTop.FormatWith(top);
            WriteLog(strSql);
            return Data.GetDbDataReader(strSql).ToList<EmailList>();
        }
        /// <summary>
        /// 按ID批量删除数据
        /// </summary>
        /// <param name="idlist">idlist</param>
        /// <returns></returns>
        public int DeleteByIDList(string idlist) {
            if (idlist.IsNullEmpty()) return 0;
            string strSql = config.DeleteByIDList.FormatWith(idlist);
            WriteLog(strSql);
            return Data.ExecSql(strSql);
        }
        /// <summary>
        /// 发送失败的记录存入EmailSendHistory表
        /// </summary>
        /// <param name="mail">EmailList</param>
        /// <returns></returns>
        public bool InsertSendHistry(EmailList mail) {
            string strSql = config.InsertSendHistry.FormatWith(mail.Email, mail.Body, mail.Subject);
            WriteLog(strSql);
            return Data.ExecSql(strSql) > 0 ? true : false;
        }
        /// <summary>
        /// 清理过期邮件
        /// </summary>
        /// <returns></returns>
        public int ClearExpireEmail() {
            string strSql = config.ClearExpireEmail.FormatWith(DateTime.Now.AddDays(-config.ExpireDay).ToDateTime());
            WriteLog(strSql);
            return Data.ExecSql(strSql);
        }
    }
    /// <summary>
    /// 邮件实体类
    /// </summary>
    public class EmailList {
        private int? emailID = null;
        /// <summary>
        /// 邮件编号
        /// </summary>
        public new int? EmailID { get { return emailID; } set { emailID = value; } }
        private string email = null;
        /// <summary>
        /// 邮件地址
        /// </summary>
        public new string Email { get { return email; } set { email = value; } }
        private string subject = null;
        /// <summary>
        /// 邮件主题
        /// </summary>
        public new string Subject { get { return subject; } set { subject = value; } }
        private string body = null;
        /// <summary>
        /// 邮件正文
        /// </summary>
        public new string Body { get { return body; } set { body = value; } }
    }
    /// <summary>
    /// Web.config 配置信息
    /// </summary>
    public class ConfigInfo {
        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string LogPath { get; set; }
        /// <summary>
        /// smtp服务器地址
        /// </summary>
        public string SmtpServer { get; set; }
        /// <summary>
        /// smtp账号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// smtp密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 发件人地址
        /// </summary>
        public string FromAddress { get; set; }
        /// <summary>
        /// 线程数
        /// </summary>
        public int AmountThread { get; set; }
        /// <summary>
        /// 每次读记录数
        /// </summary>
        public int RecordCount { get; set; }
        /// <summary>
        /// 时间间隔
        /// </summary>
        public int TimeInterval { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int SmtpPort { get; set; }
        /// <summary>
        /// 以HTML发送
        /// </summary>
        public bool IsBodyHtml { get; set; }
        /// <summary>
        /// Ssl
        /// </summary>
        public bool Ssl { get; set; }
        /// <summary>
        /// 开启日志
        /// </summary>
        public bool UseLog { get; set; }
        /// <summary>
        /// 重发次数
        /// </summary>
        public int Retries { get; set; }
        /// <summary>
        /// 重发延时（毫秒）
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 只发送最近几天的邮件，删除过期的邮件
        /// </summary>
        public int ExpireDay { get; set; }
        public string SelectListByTop { get; set; }
        public string DeleteByIDList { get; set; }
        public string InsertSendHistry { get; set; }
        public string ClearExpireEmail { get; set; }
        public string EmailProviderName { get; set; }
    }
}