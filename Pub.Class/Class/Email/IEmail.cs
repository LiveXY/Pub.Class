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

namespace Pub.Class {
    /// <summary>
    /// 发送EMAIL接口
    /// 
    /// 修改纪录
    ///     2012.02.20 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public interface IEmail : IAddIn {
        /// <summary>
        /// 发送EMAIL
        /// </summary>
        /// <param name="message">MailMessage</param>
        /// <param name="smtp">SmtpClient</param>
        /// <returns>true/false</returns>
        bool Send(MailMessage message, SmtpClient smtp);
        /// <summary>
        /// 出错消息
        /// </summary>
        /// <returns></returns>
        string ErrorMessage { get; }
    }
}
