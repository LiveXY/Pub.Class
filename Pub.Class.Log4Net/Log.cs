//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pub.Class.Log4Net {
    /// <summary>
    /// 写日志
    /// 
    /// 修改纪录
    ///     2013.02.12 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Log : ILog {
        private readonly static string logName = WebConfig.GetApp("log4net.LoggerName");
        private readonly static log4net.ILog log = log4net.LogManager.GetLogger(logName.IsNullEmpty() ? "loginfo": logName);
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="encoding">编码</param>
        /// <returns>true/false</returns>
        public bool Write(string msg, Encoding encoding = null) {
            log.Info(msg);
            return true;
        }
    }
}
