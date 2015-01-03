//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Web.Mail;

namespace Pub.Class.Email.SmtpMail {
    /// <summary>
    /// 使用SmtpMail发送邮件
    /// 
    /// 修改纪录
    ///     2012.02.20 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class SendEmail : IEmail {
        private string errorMessage = string.Empty;
        /// <summary>
        /// 出错消息
        /// </summary>
        /// <returns></returns>
        public string ErrorMessage { get { return errorMessage; } }
        /// <summary>
        /// 发送EMAIL
        /// </summary>
        /// <param name="message">MailMessage</param>
        /// <param name="smtp">SmtpClient</param>
        /// <returns>true/false</returns>
        public bool Send(System.Net.Mail.MailMessage message, System.Net.Mail.SmtpClient smtp) {
            try {
                //Msg.Write("Host:{0}<br />".FormatWith(smtp.Host));
                //Msg.Write("Port:{0}<br />".FormatWith(smtp.Port));
                //Msg.Write("From:{0}<br />".FormatWith(message.From.ToJson()));
                //Msg.Write("Body:{0}<br />".FormatWith(message.Body));
                //Msg.Write("Subject:{0}<br />".FormatWith(message.Subject));
                //Msg.Write("IsBodyHtml:{0}<br />".FormatWith(message.IsBodyHtml));
                //Msg.Write("Credentials:{0}<br />".FormatWith(smtp.Credentials.GetCredential(smtp.Host, smtp.Port, "").ToJson()));
                //Msg.Write("To:{0}<br />".FormatWith(message.To.ToJson()));
                //Msg.Write("Cc:{0}<br />".FormatWith(message.CC.ToJson()));
                StringBuilder toList = new StringBuilder();
                message.To.Do(p => toList.Append(p.Address).Append(";"));
                StringBuilder ccList = new StringBuilder();
                message.CC.Do(p => ccList.Append(p.Address).Append(";"));

                System.Web.Mail.MailMessage msg = new System.Web.Mail.MailMessage();
                msg.From = "\"{0}\"<{1}>".FormatWith(message.From.DisplayName, message.From.Address);
                msg.To = toList.ToStr();
                msg.Cc = ccList.ToStr();
                msg.BodyFormat = message.IsBodyHtml ? MailFormat.Html : MailFormat.Text;
                msg.Subject = message.Subject;
                msg.BodyEncoding = message.BodyEncoding;
                msg.Body = message.Body;
                msg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", smtp.UseDefaultCredentials ? "0" : "1");
                msg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", smtp.Port);
                msg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", smtp.EnableSsl ? "true" : "false");
                if (!smtp.UseDefaultCredentials) {
                    NetworkCredential n = smtp.Credentials.GetCredential(smtp.Host, smtp.Port, "");
                    msg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", n.UserName);
                    msg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", n.Password);
                }
                System.Web.Mail.SmtpMail.SmtpServer = smtp.Host;
                System.Web.Mail.SmtpMail.Send(msg);
                return true;
            } catch(Exception ex) {
                errorMessage = ex.ToExceptionDetail();
                return false;
            } finally {
                message = null;
                smtp = null;
            }
        }
    }
}
