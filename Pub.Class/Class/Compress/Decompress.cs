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

namespace Pub.Class {
    /// <summary>
    /// 解压缩文件
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public interface IDecompress: IAddIn {
        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipPath">ZIP文件</param>
        /// <param name="directory">解压到目录</param>
        /// <param name="password">密码</param>
        void File(string zipPath, string directory, string password = null);
    }

    /// <summary>
    /// 解压缩文件
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    ///         new Decompress("Pub.Class.SharpZip.dll", "Pub.Class.SharpZip.Decompress")
    ///             .File("~/web.config.zip".GetMapPath(), "~/back/".GetMapPath())
    ///             .Dispose();
    /// </code>
    /// </example>
    /// </summary>
    public class Decompress: Disposable {
        private readonly IDecompress decompress = null;
        /// <summary>
        /// 构造器 指定DLL文件和全类名
        /// </summary>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">命名空间.类名</param>
        public Decompress(string dllFileName, string className) {
            if (decompress.IsNull()) {
                decompress = (IDecompress)dllFileName.LoadClass(className);
            }
        }
        /// <summary>
        /// 构造器 指定classNameDllName(DecompressProviderName) 默认Pub.Class.SharpZip.Decompress,Pub.Class.SharpZip
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        public Decompress(string classNameAndAssembly) { 
            if (decompress.IsNull()) {
                decompress = (IDecompress)classNameAndAssembly.IfNullOrEmpty("Pub.Class.SharpZip.Decompress,Pub.Class.SharpZip").LoadClass();
            }
        }
        /// <summary>
        /// 构造器 从Web.config中读DecompressProviderName 默认Pub.Class.SharpZip.Decompress,Pub.Class.SharpZip
        /// </summary>
        public Decompress() { 
            if (decompress.IsNull()) {
                decompress = (IDecompress)(WebConfig.GetApp("DecompressProviderName") ?? "Pub.Class.SharpZip.Decompress,Pub.Class.SharpZip").LoadClass();
            }
        }
        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipPath">ZIP文件</param>
        /// <param name="directory">解压到目录</param>
        /// <param name="password">密码</param>
        public Decompress File(string zipPath, string directory, string password = null) {
            decompress.File(zipPath, directory, password);
            return this;
        }
        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipPath">ZIP文件</param>
        /// <param name="directory">解压到目录</param>
        public Decompress File(string zipPath, string directory) {
            decompress.File(zipPath, directory, null);
            return this;
        }
        /// <summary>
        /// 用using 自动释放
        /// </summary>
        protected override void InternalDispose() {
            base.InternalDispose();
        }
    }
}
