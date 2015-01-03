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
    /// 压缩文件
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public interface ICompress : IAddIn {
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="source">源文件</param>
        /// <param name="desc">目标文件</param>
        /// <param name="password">密码</param>
        void File(string source, string desc, string password = null);
        /// <summary>
        /// 将多个文件压缩成一个文件
        /// </summary>
        /// <param name="source">多个文件，采用全路径，例：e:\tmp\tmp1\DD.cs</param>
        /// <param name="descZip">目标文件</param>
        /// <param name="password">密码</param>
        void File(string[] source, string descZip, string password = null);
        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="source">要压缩的目录</param>
        /// <param name="descZip">压缩后的文件名</param>
        /// <param name="password">密码</param>
        void Directory(string source, string descZip, string password = null);
    }

    /// <summary>
    /// 压缩文件
    /// 
    /// 修改纪录
    ///     2011.07.11 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    ///         new Compress("Pub.Class.SharpZip.dll", "Pub.Class.SharpZip.Compress")
    ///             .File("~/web.config".GetMapPath(), "~/web.config.zip".GetMapPath())
    ///             .Directory("~/test/".GetMapPath(), "~/test.zip".GetMapPath())
    ///             .File(new string[]{ "~/test.xls".GetMapPath(),"~/test.xml".GetMapPath(),"~/test2.xls".GetMapPath() }, "~/excel.zip".GetMapPath())
    ///             .Dispose();
    /// </code>
    /// </example>
    /// </summary>
    public class Compress: Disposable {
        private readonly ICompress compress = null;
        /// <summary>
        /// 构造器 指定DLL文件和全类名
        /// </summary>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">命名空间.类名</param>
        public Compress(string dllFileName, string className) {
            if (compress.IsNull()) {
                compress = (ICompress)dllFileName.LoadClass(className);
            }
        }
        /// <summary>
        /// 构造器 指定classNameDllName(CompressProviderName) 默认Pub.Class.SharpZip.Compress,Pub.Class.SharpZip
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        public Compress(string classNameAndAssembly) { 
            if (compress.IsNull()) {
                compress = (ICompress)classNameAndAssembly.IfNullOrEmpty("Pub.Class.SharpZip.Compress,Pub.Class.SharpZip").LoadClass();
            }
        }
        /// <summary>
        /// 构造器 从Web.config中读CompressProviderName 默认Pub.Class.SharpZip.Compress,Pub.Class.SharpZip
        /// </summary>
        public Compress() { 
            if (compress.IsNull()) {
                compress = (ICompress)(WebConfig.GetApp("CompressProviderName") ?? "Pub.Class.SharpZip.Compress,Pub.Class.SharpZip").LoadClass();
            }
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="source">源文件</param>
        /// <param name="descZip">目标文件</param>
        /// <param name="password">密码</param>
        public Compress File(string source, string descZip, string password = null) {
            compress.File(source, descZip, password);
            return this;
        }
        /// <summary>
        /// 将多个文件压缩成一个文件
        /// </summary>
        /// <param name="source">多个文件，采用全路径，例：e:\tmp\tmp1\DD.cs</param>
        /// <param name="descZip">目标压缩文件</param>
        /// <param name="password">密码</param>
        public Compress File(string[] source, string descZip, string password = null) {
            compress.File(source, descZip, password);
            return this;
        }
        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="source">要压缩的目录</param>
        /// <param name="descZip">压缩后的文件名</param>
        /// <param name="password">密码</param>
        public Compress Directory(string source, string descZip, string password = null) {
            compress.Directory(source, descZip, password);
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
