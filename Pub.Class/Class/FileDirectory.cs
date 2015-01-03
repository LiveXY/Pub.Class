//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Web;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Pub.Class {
    /// <summary>
    /// COPY 选项
    /// 
    /// 修改纪录
    ///     2006.05.02 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public enum CopyOptions {
        /// <summary>
        /// 如果是新的
        /// </summary>
        CopyIfNewer,
        /// <summary>
        /// 总是
        /// </summary>
        CopyAlways,
        /// <summary>
        /// 不覆盖
        /// </summary>
        DoNotOverwrite
    }

    /// <summary>
    /// 文件目录操作类
    /// 
    /// 修改纪录
    ///     2006.05.02 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class FileDirectory {
        private static readonly object lockHelper = new object();
        //#region Write/Read
        /// <summary>
        /// 字节写到文件
        /// </summary>
        /// <param name="bytes">字节</param>
        /// <param name="fileName">方件名</param>
        /// <param name="fileMode">FileMode</param>
        /// <returns>true/false</returns>
        public static bool FileWrite(string fileName, byte[] bytes, FileMode fileMode = FileMode.CreateNew) {
            bool returnValue = true;
            FileAccess fileAccess = FileAccess.ReadWrite;
            if (fileMode == FileMode.Append) fileAccess = FileAccess.Write;
            FileStream fs = new FileStream(fileName, fileMode, fileAccess);
            BinaryWriter bw = new BinaryWriter(fs);
                try { bw.Write(bytes); } catch (Exception) { returnValue = false; } finally { fs.Close(); bw.Close(); }
            return returnValue;
        }
        /// <summary>
        /// 字节写到文件
        /// </summary>
        /// <param name="bytes">字节</param>
        /// <param name="fileName">方件名</param>
        /// <param name="fileMode">FileMode</param>
        /// <returns>true/false</returns>
        public static bool FileWriteLock(string fileName, byte[] bytes, FileMode fileMode = FileMode.CreateNew) {
            lock (lockHelper) {
                return FileWrite(fileName, bytes, fileMode);
            }
        }
        /// <summary>
        /// 写日志gb2312 UTF-8 线程安全
        /// </summary>
        /// <param name="fileName">文件</param>
        /// <param name="str">消息</param>
        /// <param name="encoding">编码gb2312 utf-8</param>
        public static bool FileWrite(string fileName, string str, string encoding) {
            if (encoding.IsNullEmpty()) encoding = "utf-8";
            return FileWrite(fileName, str, System.Text.Encoding.GetEncoding(encoding));
        }
        /// <summary>
        /// 写日志 线程安全
        /// </summary>
        /// <param name="fileName">文件</param>
        /// <param name="str">消息</param>
        /// <param name="encoding">编码</param>
        public static bool FileWrite(string fileName, string str, Encoding encoding = null) {
            return WriteForStreamWriter(fileName, str, encoding);
        }
        /// <summary>
        /// 异步写日志 线程安全
        /// </summary>
        /// <param name="fileName">文件</param>
        /// <param name="str">消息</param>
        /// <param name="encoding">编码</param>
        public static void FileAsynWrite(string fileName, string str, Encoding encoding = null) {
            Func<string, string, Encoding, bool> action = FileWrite;
            action.BeginInvoke(fileName, str, encoding, iar => {
                Action<string, string, Encoding> end = (Action<string, string, Encoding>)((AsyncResult)iar).AsyncDelegate;
                end.EndInvoke(iar);
            }, null);
        }
        /// <summary>
        /// 写日志 线程安全
        /// </summary>
        /// <param name="fileName">文件</param>
        /// <param name="str">消息</param>
        /// <param name="encoding">编码</param>
        private static bool WriteForStreamWriter(string fileName, string str, Encoding encoding = null) {
            bool _isTrue = false;
            if (encoding.IsNull()) encoding = Encoding.Default;
            lock (lockHelper) {
                using (System.IO.StreamWriter f2 = new System.IO.StreamWriter(fileName, true, encoding.IfNull(Encoding.UTF8))) { f2.WriteLine(str); _isTrue = true; }
            }
            return _isTrue;
        }
        /// <summary>
        /// 写日志 线程安全
        /// </summary>
        /// <param name="fileName">文件</param>
        /// <param name="str">消息</param>
        /// <param name="encoding">编码</param>
        private static bool WriteForFileStream(string fileName, string str, Encoding encoding = null) {
            bool _isTrue = false;
            if (encoding.IsNull()) encoding = Encoding.Default;
            lock (lockHelper) {
                using (FileStream fs = FileDirectory.FileExists(fileName) ? File.OpenWrite(fileName) : File.Create(fileName)) {
                    byte[] bt = encoding.IfNull(Encoding.UTF8).GetBytes(str + Environment.NewLine);
                    fs.Seek(0, SeekOrigin.End);
                    fs.Write(bt, 0, bt.Length);
                    _isTrue = true;
                }
            }
            return _isTrue;
        }
        /// <summary>
        /// 读取文件中的内容
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns>读取文件中的内容</returns>
        public static IList<string> FileRead(string fileName, Encoding encoding = null) {
            string lineText = null; IList<string> txtTextArr = new List<string>();
            if (!FileDirectory.FileExists(fileName)) { return txtTextArr; }

            using (StreamReader reader = new StreamReader(fileName, encoding.IfNull(Encoding.UTF8))) {
                while ((lineText = reader.ReadLine()).IsNotNull()) txtTextArr.Add(lineText);
            }
            return txtTextArr;
        }
        /// <summary>
        /// 读取文件中的内容  线程安全
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns>读取文件中的内容</returns>
        public static IList<string> FileReadLock(string fileName, Encoding encoding = null) {
            lock (lockHelper) {
                return FileRead(fileName, encoding);
            }
        }
        /// <summary>
        /// 读取文件中的内容  线程安全
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns>读取文件中的内容</returns>
        public static string FileReadAll(string fileName, Encoding encoding = null) {
            string text = string.Empty;
            if (!FileDirectory.FileExists(fileName)) { return text; }

            using (StreamReader reader = new StreamReader(fileName, encoding.IfNull(Encoding.UTF8))) 
                return reader.ReadToEnd();
        }
        /// <summary>
        /// 读取文件中的内容  线程安全
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="encoding">编码</param>
        /// <returns>读取文件中的内容</returns>
        public static string FileReadAllLock(string fileName, Encoding encoding = null) {
            lock (lockHelper) {
                return FileReadAll(fileName, encoding);
            }
        }
        
        //#endregion

        //#region SHFILEOPSTRUCT/wFunc/FILEOP_FLAGS
        private struct SHFILEOPSTRUCT {
            public IntPtr hwnd;
            public wFunc wFunc;
            public string pFrom;
            public string pTo;
            public FILEOP_FLAGS fFlags;
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }
        private enum wFunc {
            FO_MOVE = 0x0001,
            FO_COPY = 0x0002,
            FO_DELETE = 0x0003,
            FO_RENAME = 0x0004
        }
        private enum FILEOP_FLAGS {
            FOF_MULTIDESTFILES = 0x0001, //pTo 指定了多个目标文件，而不是单个目录
            FOF_CONFIRMMOUSE = 0x0002,
            FOF_SILENT = 0x0044, // 不显示一个进度对话框
            FOF_RENAMEONCOLLISION = 0x0008, // 碰到有抵触的名字时，自动分配前缀
            FOF_NOCONFIRMATION = 0x10, // 不对用户显示提示
            FOF_WANTMAPPINGHANDLE = 0x0020, // 填充 hNameMappings 字段，必须使用 SHFreeNameMappings 释放
            FOF_ALLOWUNDO = 0x40, // 允许撤销
            FOF_FILESONLY = 0x0080, // 使用 *.* 时, 只对文件操作
            FOF_SIMPLEPROGRESS = 0x0100, // 简单进度条，意味者不显示文件名。
            FOF_NOCONFIRMMKDIR = 0x0200, // 建新目录时不需要用户确定
            FOF_NOERRORUI = 0x0400, // 不显示出错用户界面
            FOF_NOCOPYSECURITYATTRIBS = 0x0800, // 不复制 NT 文件的安全属性
            FOF_NORECURSION = 0x1000 // 不递归目录
        }
        //#endregion
        //#region Directory
        /// <summary>
        /// 虚拟目录不存在时新建
        /// </summary>
        /// <param name="filePath">目录（相对路径:/）</param>
        public static void DirectoryVirtualCreate(string filePath) {
            string[] PathArr = filePath.Split(new string[] { "/" }, StringSplitOptions.None);
            string _path = PathArr[0];
            for (int i = 1; i < PathArr.Length; i++) {
                _path = _path + "/" + PathArr[i];
                string _filePath = HttpContext.Current.Server.MapPath(_path);
                if (!System.IO.Directory.Exists(_filePath)) {
                    System.IO.Directory.CreateDirectory(_filePath);
                }
            }
        }
#if !MONO40
        /// <summary>
        /// 创建目录 一次性建多级目录 决对路径 如:c:\test\test\test
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>创建是否成功</returns>
        [DllImport("dbgHelp", SetLastError = true)]
        private static extern bool MakeSureDirectoryPathExists(string name);
        /// <summary>
        /// 创建目录 一次性建多级目录 决对路径 如:c:\\test\\test\\test
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>true/false</returns>
        public static bool DirectoryCreate(string filePath) {
            return MakeSureDirectoryPathExists(filePath.TrimEnd('\\') + "\\");
        }
        [DllImport("shell32.dll")]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);
        /// <summary>
        /// API删除
        /// </summary>
        /// <param name="path">目录路径</param>
        public static int APIDelete(string path) {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("DirectoryPath");
            SHFILEOPSTRUCT lpFileOp = new SHFILEOPSTRUCT();
            lpFileOp.wFunc = wFunc.FO_DELETE;
            lpFileOp.pFrom = path + "\0";
            lpFileOp.fFlags = FILEOP_FLAGS.FOF_NOCONFIRMATION | FILEOP_FLAGS.FOF_NOERRORUI | FILEOP_FLAGS.FOF_SILENT;
            lpFileOp.fFlags &= ~FILEOP_FLAGS.FOF_ALLOWUNDO;
            lpFileOp.fAnyOperationsAborted = false;

            return SHFileOperation(ref lpFileOp);
        }
#endif
        /// <summary>
        /// 目录删除
        /// </summary>
        /// <param name="DirectoryPath">目录路径</param>
        public static void DirectoryDelete(string DirectoryPath) {
            if (string.IsNullOrEmpty(DirectoryPath)) throw new ArgumentNullException("DirectoryPath");
            if (DirectoryExists(DirectoryPath)) Directory.Delete(DirectoryPath, true);
        }
        /// <summary>
        /// 目录是否存在
        /// </summary>
        /// <param name="folderPath">目录路径</param>
        /// <returns>true/false</returns>
        public static bool DirectoryExists(string folderPath) {
            return System.IO.Directory.Exists(folderPath);
        }
        /// <summary>
        /// 目录移动
        /// </summary>
        /// <param name="Source">源</param>
        /// <param name="Destination">目标</param>
        /// <param name="overwrite">覆盖</param>
        public static bool DirectoryMove(string Source, string Destination, bool overwrite = true) {
            if (string.IsNullOrEmpty(Source)) throw new ArgumentNullException("Source");
            if (string.IsNullOrEmpty(Destination)) throw new ArgumentNullException("Destination");
            if (!DirectoryExists(Source)) throw new ArgumentException("Source directory does not exist");
            try {
                if (overwrite && DirectoryExists(Destination)) DirectoryDelete(Destination);
                System.IO.Directory.Move(Source, Destination);
                return true;
            } catch { return false; }
        }
        /// <summary>
        /// COPY目录
        /// </summary>
        /// <param name="Source">源</param>
        /// <param name="Destination">目标</param>
        /// <param name="Recursive">递归</param>
        /// <param name="Options">选项</param>
        public static void DirectoryCopy(string Source, string Destination, bool Recursive = true, CopyOptions Options = CopyOptions.CopyAlways) {
            if (string.IsNullOrEmpty(Source)) throw new ArgumentNullException("Source");
            if (string.IsNullOrEmpty(Destination)) throw new ArgumentNullException("Destination");
            if (!DirectoryExists(Source)) throw new ArgumentException("Source directory does not exist");
            DirectoryInfo SourceInfo = new DirectoryInfo(Source);
            DirectoryInfo DestinationInfo = new DirectoryInfo(Destination);
#if !MONO40
            DirectoryCreate(Destination);
#endif
            List<FileInfo> Files = FileList(Source, false);
            foreach (FileInfo File in Files) {
                if (Options == CopyOptions.CopyAlways) {
                    File.CopyTo(Path.Combine(DestinationInfo.FullName, File.Name), true);
                } else if (Options == CopyOptions.CopyIfNewer) {
                    if (FileExists(Path.Combine(DestinationInfo.FullName, File.Name))) {
                        FileInfo FileInfo = new FileInfo(Path.Combine(DestinationInfo.FullName, File.Name));
                        if (FileInfo.LastWriteTime.CompareTo(File.LastWriteTime) < 0) {
                            File.CopyTo(Path.Combine(DestinationInfo.FullName, File.Name), true);
                        }
                    } else {
                        File.CopyTo(Path.Combine(DestinationInfo.FullName, File.Name), true);
                    }
                } else if (Options == CopyOptions.DoNotOverwrite) {
                    File.CopyTo(Path.Combine(DestinationInfo.FullName, File.Name), false);
                }
            }
            if (Recursive) {
                List<DirectoryInfo> Directories = DirectoryList(SourceInfo.FullName);
                foreach (DirectoryInfo Directory in Directories) {
                    DirectoryCopy(Directory.FullName, Path.Combine(DestinationInfo.FullName, Directory.Name), Recursive, Options);
                }
            }
        }
        /// <summary>
        /// 取子目录 非子目录的子目录
        /// </summary>
        /// <param name="DirectoryPath">目录路径</param>
        /// <returns></returns>
        public static List<DirectoryInfo> DirectoryList(string DirectoryPath) {
            if (string.IsNullOrEmpty(DirectoryPath)) throw new ArgumentNullException("DirectoryPath");
            List<DirectoryInfo> Directories = new List<DirectoryInfo>();
            if (DirectoryExists(DirectoryPath)) {
                DirectoryInfo Directory = new DirectoryInfo(DirectoryPath);
                DirectoryInfo[] SubDirectories = Directory.GetDirectories();
                foreach (DirectoryInfo SubDirectory in SubDirectories) {
                    Directories.Add(SubDirectory);
                }
            }
            return Directories;
        }
        /// <summary>
        /// 取目录大小
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <returns>取目录大小</returns>
        public static long DirectorySize(string path) {
            long size = 0;
            if (!DirectoryExists(path)) return 0;
            string[] files = System.IO.Directory.GetFiles(path);
            foreach (string s in files) size += new FileInfo(s).Length;
            string[] dir = System.IO.Directory.GetDirectories(path);
            foreach (string s in dir) size += DirectorySize(s);
            return size;
        }
        /// <summary>
        /// 目录最后修改时间
        /// </summary>
        /// <param name="folderPath">目录路径</param>
        /// <returns>目录最后修改时间</returns>
        public static DateTime DirectoryLastModified(string folderPath) {
            var dInfo = new DirectoryInfo(folderPath);
            return dInfo.LastWriteTime;
        }
        //#endregion
        //#region File
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourceFileName">源文件</param>
        /// <param name="destFileName">目标文件</param>
        /// <param name="overwrite">覆盖</param>
        /// <returns>true/false</returns>
        public static bool FileMove(string sourceFileName, string destFileName, bool overwrite = true) {
            if (!File.Exists(sourceFileName)) return false;

            try {
                if (overwrite && File.Exists(destFileName)) File.Delete(destFileName);
                File.Move(sourceFileName, destFileName);
            } catch {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="sourceFileName">源文件</param>
        /// <param name="destFileName">目标文件</param>
        /// <param name="overwrite">覆盖</param>
        /// <returns>true/false</returns>
        public static bool FileCopy(string sourceFileName, string destFileName, bool overwrite = true) {
            if (!File.Exists(sourceFileName)) return false;
            if (!overwrite && File.Exists(destFileName)) return false;

            try {
                File.Copy(sourceFileName, destFileName, true);
            } catch {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 返回文件是否存在
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否存在true/false</returns>
        public static bool FileExists(string filename) {
            return System.IO.File.Exists(filename);
        }
        /// <summary>
        /// 删除指定目录下的所有文件 
        /// </summary>
        /// <param name="path">文件路径</param>
        public static void FileDeleteAll(string path) {
            if (!DirectoryExists(path)) return;
            DirectoryInfo Folder = new DirectoryInfo(path);
            FileInfo[] subFiles = Folder.GetFiles();
            for (int j = 0; j < subFiles.Length; j++) {
                subFiles[j].Delete();
            }
        }
        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void FileDelete(string filePath) {
            if (FileExists(filePath)) System.IO.File.Delete(filePath);
        }
        /// <summary>
        /// 取指定目录下的所有文件名
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>取指定目录下的所有文件名</returns>
        public static IList<string> FileList(string path) {
            IList<string> list = new List<string>();
            DirectoryInfo Folder = new DirectoryInfo(path);
            FileInfo[] subFiles = Folder.GetFiles();
            for (int j = 0; j < subFiles.Length; j++) list.Add(subFiles[j].Name);
            return list;
        }
        /// <summary>
        /// 取目录下的所有文件 递归
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="fileList">列表</param>
        /// <param name="delStr">删除的字符串</param>
        public static void FileList(string path, ref IList<string> fileList, string delStr = "") {
            if (delStr.IsNullEmpty()) delStr = path;
            delStr = delStr.TrimEnd('\\') + "\\";
            string[] files = System.IO.Directory.GetFiles(path);
            foreach (string s in files) fileList.Add(s.Replace(delStr, ""));
            string[] dir = System.IO.Directory.GetDirectories(path);
            foreach (string s in dir) FileList(s, ref fileList, delStr);
        }
        /// <summary>
        /// 取目录下的所有文件
        /// </summary>
        /// <param name="DirectoryPath">目录路径</param>
        /// <param name="Recursive">是否递归</param>
        /// <returns></returns>
        public static List<FileInfo> FileList(string DirectoryPath, bool Recursive = false) {
            if (string.IsNullOrEmpty(DirectoryPath)) throw new ArgumentNullException("DirectoryPath");
            List<FileInfo> Files = new List<FileInfo>();
            if (DirectoryExists(DirectoryPath)) {
                DirectoryInfo Directory = new DirectoryInfo(DirectoryPath);
                Files.AddRange(Directory.GetFiles());
                if (Recursive) {
                    DirectoryInfo[] SubDirectories = Directory.GetDirectories();
                    foreach (DirectoryInfo SubDirectory in SubDirectories) {
                        Files.AddRange(FileList(SubDirectory.FullName, true));
                    }
                }
            }
            return Files;
        }
        /// <summary>
        /// 文件比较
        /// </summary>
        /// <param name="FileName1">文件1</param>
        /// <param name="FileName2">文件2</param>
        /// <returns>true/false</returns>
        public static bool FileCompare(string FileName1, string FileName2) {
            if (string.IsNullOrEmpty(FileName1)) throw new ArgumentNullException("FileName1");
            if (string.IsNullOrEmpty(FileName2)) throw new ArgumentNullException("FileName2");
            if (!FileExists(FileName1)) throw new ArgumentException("FileName1 does not exist");
            if (!FileExists(FileName2)) throw new ArgumentException("FileName2 does not exist");
            FileInfo File1 = new FileInfo(FileName1);
            FileInfo File2 = new FileInfo(FileName2);
            if (File1.Length != File2.Length) {
                return false;
            }
            string File1Contents = FileDirectory.FileReadAll(FileName1, Encoding.Default);
            string File2Contents = FileDirectory.FileReadAll(FileName2, Encoding.Default);
            if (!File1Contents.Equals(File2Contents)) return false;
            return true;
        }
        /// <summary>
        /// 删除新文件
        /// </summary>
        /// <param name="Directory">目录</param>
        /// <param name="CompareDate">时间</param>
        /// <param name="Recursive">是否递归</param>
        public static void FileDeleteNewerThan(string Directory, DateTime CompareDate, bool Recursive = false) {
            if (string.IsNullOrEmpty(Directory))
                throw new ArgumentNullException("Directory");
            List<FileInfo> Files = FileList(Directory, Recursive);
            foreach (FileInfo File in Files) {
                if (File.LastWriteTime > CompareDate) {
                    FileDelete(File.FullName);
                }
            }
        }
        /// <summary>
        /// 删除老文件
        /// </summary>
        /// <param name="Directory">目录</param>
        /// <param name="CompareDate">时间</param>
        /// <param name="Recursive">是否递归</param>
        public static void FileDeleteOlderThan(string Directory, DateTime CompareDate, bool Recursive = false) {
            if (string.IsNullOrEmpty(Directory))
                throw new ArgumentNullException("Directory");
            List<FileInfo> Files = FileList(Directory, Recursive);
            foreach (FileInfo File in Files) {
                if (File.LastWriteTime < CompareDate) {
                    FileDelete(File.FullName);
                }
            }
        }
        /// <summary>
        /// 文件改名
        /// </summary>
        /// <param name="FileName">文件名</param>
        /// <param name="NewFileName">新文件名</param>
        public static void FileRename(string FileName, string NewFileName) {
            if (string.IsNullOrEmpty(FileName)) throw new ArgumentNullException("FileName");
            if (string.IsNullOrEmpty(NewFileName)) throw new ArgumentNullException("NewFileName");
            if (!FileExists(FileName)) throw new ArgumentException("FileName does not exist");
            File.Move(FileName, NewFileName);
        }
        /// <summary>
        /// 取文件大小
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>取文件大小</returns>
        public static long FileSize(string path) {
            if (!FileExists(path)) return 0;
            return new FileInfo(path).Length;
        }
        /// <summary>
        /// 文件最后修改时间
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件最后修改时间</returns>
        public static DateTime FileLastModified(string filePath) {
            if (!FileExists(filePath)) throw new ArgumentException("filePath does not exist");
            var fInfo = new FileInfo(filePath);
            return fInfo.LastWriteTime;
        }
        //#endregion
        //#region FileEncoding/GZipCompress/GZipDecompress/GetFileContent
        /// <summary>
        /// 取文件编码
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>取文件编码</returns>
        public static Encoding FileEncoding(string fileName) {
            /*byte[] Unicode=new byte[]{0xFF,0xFE};
            byte[] UnicodeBIG=new byte[]{0xFE,0xFF};
            byte[] UTF8=new byte[]{0xEF,0xBB,0xBF};*/
            if (!FileExists(fileName)) return null;
            try {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
                byte[] ss = r.ReadBytes(3);
                r.Close();
                fs.Close();
                fs.Dispose();

                if (ss[0] >= 0xEF) {
                    if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                        return System.Text.Encoding.UTF8;
                    else if (ss[0] == 0xFE && ss[1] == 0xFF)
                        return System.Text.Encoding.BigEndianUnicode;
                    else if (ss[0] == 0xFF && ss[1] == 0xFE)
                        return System.Text.Encoding.Unicode;
                    else
                        return System.Text.Encoding.Default;
                } else return System.Text.Encoding.Default;
            } catch {
                return System.Text.Encoding.Default;
            }
        }
        /// <summary>
        /// GZipStream压缩文件
        /// </summary>
        /// <param name="inFilename">源文件</param>
        /// <param name="outFilename">新文件</param>
        public static void GZipCompress(string inFilename, string outFilename) {
            if (!FileExists(inFilename)) return;
            FileStream sourceFile = File.OpenRead(inFilename);
            FileStream destFile = File.Create(outFilename);
            GZipStream compStream = new GZipStream(destFile, CompressionMode.Compress);
            int theByte = sourceFile.ReadByte();

            while (theByte != -1) {
                compStream.WriteByte((byte)theByte);
                theByte = sourceFile.ReadByte();
            }

            sourceFile.Close();
            compStream.Close();
            destFile.Close();
        }
        /// <summary>
        /// GZipStream解压文件
        /// </summary>
        /// <param name="inFileName">源文件</param>
        /// <param name="outFileName">新文件</param>
        public static void GZipDecompress(string inFileName, string outFileName) {
            if (!FileExists(inFileName)) return;
            FileStream sourceFile = File.OpenRead(inFileName);
            FileStream destFile = File.Create(outFileName);
            GZipStream compStream = new GZipStream(sourceFile, CompressionMode.Decompress);
            int theByte = compStream.ReadByte();

            while (theByte != -1) {
                destFile.WriteByte((byte)theByte);
                theByte = compStream.ReadByte();
            }

            destFile.Close();
            compStream.Close();
            sourceFile.Close();
        }
        //#endregion
        /// <summary>
        /// 计算指定文件的MD5值
        /// </summary>
        /// <param>指定文件的完全限定名称</param>
        /// <returns>返回值的字符串形式</returns>
        public static string HashMD5(string fileName) {
            string hashMD5 = string.Empty;
            //检查文件是否存在，如果文件存在则进行计算，否则返回空值
            if (System.IO.File.Exists(fileName)) {
                using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                    //计算文件的MD5值
                    System.Security.Cryptography.MD5 calculator = System.Security.Cryptography.MD5.Create();
                    byte[] buffer = calculator.ComputeHash(fs);
                    calculator.Clear();
                    //将字节数组转换成十六进制的字符串形式
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < buffer.Length; i++) {
                        stringBuilder.Append(buffer[i].ToString("x2"));
                    }
                    hashMD5 = stringBuilder.ToString();
                }//关闭文件流
            }//结束计算
            return hashMD5;
        }
        ///// <summary>
        ///// 计算指定文件的CRC32值
        ///// </summary>
        ///// <param>指定文件的完全限定名称</param>
        ///// <returns>返回值的字符串形式</returns>
        //public static string HashCRC32(string fileName) {
        //    String hashCRC32 = String.Empty;
        //    //检查文件是否存在，如果文件存在则进行计算，否则返回空值
        //    if (System.IO.File.Exists(fileName)) {
        //        using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
        //            //计算文件的尼康数码产品测评CSC32值
        //            CRC32 calculator = new CRC32();
        //            Byte[] buffer = calculator.ComputeHash(fs);
        //            calculator.Clear();
        //            //将字节数组转换成十六进制的字符串形式
        //            StringBuilder stringBuilder = new StringBuilder();
        //            for (int i = 0; i < buffer.Length; i++) {
        //                stringBuilder.Append(buffer[i].ToString("x2"));
        //            }
        //            hashCRC32 = stringBuilder.ToString();
        //        }//关闭文件流
        //    }
        //    return hashCRC32;
        //}
        /// <summary>
        /// 计算指定文件的SHA1值
        /// </summary>
        /// <param>指定文件的完全限定名称</param>
        /// <returns>返回值的字符串形式</returns>
        public static string HashSHA1(string fileName) {
            String hashSHA1 = String.Empty;
            //检查文件是否存在，如果文件存在则进行计算，否则返回空值
            if (System.IO.File.Exists(fileName)) {
                using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                    //计算文件的SHA1值
                    System.Security.Cryptography.SHA1 calculator = System.Security.Cryptography.SHA1.Create();
                    Byte[] buffer = calculator.ComputeHash(fs);
                    calculator.Clear();
                    //将字节数组转换成十六进制的字符串形式
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < buffer.Length; i++) {
                        stringBuilder.Append(buffer[i].ToString("x2"));
                    }
                    hashSHA1 = stringBuilder.ToString();
                }//关闭文件流
            }
            return hashSHA1;
        }
    }
}
