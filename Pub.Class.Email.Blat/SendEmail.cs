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
using System.Diagnostics;

namespace Pub.Class.Email.Blat {
    /// <summary>
    /// 使用Blat发送邮件
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
        //private string Run(string cmd, string arguments) {
        //    cmd = "\"" + cmd + "\"";
        //    //Msg.Write(cmd + " " + arguments + "<br />");
        //    //Log.Write("test.log".GetMapPath(), cmd + " " + arguments);
        //    //using (System.Diagnostics.Process pScore = new System.Diagnostics.Process()) {
        //    //    pScore.StartInfo.FileName = cmd;
        //    //    pScore.StartInfo.Arguments = arguments;
        //    //    pScore.StartInfo.UseShellExecute = false;
        //    //    pScore.StartInfo.RedirectStandardInput = true;
        //    //    pScore.StartInfo.RedirectStandardOutput = false;
        //    //    pScore.StartInfo.RedirectStandardError = true;
        //    //    pScore.StartInfo.CreateNoWindow = true;
        //    //    pScore.Start();
        //    //    pScore.PriorityClass = ProcessPriorityClass.High;
        //    //    string log = pScore.StandardError.ReadToEnd().Trim();// +" " + pScore.StandardOutput.ReadToEnd().Trim();
        //    //    pScore.WaitForExit();
        //    //    pScore.Close();
        //    //    return log;
        //    //}
        //}
        /// <summary>
        /// 发送EMAIL
        /// </summary>
        /// <param name="message">MailMessage</param>
        /// <param name="smtp">SmtpClient</param>
        /// <returns>true/false</returns>
        public bool Send(System.Net.Mail.MailMessage message, System.Net.Mail.SmtpClient smtp) {
            try {
                StringBuilder toList = new StringBuilder();
                message.To.Do(p => toList.Append(p.Address).Append(","));
                StringBuilder ccList = new StringBuilder();
                message.CC.Do(p => ccList.Append(p.Address).Append(","));

                string path = "".GetBinFileFullPath().TrimEnd(".dll");
                string blatApi = path + "blat.exe";
                string install = " -install {0} {1} 3 {2}";
                string send = "-body \"{0}\" -to \"{1}\" -sf \"{2}\"{4} -i \"{5}\" -f \"{5}\"{3}{6} -charset utf-8";
                NetworkCredential n = smtp.Credentials.GetCredential(smtp.Host, smtp.Port, "");

                path += "\\temp\\";
                if (!FileDirectory.DirectoryExists(path)) FileDirectory.DirectoryCreate(path);
                //string body = path + Rand.RndDateStr() + ".txt";
                //Log.Write(body, message.Body);
                string subject = path + Rand.RndDateStr() + ".txt";
                FileDirectory.FileWrite(subject, message.Subject);

                string log = Safe.RunWait(blatApi, ProcessWindowStyle.Hidden, install.FormatWith(smtp.Host, message.From.Address, smtp.Port));
                log = Safe.RunWait(blatApi, ProcessWindowStyle.Hidden, send.FormatWith(
                    message.Body,
                    toList.ToString().Trim(','),
                    subject,
                    smtp.UseDefaultCredentials ? "" : " -u {0} -pw {1}".FormatWith(n.UserName, n.Password),
                    message.CC.Count == 0 ? "" : (" -c \"" + ccList.ToString().Trim(',') + "\""),
                    message.From.Address,
                    message.IsBodyHtml ? " -html" : ""
                ));
                //FileDirectory.FileDelete(body);
                FileDirectory.FileDelete(subject);
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
