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
using System.Net.Mime;
using System.Net;

namespace Pub.Class {
    /// <summary>
    /// 写日志
    /// 
    /// 修改纪录
    ///     2013.02.12 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Log {
        private readonly ILog log;
        /// <summary>
        /// 构造器 指定DLL文件和全类名
        /// </summary>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">命名空间.类名</param>
        public Log(string dllFileName, string className) {
            errorMessage = string.Empty;
            if (log.IsNull()) {
                log = (ILog)dllFileName.LoadClass(className);
            }
        }
        /// <summary>
        /// 构造器 指定classNameDllName(LogProviderName) 默认Pub.Class.SimpleLog,Pub.Class
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        public Log(string classNameAndAssembly) {
            errorMessage = string.Empty;
            if (log.IsNull()) {
                if (classNameAndAssembly.IsNullEmpty())
                    log = Singleton<SimpleLog>.Instance();
                else
                    log = (ILog)classNameAndAssembly.LoadClass();
            }
        }
        /// <summary>
        /// 构造器 从Web.config中读LogProviderName 默认Pub.Class.SimpleLog,Pub.Class
        /// </summary>
        public Log() {
            errorMessage = string.Empty;
            if (log.IsNull()) {
                string classNameAndAssembly = WebConfig.GetApp("LogProviderName");
                if (classNameAndAssembly.IsNullEmpty())
                    log = Singleton<SimpleLog>.Instance();
                else
                    log = (ILog)classNameAndAssembly.LoadClass();
            }
        }
        private string errorMessage = string.Empty;
        /// <summary>
        /// 出错消息
        /// </summary>
        public string ErrorMessage { get { return errorMessage; } }
        ///<summary>
        /// 写日志
        ///</summary>
        /// <param name="msg">消息</param>
        /// <param name="encoding">编码</param>
        ///<returns>true/false</returns>
        public bool Write(string msg, Encoding encoding = null) {
            errorMessage = string.Empty;
            try {
                log.Write(msg, encoding ?? Encoding.UTF8);
                return true;
            } catch (Exception ex) {
                errorMessage = ex.ToExceptionDetail();
                return false;
            }
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="ex">消息</param>
        /// <param name="encoding">编码</param>
        /// <returns>true/false</returns>
        public bool Write(Exception ex, Encoding encoding = null) {
            return Write(ex.ToExceptionDetail(), encoding);
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msgFormat">消息格式</param>
        /// <param name="values">消息</param>
        /// <returns>true/false</returns>
        public bool Write(string msgFormat, params string[] values) {
            return Write(string.Format(msgFormat, values));
        }

        private static ILog s_log;
        /// <summary>
        /// 使用外部插件写日志
        /// </summary>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">命名空间.类名</param>
        public static void Use(string dllFileName, string className) {
            s_log = (ILog)dllFileName.LoadClass(className);
        }
        /// <summary>
        /// 使用外部插件写日志
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        public static void Use(string classNameAndAssembly) {
            if (classNameAndAssembly.IsNullEmpty())
                s_log = HttpContext.Current.IsNull() ? (ILog)Singleton<TraceLog>.Instance() : (ILog)Singleton<SimpleLog>.Instance();
            else
                s_log = (ILog)classNameAndAssembly.LoadClass();
        }
        /// <summary>
        /// 使用外部插件
        /// </summary>
        public static void Use<T>(T t) where T : ILog, new() {
            s_log = Singleton<T>.Instance();
        }
        ///<summary>
        /// 写日志
        ///</summary>
        /// <param name="msg">消息</param>
        /// <param name="encoding">编码</param>
        ///<returns>true/false</returns>
        public static bool WriteLog(string msg, Encoding encoding = null) {
            if (s_log.IsNull()) {
                string classNameAndAssembly = WebConfig.GetApp("LogProviderName");
                Use(classNameAndAssembly);
            }
            return s_log.Write(msg, encoding ?? Encoding.UTF8);
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="ex">消息</param>
        /// <param name="encoding">编码</param>
        /// <returns>true/false</returns>
        public static bool WriteLog(Exception ex, Encoding encoding = null) {
            return WriteLog(ex.ToExceptionDetail(), encoding);
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msgFormat">消息格式</param>
        /// <param name="values">消息</param>
        /// <returns>true/false</returns>
        public static bool WriteLog(string msgFormat, params string[] values) {
            return WriteLog(string.Format(msgFormat, values));
        }
    }
}
