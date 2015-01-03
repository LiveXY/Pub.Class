//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.IO;
using Ionic.Zip;

namespace Pub.Class.IonicZip {
    /// <summary>
    /// 解压缩文件
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Decompress : IDecompress {
        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipPath">源文件</param>
        /// <param name="directory">目标文件</param>
        /// <param name="password">密码</param>
        public void File(string zipPath, string directory, string password = null) {
            using (ZipFile zip = ZipFile.Read(zipPath)) {
                if (!password.IsNullEmpty()) zip.Password = password;
                foreach (ZipEntry entry in zip) entry.Extract(directory, ExtractExistingFileAction.OverwriteSilently);
            }
        }
    }
}
