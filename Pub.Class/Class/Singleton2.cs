//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Pub.Class {
    /// <summary>
    /// Singleton2 泛型饿汉单实例类延时实例化
    /// 
    /// 修改纪录
    ///     2008.06.12 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// public class UC_Member : Singleton2&lt;UC_Member> { }
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton2<T> where T : new() {
        private Singleton2() { }
        /// <summary>
        /// 获取实例
        /// </summary>
        public static T Instance() {
            return Singleton2Creator.instance;
        }
        class Singleton2Creator {
            internal static readonly T instance = new T();
        }
    }
}