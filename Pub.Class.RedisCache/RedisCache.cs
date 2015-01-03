//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ServiceStack.Redis;
using Pub.Class;

namespace Pub.Class {
    /// <summary>
    /// Memcached缓存管理
    /// 
    /// 修改纪录
    ///     2009.11.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class RedisCache : ICache2 {
        #region 私有静态字段
        private static readonly string[] SingleHost = WebConfig.GetApp("RedisCache.SingleHost").Split(':');
        private static readonly string[] MasterHosts = WebConfig.GetApp("RedisCache.MasterHosts").Split(';');
        private static readonly string[] SlaveHosts = WebConfig.GetApp("RedisCache.SlaveHosts").Split(';');
        private static readonly bool UsePool = WebConfig.GetApp("RedisCache.UsePool").ToBool(false);
        private static IRedisClient client = null;
        private static IRedisClientsManager clients = null;
        /// <summary>
        /// 缓存因子
        /// </summary>
        private int Factor = 5;
        #endregion

        #region 构造器
        public RedisCache() {
            if (!SingleHost[0].IsNullEmpty()) {
                client = SingleHost.Length == 2 ? new RedisClient(SingleHost[0], SingleHost[0].ToInt(6379)) : new RedisClient(SingleHost[0]);
            } else if (!MasterHosts[0].IsNullEmpty()) {
                clients = UsePool ? CreatePooledRedisClientManager() : CreateBasicRedisClientManager();
            }
        }

        ~RedisCache() {
            if (!client.IsNull()) client.Dispose();
            if (!clients.IsNull()) clients.Dispose();
        }
        private IRedisClientsManager CreatePooledRedisClientManager() { return SlaveHosts[0].IsNullEmpty() ? new PooledRedisClientManager(MasterHosts) : new PooledRedisClientManager(MasterHosts, SlaveHosts); }
        private IRedisClientsManager CreateBasicRedisClientManager() { return SlaveHosts[0].IsNullEmpty() ? new BasicRedisClientManager(MasterHosts) : new BasicRedisClientManager(MasterHosts, SlaveHosts); }
        private IRedisClient GetClient() { return !client.IsNull() ? client : clients.GetClient(); }
        private IRedisClient GetReadOnlyClient() { return !client.IsNull() ? client : clients.GetReadOnlyClient(); }
        #endregion

        #region 静态方法
        public IList<CachedItem> GetList() {
            IList<CachedItem> list = new List<CachedItem>();
            var c = SlaveHosts[0].IsNullEmpty() ? GetClient() : Rand.RndInt(1, 3) == 1 ? GetClient() : GetReadOnlyClient();
            var list2 = c.GetAllKeys();
            foreach (string key in list2) {
                if (!ContainsKey(key)) continue;
                list.Add(new CachedItem(key, Get(key).GetType().ToString()));
            }
            return list;
        }
        /// <summary>
        /// 清空所有缓存项目
        /// </summary>
        public void Clear() {
            var c = GetClient();
            c.FlushAll();
            //using(c.AcquireLock("clear")){ }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="pattern">缓存键正则匹配模式</param>
        public void RemoveByPattern(string pattern) {
            var c = SlaveHosts[0].IsNullEmpty() ? GetClient() : Rand.RndInt(1, 3) == 1 ? GetClient() : GetReadOnlyClient();
            var list = c.GetAllKeys();
            foreach (string key in list) {
                if (!ContainsKey(key)) continue;
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
                if (regex.IsMatch(key)) Remove(key);
            }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存键名</param>
        public void Remove(string key) {
            var c = GetClient();
             c.Remove(key);
             //using (c.AcquireLock("remove")) { }
        }
        /// <summary>
        /// 增加缓存项目 
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        public void Insert(string key, object obj) {
            if (obj.IsNull()) return;
            var c = GetClient();
            c.Set(key, obj);
            //using (c.AcquireLock("set")) {  }
        }
        /// <summary>
        /// 增加缓存项目
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="seconds">缓存秒数</param>
        public void Insert(string key, object obj, int seconds) {
            if (obj.IsNull()) return;
            var c = GetClient();
            c.Set(key, obj, DateTime.Now.AddSeconds(Factor * seconds));
            //using (c.AcquireLock("set")) {  }
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        public object Get(string key) { 
            var c = SlaveHosts[0].IsNullEmpty() ? GetClient() : Rand.RndInt(1, 3) == 1 ? GetClient() : GetReadOnlyClient();
            return c.ContainsKey(key) ? c.GetValue(key) : null;
            //using (c.AcquireLock("get")) {  }
        }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        public T Get<T>(string key) { 
            var c = SlaveHosts[0].IsNullEmpty() ? GetClient() : Rand.RndInt(1, 3) == 1 ? GetClient() : GetReadOnlyClient();
            return c.ContainsKey(key) ? c.Get<T>(key) : default(T);
            //using (c.AcquireLock("get")) {  }
        }
        /// <summary>
        /// 键是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>true/false</returns>
        public bool ContainsKey(string key) {
            var c = SlaveHosts[0].IsNullEmpty() ? GetClient() : Rand.RndInt(1, 3) == 1 ? GetClient() : GetReadOnlyClient();
            return c.ContainsKey(key);
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
