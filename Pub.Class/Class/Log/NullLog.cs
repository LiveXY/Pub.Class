//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Text;
using System.Web;
using System.IO;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Pub.Class {
    /// <summary>
    /// 日志管理类
    /// 
    /// 修改纪录
    ///     2006.05.03 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class NullLog: ILog {
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="encoding">编码</param>
        /// <returns>true/false</returns>
        public bool Write(string msg, Encoding encoding = null) {
            return true;
        }
    }
}
