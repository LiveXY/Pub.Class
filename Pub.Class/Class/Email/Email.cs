//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2011 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Text;
using System.Net.Mime;
using System.Net;

namespace Pub.Class {
    /// <summary>
    /// 发送EMAIL
    /// 
    /// 修改纪录
    ///     2012.02.20 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// var email = new Email("Pub.Class.SmtpClient.SendEmail,Pub.Class.SmtpClient")
    ///     .Host("smtp.163.com").Port(25)
    ///     .From("熊华春", "cexo255@163.com")
    ///     .Body("测试数据")
    ///     .Subject("测试")
    ///     .IsBodyHtml(true)
    ///     .Credentials("cexo255@163.com", "cexo851029")
    ///     .To(to => to.Add("hcxiong@elibiz.com"))
    ///     .Cc(cc => cc.Add("cexo255@163.com"))
    ///     .Send();
    /// </code>
    /// </example>
    /// </summary>
    public class Email : Disposable {
        private readonly IEmail email = null;
        private readonly bool isSend = false;
        private readonly SmtpClient smtpClient = null;
        private readonly MailMessage message = null;
        private readonly IList<LinkedResource> linkeds = null;
        /// <summary>
        /// 构造器 指定DLL文件和全类名
        /// </summary>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">命名空间.类名</param>
        public Email(string dllFileName, string className) {
            errorMessage = string.Empty;
            if (email.IsNull()) {
                email = (IEmail)dllFileName.LoadClass(className);
                smtpClient = new SmtpClient();
                message = new MailMessage();
                linkeds = new List<LinkedResource>();
            }
        }
        /// <summary>
        /// 构造器 指定classNameDllName(EmailProviderName) 默认Pub.Class.SmtpClient.SendEmail,Pub.Class.SmtpClient
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        public Email(string classNameAndAssembly) {
            errorMessage = string.Empty;
            if (email.IsNull()) {
                email = (IEmail)classNameAndAssembly.IfNullOrEmpty("Pub.Class.Email.SmtpClient.SendEmail,Pub.Class.Email.SmtpClient").LoadClass();
                smtpClient = new SmtpClient();
                message = new MailMessage();
                linkeds = new List<LinkedResource>();
            }
        }
        /// <summary>
        /// 构造器 从Web.config中读EmailProviderName 默认Pub.Class.SmtpClient.SendEmail,Pub.Class.SmtpClient
        /// </summary>
        public Email() {
            errorMessage = string.Empty;
            if (email.IsNull()) {
                email = (IEmail)(WebConfig.GetApp("EmailProviderName") ?? "Pub.Class.Email.SmtpClient.SendEmail,Pub.Class.Email.SmtpClient").LoadClass();
                smtpClient = new SmtpClient();
                message = new MailMessage();
                linkeds = new List<LinkedResource>();
            }
        }
        /// <summary>
        /// 构造器 直接发送 不需要指定EmailProviderName或classNameDllName
        /// </summary>
        /// <param name="host">SMTP 事务的主机的名称或 IP 地址</param>
        /// <param name="port">host 上使用的端口</param>
        public Email(string host, int port) {
            errorMessage = string.Empty;
            smtpClient = new SmtpClient(host, port);
            message = new MailMessage();
            linkeds = new List<LinkedResource>();
            isSend = true;
        }
        /// <summary>
        /// 用using 自动释放
        /// </summary>
        protected override void InternalDispose() {
            base.InternalDispose();
        }
        ///<summary>
        /// 实例化MailMessage
        ///</summary>
        public MailMessage Message {
            get { return message; }
        }
        ///<summary>
        /// From显示名称和地址
        ///</summary>
        ///<param name="fromMail">From显示地址</param>
        ///<returns>Email(this)</returns>
        public virtual Email From(string fromMail) {
            Message.From = new MailAddress(fromMail);
            return this;
        }
        ///<summary>
        /// From显示名称和地址
        ///</summary>
        ///<param name="fromDisplayName">From显示名称</param>
        ///<param name="fromMail">From显示地址</param>
        ///<returns>Email(this)</returns>
        public virtual Email From(string fromDisplayName, string fromMail) {
            Message.From = new MailAddress(fromMail, fromDisplayName);
            return this;
        }
        ///<summary>
        /// 添加发送邮件地址
        ///</summary>
        ///<param name="mailAddresses">邮件地址</param>
        ///<returns>Email(this)</returns>
        public virtual Email To(Func<MailAddresses, MailAddresses> mailAddresses) {
            foreach (var address in mailAddresses(new MailAddresses()).AddressCollection)
                Message.To.Add(address);

            return this;
        }
        /// <summary>
        /// 移除所有发送的EMAIL地址
        /// </summary>
        /// <returns></returns>
        public virtual Email ClearTo() {
            Message.To.Clear();
            return this;
        }
        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="filePath">附件路径</param>
        /// <returns>Email(this)</returns>
        public virtual Email AddAttachment(string filePath) {
            Attachment data = new Attachment(filePath, MediaTypeNames.Application.Octet);
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(filePath);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(filePath);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(filePath);
            Message.Attachments.Add(data);
            return this;
        }
        ///<summary>
        /// 添加发送邮件地址
        ///</summary>
        ///<param name="mailAddresses">邮件地址</param>
        ///<returns>Email(this)</returns>
        public virtual Email Cc(Func<MailAddresses, MailAddresses> mailAddresses) {
            foreach (var address in mailAddresses(new MailAddresses()).AddressCollection)
                Message.CC.Add(address);

            return this;
        }
        /// <summary>
        /// 移除所有抄送的EMAIL地址
        /// </summary>
        /// <returns></returns>
        public virtual Email ClearCc() {
            Message.CC.Clear();
            return this;
        }
        ///<summary>
        /// 添加发送邮件地址
        ///</summary>
        ///<param name="mailAddresses">邮件地址</param>
        ///<returns>Email(this)</returns>
        public virtual Email Bcc(Func<MailAddresses, MailAddresses> mailAddresses) {
            foreach (var address in mailAddresses(new MailAddresses()).AddressCollection)
                Message.Bcc.Add(address);

            return this;
        }
        public virtual Email AddLinkedResource(string fileName, string contentType, string cid) { 
            LinkedResource link = new System.Net.Mail.LinkedResource(fileName, contentType);
            link.ContentId = cid;
            linkeds.Add(link);
            return this;
        }
        public virtual Email AddImageResource(string fileName, string cid) {
            return AddLinkedResource(fileName, "image/gif", cid);
        }
        /// <summary>
        /// 移除所有密件抄送的EMAIL地址
        /// </summary>
        /// <returns></returns>
        public virtual Email ClearBcc() {
            Message.Bcc.Clear();
            return this;
        }
        /// <summary>
        /// 移除所有To/Cc/Bcc的EMAIL地址
        /// </summary>
        /// <returns></returns>
        public virtual Email Clear() {
            ClearBcc();
            ClearCc();
            ClearTo();
            return this;
        }
        ///<summary>
        /// 发送标题
        ///</summary>
        ///<param name="subject">标题</param>
        ///<returns>Email(this)</returns>
        public virtual Email Subject(string subject) {
            Message.Subject = subject;
            return this;
        }
        ///<summary>
        /// 发送内容
        ///</summary>
        ///<param name="body">内容</param>
        ///<returns>Email(this)</returns>
        public virtual Email Body(string body) {
            Message.Body = body;
            return this;
        }
        ///<summary>
        /// 发送标题编码
        ///</summary>
        ///<param name="subjectEncoding">标题编码</param>
        ///<returns>Email(this)</returns>
        public virtual Email SubjectEncoding(Encoding subjectEncoding) {
            Message.SubjectEncoding = subjectEncoding;
            return this;
        }
        ///<summary>
        /// 发送内容编码
        ///</summary>
        ///<param name="bodyEncoding">内容编码</param>
        ///<returns>Email(this)</returns>
        public virtual Email BodyEncoding(Encoding bodyEncoding) {
            Message.BodyEncoding = bodyEncoding;
            return this;
        }
        public virtual Email Priority(MailPriority mailPriority) {
            Message.Priority = mailPriority;
            return this;
        }
        ///<summary>
        /// 内容是否以HTML形式发送
        ///</summary>
        ///<param name="isBodyHtml">是否以HTML形式发送 true/false</param>
        ///<returns>Email(this)</returns>
        public virtual Email IsBodyHtml(bool isBodyHtml) {
            Message.IsBodyHtml = isBodyHtml;
            return this;
        }
#if !MONO40
        ///<summary>
        /// 如果使用默认凭据，则为 true；否则为 false。默认值为 false。
        ///</summary>
        ///<param name="useDefaultCredentials">如果使用默认凭据，则为 true；否则为 false。默认值为 true。</param>
        ///<returns>Email(this)</returns>
        public virtual Email UseDefaultCredentials(bool useDefaultCredentials = true) {
            smtpClient.UseDefaultCredentials = useDefaultCredentials;
            return this;
        }
#endif
        ///<summary>
        /// 邮件服务器主机
        ///</summary>
        ///<param name="host">邮件服务器主机</param>
        ///<returns>Email(this)</returns>
        public virtual Email Host(string host) {
            smtpClient.Host = host;
            return this;
        }
        ///<summary>
        /// 邮件服务器
        ///</summary>
        ///<param name="host">邮件服务器主机</param>
        ///<param name="port">指定的端口</param>
        ///<returns>Email(this)</returns>
        public virtual Email Server(string host, int port = 25) {
            smtpClient.Host = host;
            smtpClient.Port = port;
            return this;
        }
        ///<summary>
        /// 指定的端口
        ///</summary>
        ///<param name="port">指定的端口</param>
        ///<returns>Email(this)</returns>
        public virtual Email Port(int port) {
            smtpClient.Port = port;
            return this;
        }
        ///<summary>
        /// 是否启用SSL
        ///</summary>
        ///<param name="enableSsl">是否启用SSL</param>
        ///<returns>Email(this)</returns>
        public virtual Email Ssl(bool enableSsl) {
            smtpClient.EnableSsl = enableSsl;
            return this;
        }
        ///<summary>
        /// 超时时间
        ///</summary>
        ///<param name="timeout">超时时间</param>
        ///<returns>Email(this)</returns>
        public virtual Email Timeout(int timeout) {
            smtpClient.Timeout = timeout;
            return this;
        }
        ///<summary>
        /// 定义要使用的凭据发送邮件（NetworkCredentials）
        ///</summary>
        ///<param name="username">用户名</param>
        ///<param name="password">密码</param>
        ///<returns>Email(this)</returns>
        public virtual Email Credentials(string username, string password) {
#if !MONO40
            if (username.IsNullEmpty() || password.IsNullEmpty()) UseDefaultCredentials(true);
            else smtpClient.Credentials = new NetworkCredential(username, password);
#else
            smtpClient.Credentials = new NetworkCredential(username, password);
#endif
            return this;
        }
        ///<summary>
        /// 定义要使用的凭据发送邮件（NetworkCredentials）
        ///</summary>
        ///<param name="username">用户名</param>
        ///<param name="password">密码</param>
        ///<param name="domain">域</param>
        ///<returns>Email(this)</returns>
        public virtual Email Credentials(string username, string password, string domain) {
            smtpClient.Credentials = new NetworkCredential(username, password, domain);
            return this;
        }
        ///<summary>
        /// 发送
        ///</summary>
        ///<example>
        /// <code>
        ///     var Email = new Email("smtp.gmail.com", 587);
        ///     Email
        ///         .From("Andre Carrilho", "me@mymail.com")
        ///         .To(to => to.Add("Andre Carrilho", "anotherme@mymail.com"))
        ///         .Bcc(bcc => bcc.Add(mailsWithDisplayNames))
        ///         .Cc(cc => cc.Add(justMails))
        ///         .Body("Trying out the Email class with some Html: &lt;p style='font-weight:bold;color:blue;font-size:32px;'>html&lt;/p>")
        ///         .Subject("Testing Fluent Email")
        ///         .IsBodyHtml(true)
        ///         .Credentials("someUser", "somePass")
        ///         .Port(1234)
        ///         .Ssl(true)
        ///         .Send();
        /// </code>
        ///</example>
        ///<returns>true/false</returns>
        public bool Send() {
            if (linkeds.Count > 0) {
                AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(Message.Body, null, "text/html");
                foreach(var info in linkeds) htmlBody.LinkedResources.Add(info);
                Message.AlternateViews.Add(htmlBody);
                linkeds.Clear();
            }
            errorMessage = string.Empty;
            if (isSend) {
                try {
                    smtpClient.Send(message);
                    return true;
                } catch (Exception ex) {
                    errorMessage = ex.ToExceptionDetail();
                    return false;
                }
            }
            bool isTrue = email.Send(message, smtpClient);
            if (!isTrue) errorMessage = email.ErrorMessage;
            return isTrue;
        }
        public bool SendAsync(object state) {
            if (linkeds.Count > 0) {
                AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(Message.Body, null, "text/html");
                foreach (var info in linkeds) htmlBody.LinkedResources.Add(info);
                Message.AlternateViews.Add(htmlBody);
                linkeds.Clear();
            }
            errorMessage = string.Empty;
            if (isSend) {
                try {
                    smtpClient.SendAsync(message, state);
                    return true;
                } catch (Exception ex) {
                    errorMessage = ex.ToExceptionDetail();
                    return false;
                }
            }
            bool isTrue = email.Send(message, smtpClient);
            if (!isTrue) errorMessage = email.ErrorMessage;
            return isTrue;
        }
        ///<summary>
        /// MailAddresses类
        ///</summary>
        public class MailAddresses {
            private readonly MailAddressCollection addressCollection = null;
            internal MailAddressCollection AddressCollection { get { return addressCollection; } }
            /// <summary>
            /// 构造器
            /// </summary>
            public MailAddresses() {
                addressCollection = new MailAddressCollection();
            }
            ///<summary>
            /// 添加一个新的邮件地址
            ///</summary>
            ///<param name="mail">邮件地址</param>
            ///<returns>MailAddresses(this)</returns>
            public MailAddresses Add(string mail) {
                AddressCollection.Add(new MailAddress(mail));
                return this;
            }
            ///<summary>
            /// 添加一个新的邮件地址
            ///</summary>
            ///<param name="displayName">显示名称</param>
            ///<param name="mail">邮件地址</param>
            ///<returns>MailAddresses(this)</returns>
            public MailAddresses Add(string displayName, string mail) {
                AddressCollection.Add(new MailAddress(mail, displayName));
                return this;
            }
            ///<summary>
            /// 添加一个新的邮件地址
            ///</summary>
            ///<param name="mails">邮件列表</param>
            ///<returns>MailAddresses(this)</returns>
            public MailAddresses Add(IEnumerable<string> mails) {
                foreach (var mail in mails) {
                    AddressCollection.Add(new MailAddress(mail));
                }
                return this;
            }
            ///<summary>
            /// 添加一个新的邮件地址
            ///</summary>
            ///<param name="contacts">邮件列表</param>
            ///<returns>MailAddresses(this)</returns>
            public MailAddresses Add(Dictionary<string, string> contacts) {
                foreach (var contact in contacts) {
                    AddressCollection.Add(new MailAddress(contact.Value, contact.Key));
                }
                return this;
            }
        }
        private string errorMessage = string.Empty;
        /// <summary>
        /// 出错消息
        /// </summary>
        public string ErrorMessage { get { return errorMessage; } }
    }
}
