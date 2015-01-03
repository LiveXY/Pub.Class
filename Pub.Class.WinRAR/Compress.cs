//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace Pub.Class.WinRAR {
    /// <summary>
    /// 压缩文件
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Compress : ICompress {
        private string rarSetupPath = "c:\\Program Files\\WinRAR\\rar.exe";

        public Compress() {
            if (Registry2.Exists("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths", "WinRAR.exe")) {
                string regPath = Registry2.Read("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\WinRAR.exe", "").ToStr();
                if (FileDirectory.FileExists(regPath)) rarSetupPath = regPath;
            }
            if (!FileDirectory.FileExists(rarSetupPath)) {
                rarSetupPath = "c:\\Program Files (x86)\\WinRAR\\rar.exe";
                if (!FileDirectory.FileExists(rarSetupPath)) {
                    rarSetupPath = "rar.exe".GetBinFileFullPath();
                    if (!FileDirectory.FileExists(rarSetupPath)) {
                        throw new ArgumentNullException("未找到WinRAR安装程序");
                    }
                }
            }
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="source">源文件</param>
        /// <param name="desc">目标ZIP文件路径</param>
        /// <param name="password">密码</param>
        public void File(string source, string descZip, string password = null) {
            password = password.IsNullEmpty() ? "" : " -P" + password;
            string msg = Safe.RunWait(rarSetupPath, System.Diagnostics.ProcessWindowStyle.Hidden, " a -ep \"{0}\" \"{1}\"{2}".FormatWith(descZip, source, password));
        }
        /// <summary>
        /// 将多个文件压缩成一个文件
        /// </summary>
        /// <param name="source">多个文件，采用全路径，例：e:\tmp\tmp1\DD.cs</param>
        /// <param name="descZip">目标ZIP文件路径</param>
        /// <param name="password">密码</param>
        public void File(string[] source, string descZip, string password = null) {
            password = password.IsNullEmpty() ? "" : " -P" + password;
            if (source.Length == 0) return;
            StringBuilder sbFile = new StringBuilder();
            foreach (string info in source) sbFile.AppendFormat(" \"{0}\"", info);
            string msg = Safe.RunWait(rarSetupPath, System.Diagnostics.ProcessWindowStyle.Hidden, " a -ep \"{0}\" {1}{2}".FormatWith(descZip, sbFile.ToString(), password));
        }
        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="source">要压缩的目录</param>
        /// <param name="descZip">压缩后的文件名</param>
        /// <param name="password">密码</param>
        public void Directory(string source, string descZip, string password = null) {
            password = password.IsNullEmpty() ? "" : " -P" + password;
            string msg = Safe.RunWait(rarSetupPath, System.Diagnostics.ProcessWindowStyle.Hidden, " a -r -ep1 \"{0}\" \"{1}\\*.*\"{2}".FormatWith(descZip, source.Trim('\\'), password));
        }
    }
}
