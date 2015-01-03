//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Pub.Class {
    /// <summary>
    /// Singleton 泛型单实例类
    /// 
    /// 修改纪录
    ///     2008.06.12 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// public class UC_Member : Singleton&lt;UC_Member> { }
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Singleton<T> where T : new() {
        private static T instance = new T();
        private static readonly object lockHelper = new object();
        /// <summary>
        /// 获取实例
        /// </summary>
        public static T Instance() {
            if (instance.IsNull()) {
                lock (lockHelper) {
                    if (instance.IsNull()) instance = new T();
                }
            }

            return instance;
        }
        /// <summary>
        /// 设置实例
        /// </summary>
        /// <param name="value"></param>
        public void Instance(T value) {
            instance = value;
        }
    }

    /// <summary>
    /// SingletonEx 插件泛型单实例类
    /// 
    /// 修改纪录
    ///     2008.06.12 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// SingletonEx&lt;ICompress>.Instance("Pub.Class.SharpZip.dll", "Pub.Class.SharpZip.Compress").File("~/web.config".GetMapPath(), "~/web.config.zip".GetMapPath());
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="T">插件接口</typeparam>
    public sealed class SingletonAddIn<T> where T : IAddIn {
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <example>
        /// <code>
        /// SingletonAddIn&lt;ICompress>.Instance("Pub.Class.SharpZip.dll", "Pub.Class.SharpZip.Compress")
        ///     .File("~/web.config".GetMapPath(), "~/web.config.zip".GetMapPath());
        /// </code>
        /// </example>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">类名</param>
        public static T Instance(string dllFileName, string className) {
            return (T)dllFileName.LoadClass(className);
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <example>
        /// <code>
        /// SingletonAddIn&lt;ICompress>.InstanceDynamic("Pub.Class.SharpZip.dll", "Pub.Class.SharpZip.Compress")
        ///     .File("~/web.config".GetMapPath(), "~/web.config.zip".GetMapPath());
        /// </code>
        /// </example>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">类名</param>
        public static T InstanceDynamic(string dllFileName, string className) {
            return (T)dllFileName.LoadDynamicClass(className);
        }
        /// <summary>
        /// 获取实例 性能比较好
        /// </summary>
        /// <example>
        /// <code>
        /// SingletonAddIn&lt;ICompress>.Instance("Pub.Class.SharpZip.Compress,Pub.Class.SharpZip")
        ///     .File("~/web.config".GetMapPath(), "~/web.config.zip".GetMapPath());
        /// </code>
        /// </example>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        public static T Instance(string classNameAndAssembly) {
            return (T)classNameAndAssembly.LoadClass();
        }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static T Instance<T>() where T : IAddIn, new() {
            return Singleton<T>.Instance();
        }
    }
}