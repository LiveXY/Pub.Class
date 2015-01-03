//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;
using System.IO;

namespace Pub.Class {
    /// <summary>
    /// Memory缓存管理
    /// 
    /// 修改纪录
    ///     2009.11.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class MemoryCache : ICache2 {
        #region 构造器
        public MemoryCache() { }
        #endregion

        #region 私有静态字段
#if NET20
        private static readonly ISafeDictionary<string, CachedItem> cacheList = new SafeDictionary<string, CachedItem>();
#else
        private static readonly ISafeDictionary<string, CachedItem> cacheList = new SafeDictionarySlim<string, CachedItem>();
#endif
        /// <summary>
        /// 缓存因子
        /// </summary>
        private int Factor = 5;
        #endregion

        #region 静态方法
        public IList<CachedItem> GetList() {
            IList<CachedItem> list = new List<CachedItem>();
            foreach (string s in cacheList.Keys) { list.Add(cacheList[s]); }
            return list;
        }
        /// <summary>
        /// 清空所有缓存项目
        /// </summary>
        public void Clear() {
            cacheList.Clear();
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="pattern">缓存键正则匹配模式</param>
        public void RemoveByPattern(string pattern) {
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            foreach (string key in cacheList.Keys) {
                if (regex.IsMatch(key)) cacheList.Remove(key);
            }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存键名</param>
        public void Remove(string key) {
            if (cacheList.ContainsKey(key)) cacheList.Remove(key);
        }
        /// <summary>
        /// 增加缓存项目 
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        public void Insert(string key, object obj) { Insert(key, obj, 1); }
        /// <summary>
        /// 增加缓存项目
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="seconds">缓存秒数</param>
        public void Insert(string key, object obj, int seconds) {
            Remove(key);
            CachedItem item = new CachedItem();
            item.StartTime = DateTime.Now;
            item.EndTime = DateTime.Now.AddSeconds(seconds * Factor);
            item.CacheData = obj;
            item.CacheType = obj.GetType().ToString();
            item.CacheKey = key;
            cacheList.Add(key, item);
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        public object Get(string key) {
            if (!cacheList.ContainsKey(key)) return null;
            CachedItem item = cacheList[key];
            return DateTime.Now.IsBetween(item.StartTime, item.EndTime) ? item.CacheData : null;
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        public T Get<T>(string key) {
            if (!cacheList.ContainsKey(key)) return default(T);
            CachedItem item = cacheList[key];
            return DateTime.Now.IsBetween(item.StartTime, item.EndTime) ? (T)item.CacheData : default(T);
        }
        /// <summary>
        /// 键是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>true/false</returns>
        public bool ContainsKey(string key) { return cacheList.ContainsKey(key); }
        /// <summary>
        /// 缓存压缩
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        /// <param name="seconds">缓存秒</param>
        public void Compress<T>(string key, T obj, int seconds) where T : class {
            Insert(key, obj.ToBytes().DeflateCompress(), seconds);
        }
        /// <summary>
        /// 缓存压缩
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        public void Compress<T>(string key, T obj) where T : class {
            Compress<T>(key, obj, 1);
        }

        /// <summary>
        /// 缓存解压
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T Decompress<T>(string key) where T : class {
            return ((byte[])Get(key)).DeflateDecompress().FromBytes<T>();
        }
        #endregion
    }
}
