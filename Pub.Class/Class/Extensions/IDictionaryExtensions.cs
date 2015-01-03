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

namespace Pub.Class {
    /// <summary>
    /// IDictionary 扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class IDictionaryExtensions {
        /// <summary>
        /// IsNullEmpty
        /// </summary>
        /// <param name="self">Dictionary扩展</param>
        /// <returns>true/false</returns>
        public static bool IsNullEmpty(this IDictionary self) { return self.IsNull() || self.Count == 0; }
        /// <summary>
        /// 取值，不存在时添加
        /// </summary>
        /// <example>
        /// <code>
        /// Dictionary&lt;int, int> dict = new Dictionary&lt;int, int>(); 
        /// int value1 = dict.GetValueOrAdd&lt;int, int>(10, 5);
        /// </code>
        /// </example>
        /// <typeparam name="TKey">key类型</typeparam>
        /// <typeparam name="TValue">value类型</typeparam>
        /// <param name="dictionary">Dictionary扩展</param>
        /// <param name="key">key</param>
        /// <param name="defValue">默认值</param>
        /// <returns>值</returns>
        public static TValue GetValueOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defValue) where TValue : new() {
            TValue result;
            if (dictionary.TryGetValue(key, out result)) {
                return result;
            } else {
                result = defValue;
                dictionary.Add(key, defValue);
                return result;
            }
        }
        /// <summary>
        /// 取值，不存在时添加
        /// </summary>
        /// <example>
        /// <code>
        /// Dictionary&lt;int, int> dict = new Dictionary&lt;int, int>(); 
        /// int value = dict.GetValueOrAdd&lt;int, int>(10); 
        /// </code>
        /// </example>
        /// <typeparam name="TKey">key类型</typeparam>
        /// <typeparam name="TValue">value类型</typeparam>
        /// <param name="dictionary">Dictionary扩展</param>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        public static TValue GetValueOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TValue : new() {
            return dictionary.GetValueOrAdd(key, default(TValue));
        }
        /// <summary>
        /// 取值
        /// </summary>
        /// <example>
        /// <code>
        /// Dictionary&lt;int, int> dict = new Dictionary&lt;int, int>(); 
        /// int value3 = dict.GetValue&lt;int, int>(10, 1);
        /// </code>
        /// </example>
        /// <typeparam name="TKey">key类型</typeparam>
        /// <typeparam name="TValue">value类型</typeparam>
        /// <param name="dictionary">Dictionary扩展</param>
        /// <param name="key">key</param>
        /// <param name="defValue">默认值</param>
        /// <returns>值</returns>
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defValue) {
            TValue result;
            if (dictionary.TryGetValue(key, out result)) return result; else return defValue;
        }
        /// <summary>
        /// 取值
        /// </summary>
        /// <example>
        /// <code>
        /// Dictionary&lt;int, int> dict = new Dictionary&lt;int, int>(); 
        /// int value2 = dict.GetValue&lt;int, int>(10);
        /// </code>
        /// </example>
        /// <typeparam name="TKey">key类型</typeparam>
        /// <typeparam name="TValue">value类型</typeparam>
        /// <param name="dictionary">Dictionary扩展</param>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) {
            return dictionary.GetValue(key, default(TValue));
        }
        /// <summary>
        /// IDictionary数据转URL字符串
        /// </summary>
        /// <param name="parameters">IDictionary</param>
        /// <returns></returns>
        public static string ToUrlEncode(this IDictionary parameters) {
            if (parameters.IsNull() || parameters.Count == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (string k in parameters.Keys) sb.AppendFormat("{0}={1}&", k.UrlEncode(), parameters[k].ToString().UrlEncode());
            sb.RemoveLastChar("&");
            return sb.ToString();
        }
        /// <summary>
        /// IDictionary数据转URL字符串 Join("=","&amp;")
        /// </summary>
        /// <param name="parameters">IDictionary</param>
        /// <param name="split1">分隔符1</param>
        /// <param name="split2">分隔符2</param>
        /// <returns></returns>
        public static string Join(this IDictionary parameters, string split1 = "=", string split2 = "&") {
            if (parameters.IsNull() || parameters.Count == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (string k in parameters.Keys) sb.AppendFormat("{0}{2}{1}{3}", k, parameters[k].ToString(), split1, split2);
            sb.RemoveLastChar(split2);
            return sb.ToString();
        }
        /// <summary>
        /// 添加唯一项
        /// </summary>
        /// <typeparam name="K">key类型</typeparam>
        /// <typeparam name="V">value类型</typeparam>
        /// <param name="list">IDictionary列表</param>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns>IDictionary列表</returns>
        public static IDictionary<K, V> Add<K, V>(this IDictionary<K, V> list, K key, V value) {
            lock (((ICollection)list).SyncRoot) { if (!list.ContainsKey(key)) list.Add(key, value); else list[key] = value; }
            return list;
        }
        /// <summary>
        /// 遍历
        /// </summary>
        /// <example>
        /// <code>
        /// 
        /// </code>
        /// </example>
        /// <typeparam name="K">key类型</typeparam>
        /// <typeparam name="V">value类型</typeparam>
        /// <param name="source">IDictionary扩展</param>
        /// <param name="action">动作</param>
        /// <returns></returns>
        public static void Do<K, V>(this IDictionary<K, V> source, Action<K, V> action) {
            foreach (K x in source.Keys) action(x, source[x]);
        }
        /// <summary>
        /// 遍历
        /// </summary>
        /// <example>
        /// <code>
        /// 
        /// </code>
        /// </example>
        /// <typeparam name="K">key类型</typeparam>
        /// <typeparam name="V">value类型</typeparam>
        /// <param name="source">IDictionary扩展</param>
        /// <param name="action">动作</param>
        /// <returns></returns>
        public static void Do<K, V>(this IDictionary<K, V> source, Action<K, V, int> action) {
            var i = 0;
            foreach (K x in source.Keys) action(x, source[x], i++);
        }
        public static DataTable ToDataTable<TKey, TValue>(this Dictionary<TKey, TValue> hashtable) {
            var dataTable = new DataTable(hashtable.GetType().Name);
            dataTable.Columns.Add("Key", typeof(TKey));
            dataTable.Columns.Add("Value", typeof(TValue));
            foreach (KeyValuePair<TKey, TValue> var in hashtable) {
                dataTable.Rows.Add(var.Key, var.Value);
            }
            return dataTable;
        }
    }
}
