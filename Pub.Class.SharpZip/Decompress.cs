//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace Pub.Class.SharpZip {
    /// <summary>
    /// 解压缩文件
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Decompress: IDecompress {
        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipPath">源文件</param>
        /// <param name="directory">目标文件</param>
        /// <param name="password">密码</param>
        public void File(string zipPath, string directory, string password = null) {
            FileInfo objFile = new FileInfo(zipPath);
            if (!objFile.Exists || !objFile.Extension.ToUpper().Equals(".ZIP")) return;
            FileDirectory.DirectoryCreate(directory);

            ZipInputStream objZIS = new ZipInputStream(System.IO.File.OpenRead(zipPath));
            if (!password.IsNullEmpty()) objZIS.Password = password;
            ZipEntry objEntry;
            while ((objEntry = objZIS.GetNextEntry()) != null) {
                string directoryName = Path.GetDirectoryName(objEntry.Name);
                string fileName = Path.GetFileName(objEntry.Name);
                if (directoryName != String.Empty) FileDirectory.DirectoryCreate(directory + directoryName);
                if (fileName != String.Empty) {
                    FileStream streamWriter = System.IO.File.Create(Path.Combine(directory, objEntry.Name));
                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true) {
                        size = objZIS.Read(data, 0, data.Length);
                        if (size > 0) {
                            streamWriter.Write(data, 0, size);
                        } else {
                            break;
                        }
                    }
                    streamWriter.Close();
                }
            }
            objZIS.Close();
        }
    }
}
