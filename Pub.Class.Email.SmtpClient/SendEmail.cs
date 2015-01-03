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

namespace Pub.Class.Email.SmtpClient {
    /// <summary>
    /// 使用SmtpClient发送邮件
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
        public bool Send(MailMessage message, System.Net.Mail.SmtpClient smtp) {
            try {
                smtp.Send(message);
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
