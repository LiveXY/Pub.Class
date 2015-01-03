//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;
using System.IO;

namespace Pub.Class.WinRAR {
    /// <summary>
    /// 解压缩文件
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Decompress: IDecompress {
        private string rarSetupPath = "c:\\Program Files\\WinRAR\\rar.exe";

        public Decompress() {
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
        /// 解压缩文件
        /// </summary>
        /// <param name="zipPath">源文件</param>
        /// <param name="directory">目标文件</param>
        /// <param name="password">密码</param>
        public void File(string zipPath, string directory, string password = null) {
            password = password.IsNullEmpty() ? "" : " -P" + password;
            string msg = Safe.RunWait(rarSetupPath, System.Diagnostics.ProcessWindowStyle.Hidden, " x -o+ -y \"{0}\" * \"{1}\"{2}".FormatWith(zipPath, directory, password));
        }
    }
}
