//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using System.Collections;
using System.Collections.Generic;

namespace Pub.Class.SharpZip {
    /// <summary>
    /// 压缩文件
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Compress: ICompress {
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

            FileStream fs = System.IO.File.OpenRead(source);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            FileStream ZipFile = System.IO.File.Create(descZip);
            ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
            if (!password.IsNullEmpty()) ZipStream.Password = password;
            ZipEntry ZipEntry = new ZipEntry(filename);
            ZipStream.PutNextEntry(ZipEntry);
            ZipStream.SetLevel(6);
            ZipStream.Write(buffer, 0, buffer.Length);
            ZipStream.Finish();
            ZipStream.Close();
        }
        /// <summary>
        /// 将多个文件压缩成一个文件
        /// </summary>
        /// <param name="source">多个文件，采用全路径，例：e:\tmp\tmp1\DD.cs</param>
        /// <param name="descZip">目标ZIP文件路径</param>
        /// <param name="password">密码</param>
        public void File(string[] source, string descZip, string password = null) {
            ZipOutputStream outStream = new ZipOutputStream(System.IO.File.Create(descZip));
            if (!password.IsNullEmpty()) outStream.Password = password;
            Crc32 crc = new Crc32();
            foreach (string info in source) {
                File(info, crc, outStream);
            }
            outStream.Finish();
            outStream.Close();
        }
        /// <summary>
        /// 功能：压缩一个文件,不包含路径信息
        /// </summary>
        /// <param name="strSrcFile">待压缩文件</param>
        /// <param name="crc">CRC校验</param>
        /// <param name="outStream"></param>
        private void File(string source, Crc32 crc, ZipOutputStream outStream) {
            string strFileName = Path.GetFileName(source); // 文件名，不含路径信息
            #region 读取文件信息
            FileStream fs = System.IO.File.OpenRead(source);
            long iLength = fs.Length;// 文件长度
            byte[] buffer = new byte[iLength];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            #endregion
            ZipEntry entry = new ZipEntry(strFileName);
            entry.CompressionMethod = CompressionMethod.Deflated; // deflate
            entry.DateTime = DateTime.Now;
            entry.Size = iLength;
            #region CRC校验
            crc.Reset();
            crc.Update(buffer);
            entry.Crc = crc.Value;
            #endregion
            outStream.PutNextEntry(entry);
            outStream.Write(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="source">要压缩的目录</param>
        /// <param name="descZip">压缩后的文件名</param>
        /// <param name="password">密码</param>
        public void Directory(string source, string descZip, string password = null) {
            source = source.Trim('\\') + "\\";
            IList<string> filenames = new List<string>();
            FileDirectory.FileList(source, ref filenames, source);
            Crc32 crc = new Crc32();
            ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(descZip));
            if (!password.IsNullEmpty()) s.Password = password;
            s.SetLevel(6);
            foreach (string file in filenames) {
                FileStream fs = System.IO.File.OpenRead(source + file);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                ZipEntry entry = new ZipEntry(file);
                entry.DateTime = DateTime.Now;
                entry.Size = fs.Length;
                fs.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                s.PutNextEntry(entry);
                s.Write(buffer, 0, buffer.Length);
            }
            s.Finish();
            s.Close();
        }
    }
}
