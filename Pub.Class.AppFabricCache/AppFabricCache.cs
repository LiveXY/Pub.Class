//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using Microsoft.ApplicationServer.Caching;

namespace Pub.Class {
    /// <summary>
    /// Velocity缓存管理
    /// 
    /// 修改纪录
    ///     2009.11.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class AppFabricCache : ICache2 {
        #region 私有静态字段
        public static readonly string velocityCacheName = WebConfig.GetApp("AppFabricCache.Name") ?? string.Empty;
        private readonly DataCache _cache;
        private readonly DataCacheFactory _factory;
        private static readonly string RegionName = WebConfig.GetApp("AppFabricCache.Region").IfNullOrEmpty("PubClassRegion");
        private static readonly string[] Servers = WebConfig.GetApp("AppFabricCache.Servers").Split(';');
        private static readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        /// <summary>
        /// 缓存因子
        /// </summary>
        private int Factor = 5;
        #endregion

        #region 构造器
        public AppFabricCache() {
            if (!velocityCacheName.IsNullEmpty()) {
                DataCacheServerEndpoint[] cluster = GetClusterEndpoints();
                DataCacheFactoryConfiguration cfg = new DataCacheFactoryConfiguration();
                cfg.Servers = cluster;
                cfg.LocalCacheProperties = new DataCacheLocalCacheProperties();
                cfg.SecurityProperties = new DataCacheSecurity(DataCacheSecurityMode.None, DataCacheProtectionLevel.None);

                _factory = new DataCacheFactory(cfg);
                _cache = _factory.GetCache(velocityCacheName);
                try { _cache.CreateRegion(RegionName); } catch { }
            }
        }
        private DataCacheServerEndpoint[] GetClusterEndpoints() {
            DataCacheServerEndpoint[] cacheCluster = new DataCacheServerEndpoint[Servers.Length];
            int i = 0;
            foreach(string server in Servers) {
                string[] s = server.Split(':');
                string host = s[0];
                int port = s.Length == 2 ? s[1].ToInt(22233) : 22233;
                cacheCluster[i] = new DataCacheServerEndpoint(host, port);
                i++;
            }
            return cacheCluster;
        }
        #endregion

        #region 静态方法
        public IList<CachedItem> GetList() {
            IList<CachedItem> list = new List<CachedItem>();
            IEnumerable<KeyValuePair<string, object>> result;
            using (new ReaderLockSlimDisposable(locker)) { result = _cache.GetObjectsInRegion(RegionName); }

            foreach (var info in result) {
                list.Add(new CachedItem(info.Key, info.Value.GetType().ToString()));
            }
            return list;
        }
        /// <summary>
        /// 清空所有缓存项目
        /// </summary>
        public void Clear() {
            using (new WriterLockSlimDisposable(locker)) { _cache.ClearRegion(RegionName); }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="pattern">缓存键正则匹配模式</param>
        public void RemoveByPattern(string pattern) {
            IEnumerable<KeyValuePair<string, object>> result;
            using (new ReaderLockSlimDisposable(locker)) { result = _cache.GetObjectsInRegion(RegionName); }

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            foreach (var info in result) {
                if (regex.IsMatch(info.Key)) _cache.Remove(info.Key, RegionName);
            }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存键名</param>
        public void Remove(string key) {
            if (!ContainsKey(key)) return;
            using (new WriterLockSlimDisposable(locker)) {
                _cache.Remove(key, RegionName);
            }
        }
        /// <summary>
        /// 增加缓存项目 
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        public void Insert(string key, object obj) {
            if (obj.IsNull()) { Remove(key); return; }
            using (new WriterLockSlimDisposable(locker)) {
                _cache.Put(key, obj, DateTime.Now.GetTimeSpan(DateTime.Now.AddSeconds(Factor)), RegionName);
            }
        }
        public void Add(string key, object obj) {
            if (obj.IsNull()) { Remove(key); return; }
            using (new WriterLockSlimDisposable(locker)) {
                _cache.Add(key, obj, DateTime.Now.GetTimeSpan(DateTime.Now.AddSeconds(Factor)), RegionName);
            }
        }
        /// <summary>
        /// 增加缓存项目
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="seconds">缓存秒数</param>
        public void Insert(string key, object obj, int seconds) {
            if (obj.IsNull()) { Remove(key); return; }
            using (new WriterLockSlimDisposable(locker)) {
                _cache.Put(key, obj, DateTime.Now.GetTimeSpan(DateTime.Now.AddSeconds(Factor * seconds)), RegionName);  //TimeSpan.FromSeconds();
            }
        }
        public void Add(string key, object obj, int seconds) {
            if (obj.IsNull()) { Remove(key); return; }
            using (new WriterLockSlimDisposable(locker)) {
                _cache.Add(key, obj, DateTime.Now.GetTimeSpan(DateTime.Now.AddSeconds(Factor * seconds)), RegionName);
            }
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        public object Get(string key) {
            using (new ReaderLockSlimDisposable(locker)) {
                return _cache.Get(key, RegionName);
            }
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        public T Get<T>(string key) {
            using (new ReaderLockSlimDisposable(locker)) {
                return (T)_cache.Get(key, RegionName);
            }
        }
        /// <summary>
        /// 键是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>true/false</returns>
        public bool ContainsKey(string key) {
            using (new ReaderLockSlimDisposable(locker)) {
                return _cache.Get(key, RegionName).IsNotNull();
            }
        }
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
