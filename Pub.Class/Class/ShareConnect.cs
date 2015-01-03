using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif
using Pub.Class;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Pub.Class {
    /// <summary>
    /// LogonType
    /// </summary>
    public enum LogonType : uint {
        /// <summary>
        /// Interactive
        /// </summary>
        Interactive = 2,
        /// <summary>
        /// Batch
        /// </summary>
        Network = 3,
        /// <summary>
        /// Batch
        /// </summary>
        Batch = 4,
        /// <summary>
        /// Service
        /// </summary>
        Service = 5,
        /// <summary>
        /// Unlock
        /// </summary>
        Unlock = 7,
        /// <summary>
        /// NetworkClearText
        /// </summary>
        NetworkClearText = 8,
        /// <summary>
        /// NewCredentials
        /// </summary>
        NewCredentials = 9
    }
    /// <summary>
    /// LogonProvider
    /// </summary>
    public enum LogonProvider : uint {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,
        /// <summary>
        /// WinNT35
        /// </summary>
        WinNT35 = 1,
        /// <summary>
        /// WinNT40
        /// </summary>
        WinNT40 = 2,
        /// <summary>
        /// WinNT50
        /// </summary>
        WinNT50 = 3,
    }
    /// <summary>
    /// 共享连接类
    /// 
    /// 修改纪录
    ///     2006.05.12 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    ///     using(new ShareConnect(ToSwf.IP, ToSwf.UserName, ToSwf.Password, LogonType.NewCredentials, LogonProvider.WinNT50)){
    ///         if (!FileFolder.FileExists(flashPrinterApiPath) || (!System.IO.File.Exists(filePath))) return; ==
    ///     }
    /// </code>
    /// </example>
    /// </summary>
    public class ShareConnect : IDisposable {
        [DllImport("Advapi32.dll")]
        static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, LogonType dwLogonType, LogonProvider dwLogonProvider, out IntPtr phToken);
        [DllImport("Advapi32.DLL")]
        static extern bool ImpersonateLoggedOnUser(IntPtr hToken);
        [DllImport("Advapi32.DLL")]
        static extern bool RevertToSelf();
        [DllImport("Kernel32.dll")]
        static extern int GetLastError();
        bool disposed;
        /// <summary>
        /// 访问共享目录
        /// </summary>
        /// <param name="domain">共享目录</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        public ShareConnect(string domain, string userName, string password) : this(domain, userName, password, LogonType.Interactive, LogonProvider.Default) {
        }
        /// <summary>
        /// 访问共享目录
        /// </summary>
        /// <param name="domain">共享目录</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="logonType"></param>
        /// <param name="logonProvider"></param>
        public ShareConnect(string domain, string userName, string password, LogonType logonType, LogonProvider logonProvider) {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(domain)) domain = ".";

            IntPtr token;
            int errorCode = 0;
            if (LogonUser(userName, domain, password, logonType, logonProvider, out token)) {
                if (!ImpersonateLoggedOnUser(token)) errorCode = GetLastError();
            } else {
                errorCode = GetLastError();
            }
            if (errorCode != 0) throw new Win32Exception(errorCode);
        }
        /// <summary>
        /// 析构
        /// </summary>
        ~ShareConnect() { Dispose(false); }
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                if (disposing) {
                    // Nothing to do.
                }
                RevertToSelf();
                disposed = true;
            }
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

