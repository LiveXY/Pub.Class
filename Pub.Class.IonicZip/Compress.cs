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
using Ionic.Zip;
using Ionic.Zlib;

namespace Pub.Class.IonicZip {
    /// <summary>
    /// 压缩文件
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Compress : ICompress {
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="source">源文件</param>
        /// <param name="desc">目标ZIP文件路径</param>
        /// <param name="password">密码</param>
        public void File(string source, string descZip, string password = null) {
            FileInfo objFile = new FileInfo(source);
            if (!objFile.Exists) return;
            string filename = objFile.Name;

            using (ZipFile zip = new ZipFile()) {
                if (!password.IsNullEmpty()) zip.Password = password;
                zip.AddFile(source, "");
                zip.Save(descZip);
            }
        }
        /// <summary>
        /// 将多个文件压缩成一个文件
        /// </summary>
        /// <param name="source">多个文件，采用全路径，例：e:\tmp\tmp1\DD.cs</param>
        /// <param name="descZip">目标zip文件路径</param>
        /// <param name="password">密码</param>
        public void File(string[] source, string descZip, string password = null) {
            using (ZipFile zip = new ZipFile()) {
                if (!password.IsNullEmpty()) zip.Password = password;
                foreach(string file in source) zip.AddFile(file, "");
                zip.Save(descZip);
            }
        }
        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="source">要压缩的目录</param>
        /// <param name="descZip">压缩后的文件名</param>
        /// <param name="password">密码</param>
        public void Directory(string source, string descZip, string password = null) {
            source = source.Trim('\\') + "\\";
            using (ZipFile zip = new ZipFile()) {
                if (!password.IsNullEmpty()) zip.Password = password;
                zip.AddDirectory(source, "");
                zip.Save(descZip);
            }
        }
    }
}
