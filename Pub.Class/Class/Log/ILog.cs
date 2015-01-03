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

namespace Pub.Class {
    /// <summary>
    /// 写日志
    /// 
    /// 修改纪录
    ///     2013.02.12 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public interface ILog : IAddIn {
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="encoding">编码</param>
        /// <returns>true/false</returns>
        bool Write(string msg, Encoding encoding = null);
    }
}
