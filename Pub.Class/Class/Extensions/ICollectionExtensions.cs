//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
using System.Web.Script.Serialization;
#endif
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using Microsoft.VisualBasic;

namespace Pub.Class {
    /// <summary>
    /// ICollection扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class ICollectionExtensions {
        /// <summary>
        /// IsNullEmpty
        /// </summary>
        /// <param name="self">ICollection扩展</param>
        /// <returns>true/false</returns>
        public static bool IsNullEmpty(this ICollection self) { return self.IsNull() || self.Count == 0; }
        /// <summary>
        /// 添加项
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">IList列表</param>
        /// <param name="item">值</param>
        /// <returns>IList列表</returns>
        public static ICollection<T> Add<T>(this ICollection<T> list, T item) {
            list.Add(item);
            return list;
        }
        /// <summary>
        /// 添加唯一项
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">IList列表</param>
        /// <param name="item">值</param>
        /// <returns>IList列表</returns>
        public static ICollection<T> AddUnique<T>(this ICollection<T> list, T item) {
            lock (((ICollection)list).SyncRoot) { if (!list.Contains(item)) list.Add(item); }
            return list;
        }
        /// <summary>
        /// 添加唯一项 
        /// </summary>
        /// <example>
        /// <code>
        /// IList&lt;string> list2 = new List&lt;string>(); 
        /// list2.AddUnique&lt;string>("test1"); 
        /// list.AddUnique&lt;string>(list2);
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="collection">ICollection扩展</param>
        /// <param name="values">值</param>
        /// <returns>true/false</returns>
        public static ICollection<T> AddUnique<T>(this ICollection<T> collection, IEnumerable<T> values) {
            foreach (var value in values) collection.AddUnique<T>(value);
            return collection;
        }
    }
}
