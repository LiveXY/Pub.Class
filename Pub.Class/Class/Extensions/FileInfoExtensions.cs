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
using System.Drawing;
using Microsoft.Win32;
using System.Collections;

namespace Pub.Class {
    /// <summary>
    /// 文件扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class FileInfoExtensions {
        /// <summary>
        /// 文件改名 带扩展名
        /// </summary>
        /// <param name="file">文件FileInfo</param>
        /// <param name="newName">新文件名</param>
        /// <returns>文件FileInfo</returns>
        public static FileInfo Rename(this FileInfo file, string newName) {
            var filePath = Path.Combine(Path.GetDirectoryName(file.FullName), newName);
            file.MoveTo(filePath);
            return file;
        }
        /// <summary>
        /// 文件改名 不修改扩展名 不带扩展名
        /// </summary>
        /// <param name="file">文件FileInfo</param>
        /// <param name="newName">新文件名</param>
        /// <returns></returns>
        public static FileInfo RenameFileWithoutExtension(this FileInfo file, string newName) {
            var fileName = string.Concat(newName, file.Extension);
            file.Rename(fileName);
            return file;
        }
        /// <summary>
        /// 修改扩展名
        /// </summary>
        /// <param name="file">文件FileInfo</param>
        /// <param name="newExtension">新扩展名</param>
        /// <returns></returns>
        public static FileInfo ChangeExtension(this FileInfo file, string newExtension) {
            newExtension = newExtension.ForcePrefix(".");
            var fileName = string.Concat(Path.GetFileNameWithoutExtension(file.FullName), newExtension);
            file.Rename(fileName);
            return file;
        }
        /// <summary>
        /// 批量修改扩展名
        /// </summary>
        /// <param name="files">文件FileInfo[]</param>
        /// <param name="newExtension">新扩展名</param>
        /// <returns></returns>
        public static FileInfo[] ChangeExtensions(this FileInfo[] files, string newExtension) {
            files.ForEach(f => f.ChangeExtension(newExtension));
            return files;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="files">文件FileInfo[]</param>
        public static void Delete(this FileInfo[] files) {
            files.Delete(true);
        }
        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="files">文件FileInfo[]</param>
        /// <param name="consolidateExceptions">异常</param>
        public static void Delete(this FileInfo[] files, bool consolidateExceptions) {
            List<Exception> exceptions = null;

            foreach (var file in files) {
                try {
                    file.Delete();
                } catch (Exception e) {
                    if (consolidateExceptions) {
                        if (exceptions == null) exceptions = new List<Exception>();
                        exceptions.Add(e);
                    } else {
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="files">文件FileInfo[]</param>
        /// <param name="targetPath">目标路径</param>
        /// <returns></returns>
        public static FileInfo[] CopyTo(this FileInfo[] files, string targetPath) {
            return files.CopyTo(targetPath, true);
        }
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="files">文件FileInfo[]</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="consolidateExceptions">异常</param>
        /// <returns></returns>
        public static FileInfo[] CopyTo(this FileInfo[] files, string targetPath, bool consolidateExceptions) {
            var copiedfiles = new List<FileInfo>();
            List<Exception> exceptions = null;

            foreach (var file in files) {
                try {
                    var fileName = Path.Combine(targetPath, file.Name);
                    copiedfiles.Add(file.CopyTo(fileName));
                } catch (Exception e) {
                    if (consolidateExceptions) {
                        if (exceptions == null)
                            exceptions = new List<Exception>();
                        exceptions.Add(e);
                    } else {
                        throw;
                    }
                }
            }

            return copiedfiles.ToArray();
        }
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="files">文件FileInfo[]</param>
        /// <param name="targetPath">目标路径</param>
        /// <returns></returns>
        public static FileInfo[] MoveTo(this FileInfo[] files, string targetPath) {
            return files.MoveTo(targetPath, true);
        }
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="files">文件FileInfo[]</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="consolidateExceptions">异常</param>
        /// <returns></returns>
        public static FileInfo[] MoveTo(this FileInfo[] files, string targetPath, bool consolidateExceptions) {
            List<Exception> exceptions = null;

            foreach (var file in files) {
                try {
                    var fileName = Path.Combine(targetPath, file.Name);
                    file.MoveTo(fileName);
                } catch (Exception e) {
                    if (consolidateExceptions) {
                        if (exceptions == null) exceptions = new List<Exception>();
                        exceptions.Add(e);
                    } else {
                        throw;
                    }
                }
            }

            return files;
        }
        /// <summary>
        /// 修改文件属性
        /// </summary>
        /// <param name="files">文件FileInfo[]</param>
        /// <param name="attributes">FileAttributes</param>
        /// <returns></returns>
        public static FileInfo[] SetAttributes(this FileInfo[] files, FileAttributes attributes) {
            foreach (var file in files) file.Attributes = attributes;
            return files;
        }
        /// <summary>
        /// 修改文件属性
        /// </summary>
        /// <param name="files">文件FileInfo[]</param>
        /// <param name="attributes">FileAttributes</param>
        /// <returns></returns>
        public static FileInfo[] SetAttributesAdditive(this FileInfo[] files, FileAttributes attributes) {
            foreach (var file in files) file.Attributes = (file.Attributes | attributes);
            return files;
        }
        /// <summary>
        /// 媒体文件否
        /// </summary>
        /// <param name="fileInfo">FileInfo</param>
        /// <returns></returns>
        public static bool IsMediaFile(this FileInfo fileInfo) {
            ArrayList mediaExtensions = new ArrayList();
            if (mediaExtensions == null) {
                mediaExtensions = ArrayList.Synchronized(new ArrayList());
                mediaExtensions.Add(".bmp");
                mediaExtensions.Add(".gif");
                mediaExtensions.Add(".jpe");
                mediaExtensions.Add(".jpeg");
                mediaExtensions.Add(".jpg");
                mediaExtensions.Add(".png");
                mediaExtensions.Add(".tif");
                mediaExtensions.Add(".asf");
                mediaExtensions.Add(".asx");
                mediaExtensions.Add(".avi");
                mediaExtensions.Add(".mov");
                mediaExtensions.Add(".mp4");
                mediaExtensions.Add(".mpeg");
                mediaExtensions.Add(".mpg");
                mediaExtensions.Add(".wmv");
            }
            if (mediaExtensions.Contains(fileInfo.Extension)) return true;
            return false;
        }
        /// <summary>
        /// 返回文件大小
        /// </summary>
        /// <param name="fileInfo">FileInfo</param>
        /// <returns>返回文件大小</returns>
        public static string FormatBytes(this FileInfo fileInfo) {
            long fileSize = fileInfo.Length;
            return fileSize.FormatBytes();
        }
        /// <summary>
        /// 一行一行读文件
        /// </summary>
        /// <param name="fileInfo">FileInfo</param>
        /// <returns>文件内容</returns>
        public static List<string> Lines(this FileInfo fileInfo) {
            StreamReader file = new StreamReader(fileInfo.FullName);
            string line;
            List<string> lines = new List<string>();
            while ((line = file.ReadLine()) != null) lines.Add(line);
            return lines;
        }
        /// <summary>
        /// 文件MIME
        /// </summary>
        /// <param name="fileInfo">FileInfo</param>
        /// <returns>文件MIME</returns>
        public static string MimeType(this FileInfo fileInfo) {
            string mime = "application/octetstream";
            string ext = Path.GetExtension(fileInfo.Name).ToLower();
            RegistryKey rk = Registry.ClassesRoot.OpenSubKey(ext);
            if (rk != null && rk.GetValue("Content Type") != null) mime = rk.GetValue("Content Type").ToString();
            return mime;
        }
        /// <summary>
        /// 文件转字节
        /// </summary>
        /// <param name="fileInfo">FileInfo</param>
        /// <returns>文件转字节</returns>
        public static byte[] ToBytes(this FileInfo fileInfo) {
            return File.ReadAllBytes(fileInfo.FullName);
        }
        /// <summary>
        /// 读文件内容
        /// </summary>
        /// <param name="fileInfo">FileInfo</param>
        /// <returns>读文件内容</returns>
        public static string ReadAll(this FileInfo fileInfo) {
            var stream = new FileStream(fileInfo.FullName, FileMode.Open);
            var reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            reader.Close();
            stream.Close();
            return text;
        }
    }
}
