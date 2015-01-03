//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;
using Memcached.ClientLibrary;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Pub.Class {
    /// <summary>
    /// Memcached缓存管理
    /// 
    /// 修改纪录
    ///     2009.11.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class MemcachedCache : ICache2 {
        #region 私有静态字段
        public static readonly ArrayList memcachedServer = new ArrayList();
        private static SockIOPool pool = null;
        private static MemcachedClient mc = null;
        /// <summary>
        /// 缓存因子
        /// </summary>
        private int Factor = 5;
        #endregion

        #region 构造器
        public MemcachedCache() {
            foreach (string info in (WebConfig.GetApp("MemcachedCache.MemcachedServer") ?? string.Empty).Split(';')) if (!string.IsNullOrEmpty(info.Trim())) memcachedServer.Add(info);

            if (pool.IsNull() && memcachedServer.Count > 0) {
                pool = SockIOPool.GetInstance();
                pool.SetServers(memcachedServer);

                pool.InitConnections = Convert.ToInt32(WebConfig.GetApp("MemcachedCache.InitConnections") ?? "3");
                pool.MinConnections = Convert.ToInt32(WebConfig.GetApp("MemcachedCache.MinConnections") ?? "3");
                pool.MaxConnections = Convert.ToInt32(WebConfig.GetApp("MemcachedCache.MaxConnections") ?? "5");
                pool.SocketConnectTimeout = Convert.ToInt32(WebConfig.GetApp("MemcachedCache.SocketConnectTimeout") ?? "1000");
                pool.SocketTimeout = Convert.ToInt32(WebConfig.GetApp("MemcachedCache.SocketTimeout") ?? "3000");
                pool.MaintenanceSleep = Convert.ToInt32(WebConfig.GetApp("MemcachedCache.MaintenanceSleep") ?? "30");
                pool.Failover = (WebConfig.GetApp("MemcachedCache.Failover") ?? "") == "true" ? true : false;
                pool.Nagle = (WebConfig.GetApp("MemcachedCache.Nagle") ?? "") == "true" ? true : false;
                //pool.HashingAlgorithm = HashingAlgorithm.NewCompatibleHash;
                pool.Initialize();

                mc = new MemcachedClient();
                mc.PoolName = WebConfig.GetApp("MemcachedCache.PoolName") ?? "default";
                mc.EnableCompression = (WebConfig.GetApp("MemcachedCache.EnableCompression") ?? "") == "true" ? true : false;
            }
        }

        ~MemcachedCache() {
            if (memcachedServer.Count > 0 && !pool.IsNull()) {
                pool.Shutdown();
            }
        }
        #endregion

        #region 静态方法
        public IList<CachedItem> GetList() {
            IList<CachedItem> list = new List<CachedItem>();
            ArrayList cacheItem = GetStats(memcachedServer, Stats.Items, "");
            foreach (string item in cacheItem) {
                if (item.IndexOf(":number:") < 0) continue;
                ArrayList cachearr = GetStats(memcachedServer, Stats.CachedDump, item.Split(':')[1] + " 0");
                foreach (string cache in cachearr) {
                    string cacheName = cache.Split(':')[0];
                    if (!mc.KeyExists(cacheName)) continue;
                    list.Add(new CachedItem(cacheName, Get(cacheName).GetType().ToString()));
                }
            }
            return list;
        }
        public ArrayList GetStats() {
            ArrayList arrayList = new ArrayList();
            foreach (string server in memcachedServer) {
                arrayList.Add(server);
            }
            return GetStats(arrayList, Stats.Default, null);
        }
        public ArrayList GetStats(ArrayList serverArrayList, Stats statsCommand, string param) {
            ArrayList statsArray = new ArrayList();
            param = string.IsNullOrEmpty(param) ? "" : param.Trim().ToLower();

            string commandstr = "stats";
            //转换stats命令参数
            switch (statsCommand) {
                case Stats.Reset: { commandstr = "stats reset"; break; }
                case Stats.Malloc: { commandstr = "stats malloc"; break; }
                case Stats.Maps: { commandstr = "stats maps"; break; }
                case Stats.Sizes: { commandstr = "stats sizes"; break; }
                case Stats.Slabs: { commandstr = "stats slabs"; break; }
                case Stats.Items: { commandstr = "stats items"; break; }
                case Stats.CachedDump: {
                        string[] statsparams = param.Split(" ");
                        if (statsparams.Length == 2)
                            if (statsparams.IsNumberArray())
                                commandstr = "stats cachedump  " + param;

                        break;
                    }
                case Stats.Detail: {
                        if (string.Equals(param, "on") || string.Equals(param, "off") || string.Equals(param, "dump"))
                            commandstr = "stats detail " + param.Trim();

                        break;
                    }
                default: { commandstr = "stats"; break; }
            }
            //加载返回值
            Hashtable stats = mc.Stats(serverArrayList, commandstr);
            foreach (string key in stats.Keys) {
                statsArray.Add(key);
                Hashtable values = (Hashtable)stats[key];
                foreach (string key2 in values.Keys) {
                    statsArray.Add(key2 + ":" + values[key2]);
                }
            }
            return statsArray;
        }

        /// <summary>
        /// Stats命令行参数
        /// </summary>
        public enum Stats {
            /// <summary>
            /// stats : 显示服务器信息, 统计数据等
            /// </summary>
            Default = 0,
            /// <summary>
            /// stats reset : 清空统计数据
            /// </summary>
            Reset = 1,
            /// <summary>
            /// stats malloc : 显示内存分配数据
            /// </summary>
            Malloc = 2,
            /// <summary>
            /// stats maps : 显示"/proc/self/maps"数据
            /// </summary>
            Maps = 3,
            /// <summary>
            /// stats sizes
            /// </summary>
            Sizes = 4,
            /// <summary>
            /// stats slabs : 显示各个slab的信息,包括chunk的大小,数目,使用情况等
            /// </summary>
            Slabs = 5,
            /// <summary>
            /// stats items : 显示各个slab中item的数目和最老item的年龄(最后一次访问距离现在的秒数)
            /// </summary>
            Items = 6,
            /// <summary>
            /// stats cachedump slab_id limit_num : 显示某个slab中的前 limit_num 个 key 列表
            /// </summary>
            CachedDump = 7,
            /// <summary>
            /// stats detail [on|off|dump] : 设置或者显示详细操作记录   on:打开详细操作记录  off:关闭详细操作记录 dump: 显示详细操作记录(每一个键值get,set,hit,del的次数)
            /// </summary>
            Detail = 8
        }
        /// <summary>
        /// 清空所有缓存项目
        /// </summary>
        public void Clear() {
            mc.FlushAll();
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="pattern">缓存键正则匹配模式</param>
        public void RemoveByPattern(string pattern) {
            ArrayList cacheItem = GetStats(memcachedServer, Stats.Items, "");
            foreach (string item in cacheItem) {
                if (item.IndexOf(":number:") < 0) continue;
                ArrayList cachearr = GetStats(memcachedServer, Stats.CachedDump, item.Split(':')[1] + " 0");
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
                foreach (string cache in cachearr) {
                    string cacheName = cache.Split(':')[0];
                    if (!mc.KeyExists(cacheName)) continue;
                    if (regex.IsMatch(cacheName)) Remove(cacheName);
                }
            }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存键名</param>
        public void Remove(string key) { mc.Delete(key); }
        /// <summary>
        /// 增加缓存项目 
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        public void Insert(string key, object obj) { if (obj != null) mc.Set(key, obj); }
        public void Add(string key, object obj) { if (obj != null && !mc.KeyExists(key)) mc.Add(key, obj); }
        /// <summary>
        /// 增加缓存项目
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="seconds">缓存秒数</param>
        public void Insert(string key, object obj, int seconds) {
            if (obj != null) mc.Set(key, obj, DateTime.Now.AddSeconds(Factor * seconds));
        }
        public void Add(string key, object obj, int seconds) {
            if (obj != null && !mc.KeyExists(key)) mc.Add(key, obj, DateTime.Now.AddSeconds(Factor * seconds)); //TimeSpan.FromSeconds();
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        public object Get(string key) { return mc.KeyExists(key) ? mc.Get(key) : null; }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        public T Get<T>(string key) { return mc.KeyExists(key) ? (T)mc.Get(key) : default(T); }
        /// <summary>
        /// 键是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>true/false</returns>
        public bool ContainsKey(string key) { return mc.KeyExists(key); }
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
