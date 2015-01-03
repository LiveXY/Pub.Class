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

namespace Pub.Class.Email.CDOMessage {
    /// <summary>
    /// 使用CDO.Message发送邮件
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

                CDO.Message objMail = new CDO.Message();
                objMail.To = toList.ToStr();
                objMail.CC = ccList.ToStr();
                objMail.From = "\"{0}\"<{1}>".FormatWith(message.From.DisplayName, message.From.Address);
                objMail.Subject = message.Subject;
                if (message.IsBodyHtml) objMail.HTMLBody = message.Body; else objMail.TextBody = message.Body;
                objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"].Value = smtp.Port; //设置端口
                objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"].Value = smtp.Host;
                objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"].Value = 2;
                objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpconnectiontimeout"].Value = 10;
                if (!smtp.UseDefaultCredentials) {
                    NetworkCredential n = smtp.Credentials.GetCredential(smtp.Host, smtp.Port, "");
                    //objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/sendemailaddress"].Value = "\"{0}\"<{1}>".FormatWith(message.From.DisplayName, message.From.Address);
                    //objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpuserreplyemailaddress"].Value = "\"{0}\"<{1}>".FormatWith(message.From.DisplayName, message.From.Address);
                    //objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpaccountname"].Value = n.UserName;
                    objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/sendusername"].Value = n.UserName;
                    objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"].Value = n.Password;
                    objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"].Value = 1;
                } else { 
                    objMail.Configuration.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"].Value = 0;
                }
                objMail.Configuration.Fields.Update();
                objMail.Send();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objMail);
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
