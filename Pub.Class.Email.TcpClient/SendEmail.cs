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
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

namespace Pub.Class.Email.TcpClient {
    /// <summary>
    /// 使用TcpClient发送邮件
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
        private string Base64Encode(string str) {
            return Convert.ToBase64String(Encoding.Default.GetBytes(str));
        }
        private bool SendCommand(string str, NetworkStream ns) {
            byte[] writeBuffer = Encoding.Default.GetBytes(str);
            ns.Write(writeBuffer, 0, writeBuffer.Length);
            return true;
        }
        private bool IsRight(NetworkStream ns, Hashtable rightCodeHT) {
            byte[] readBuffer = new byte[1024];
            string returnValue = "";
            int streamSize = ns.Read(readBuffer, 0, readBuffer.Length);
            if (streamSize != 0) returnValue = Encoding.Default.GetString(readBuffer, 0, streamSize);
            if (rightCodeHT[returnValue.Substring(0, 3)] == null) return false;
            return true;
        }
        /// <summary>
        /// 发送EMAIL
        /// </summary>
        /// <param name="message">MailMessage</param>
        /// <param name="smtp">SmtpClient</param>
        /// <returns>true/false</returns>
        public bool Send(System.Net.Mail.MailMessage message, System.Net.Mail.SmtpClient smtp) {
            try {
                Hashtable rightCodeHT = new Hashtable();
                rightCodeHT.Add("220", "");
                rightCodeHT.Add("250", "");
                rightCodeHT.Add("251", "");
                rightCodeHT.Add("354", "");
                rightCodeHT.Add("221", "");
                rightCodeHT.Add("334", "");
                rightCodeHT.Add("235", "");

                StringBuilder toList = new StringBuilder();
                message.To.Do(p => toList.Append("<").Append(p.Address).Append(">;"));
                StringBuilder ccList = new StringBuilder();
                message.CC.Do(p => ccList.Append("<").Append(p.Address).Append(">;"));

                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(smtp.Host, smtp.Port);
                NetworkStream ns = client.GetStream();
                if (!IsRight(ns, rightCodeHT)) return false;

                string enter = "\r\n";

                IList<string> sendBuffer = new List<string>();
                sendBuffer.Add("EHLO " + smtp.Host + enter);

                if (!smtp.UseDefaultCredentials) {
                    NetworkCredential n = smtp.Credentials.GetCredential(smtp.Host, smtp.Port, "");
                    sendBuffer.Add("AUTH LOGIN" + enter);
                    sendBuffer.Add(Base64Encode(n.UserName) + enter);
                    sendBuffer.Add(Base64Encode(n.Password) + enter);
                }

                sendBuffer.Add("MAIL FROM:<" + message.From.Address + ">" + enter);
                message.To.Do(p => sendBuffer.Add("RCPT TO:<" + p.Address + ">" + enter));
                message.CC.Do(p => sendBuffer.Add("RCPT TO:<" + p.Address + ">" + enter));
                sendBuffer.Add("DATA" + enter);

                StringBuilder code = new StringBuilder();
                code.Append("From:" + message.From.DisplayName + "<" + message.From.Address + ">" + enter);
                code.Append("To:" + toList.ToString().Trim(';') + "" + enter);
                code.Append("Cc:" + ccList.ToString().Trim(';') + "" + enter);
                code.Append("Subject:" + message.Subject + enter);
                code.Append("X-Priority:Normal" + enter);
                code.Append("X-MSMail-Priority:Normal" + enter);
                code.Append("Importance:Normal" + enter);
                code.Append("X-Mailer: Huolx.Pubclass" + enter);
                code.Append("MIME-Version: 1.0" + enter);
                if (message.IsBodyHtml)
                    code.Append("Content-Type: text/html;" + enter);
                else
                    code.Append("Content-Type: text/plain;" + enter);
                code.Append("Content-Transfer-Encoding: base64" + enter + enter);
                code.Append(Base64Encode(message.Body) + enter);
                code.Append(enter + "." + enter);
                sendBuffer.Add(code.ToString());

                sendBuffer.Add("QUIT" + enter);

                foreach (var cmd in sendBuffer) {
                    if (!SendCommand(cmd, ns)) return false;
                    if (!IsRight(ns, rightCodeHT)) return false;
                }

                client.Close();
                ns.Close();

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
