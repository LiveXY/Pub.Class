//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Pub.Class {
    /// <summary>
    /// Web缓存管理
    /// 
    /// 修改纪录
    ///     2009.11.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class WebCache : ICache2 {
        #region 构造器

        public WebCache() {
            HttpContext context = HttpContext.Current;
            if (context != null) { _cache = (Cache)context.Cache; } else { _cache = HttpRuntime.Cache; }
        }
        #endregion

        #region 私有静态字段
        private readonly Cache _cache;
        /// <summary>
        /// 缓存因子
        /// </summary>
        private int Factor = 5;
        #endregion

        #region 静态方法
        public IList<CachedItem> GetList() {
            IList<CachedItem> list = new List<CachedItem>();
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            while (CacheEnum.MoveNext()) {
                list.Add(new CachedItem(CacheEnum.Key.ToString(), CacheEnum.Value.GetType().ToString()));
            }
            return list;
        }
        /// <summary>
        /// 重新设置缓存因子 
        /// </summary>
        /// <param name="cacheFactor"></param>
        public void ReSetFactor(int cacheFactor) { Factor = cacheFactor; }
        /// <summary>
        /// 清空所有缓存项目
        /// </summary>
        public void Clear() {
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            ArrayList al = new ArrayList();
            while (CacheEnum.MoveNext()) { al.Add(CacheEnum.Key); }

            foreach (string key in al) { _cache.Remove(key); }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="pattern">缓存键正则匹配模式</param>
        public void RemoveByPattern(string pattern) {
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            while (CacheEnum.MoveNext()) {
                if (regex.IsMatch(CacheEnum.Key.ToString())) _cache.Remove(CacheEnum.Key.ToString());
            }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存键名</param>
        public void Remove(string key) { _cache.Remove(key); }
        /// <summary>
        /// 增加缓存项目 
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        public void Insert(string key, object obj) { Insert(key, obj, null, 1); }
        /// <summary>
        /// 增加缓存项目
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="seconds">缓存秒数</param>
        public void Insert(string key, object obj, int seconds) { Insert(key, obj, null, seconds); }
        /// <summary>
        /// 增加缓存项目
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="seconds">缓存秒数</param>
        /// <param name="priority">缓存优先级</param>
        public void Insert(string key, object obj, int seconds, CacheItemPriority priority) { Insert(key, obj, null, seconds, priority); }
        /// <summary>
        /// 增加缓存项目
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="dep">缓存依赖项</param>
        /// <param name="seconds">缓存秒数</param>
        public void Insert(string key, object obj, CacheDependency dep, int seconds) { Insert(key, obj, dep, seconds, CacheItemPriority.Normal); }
        /// <summary>
        /// 增加缓存
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="dep">缓存依赖项</param>
        /// <param name="seconds">缓存秒数</param>
        /// <param name="priority">缓存优先级</param>
        public void Insert(string key, object obj, CacheDependency dep, int seconds, CacheItemPriority priority) {
            if (obj != null) {
                _cache.Insert(key, obj, dep, DateTime.Now.AddSeconds(Factor * seconds), Cache.NoSlidingExpiration, priority, null);
            }
        }
        /// <summary>
        /// 微小缓存
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="secondFactor">缓存秒因子</param>
        public void MicroInsert(string key, object obj, int secondFactor) {
            if (obj != null) {
                _cache.Insert(key, obj, null, DateTime.Now.AddSeconds(Factor * secondFactor), Cache.NoSlidingExpiration);
            }
        }

        /// <summary>
        /// 增加缓存项目
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public void Max(string key, object obj) { Max(key, obj, null); }
        /// <summary>
        /// 最大缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="dep">缓存依赖项</param>
        public void Max(string key, object obj, CacheDependency dep) {
            if (obj != null) {
                _cache.Insert(key, obj, dep, DateTime.MaxValue, Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
            }
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        public object Get(string key) { return _cache[key]; }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        public T Get<T>(string key) { return (T)_cache[key]; }
        /// <summary>
        /// 键是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>true/false</returns>
        public bool ContainsKey(string key) { return _cache[key].IsNotNull(); }
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
