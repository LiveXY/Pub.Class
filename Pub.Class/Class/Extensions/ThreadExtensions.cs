//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Web.Script.Serialization;
using System.IO;
using System.Security.Cryptography;

namespace Pub.Class {
    /// <summary>
    /// Thread 扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class ThreadExtensions {
        /// <summary>
        /// WaitUntil
        /// </summary>
        /// <param name="thread">线程</param>
        /// <param name="condition">条件</param>
        public static void WaitUntil(this System.Threading.Thread thread, Func<bool> condition) {
            while (!condition.Invoke()) System.Threading.Thread.Sleep(0);
        }
        /// <summary>
        /// WaitWhile
        /// </summary>
        /// <param name="thread">线程</param>
        /// <param name="condition">条件</param>
        public static void WaitWhile(this System.Threading.Thread thread, Func<bool> condition) {
            while (condition.Invoke()) System.Threading.Thread.Sleep(0);
        }
    }
}
