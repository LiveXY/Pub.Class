//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;
using System.Reflection;

namespace Pub.Class {
    /// <summary>
    /// 缓存内容实体类
    /// 
    /// 修改纪录
    ///     2006.05.01 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    [Serializable]
    [EntityInfo("缓存内容实体类")]
    public class CachedItem {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CachedItem() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="type">类型</param>
        public CachedItem(string key, string type) { this.CacheKey = key; this.CacheType = type; }
        private string cacheKey;
        /// <summary>
        /// 缓存键
        /// </summary>
        [EntityInfo("缓存键")]
        public string CacheKey { get { return this.cacheKey; } set { this.cacheKey = value; } }
        private string cacheType;
        /// <summary>
        /// 缓存内容的数据类型
        /// </summary>
        [EntityInfo("数据类型")]
        public string CacheType { get { return this.cacheType; } set { this.cacheType = value; } }
        private object cacheData;
        /// <summary>
        /// 缓存内容
        /// </summary>
        [EntityInfo("内容")]
        public object CacheData { get { return this.cacheData; } set { this.cacheData = value; } }
        private DateTime startTime;
        /// <summary>
        /// 缓存内容开始时间
        /// </summary>
        [EntityInfo("开始时间")]
        public DateTime StartTime { get { return this.startTime; } set { this.startTime = value; } }
        private DateTime endTime;
        /// <summary>
        /// 缓存内容结束时间
        /// </summary>
        [EntityInfo("结束时间")]
        public DateTime EndTime { get { return this.endTime; } set { this.endTime = value; } }
    }

    /// <summary>
    /// 缓存管理
    /// 
    /// 修改纪录
    ///     2009.09.04 版本：1.1 livexy 修改对共享CACHE的支持
    ///     2006.05.01 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// IList&lt;CachedItem> list = Cache2.GetList();
    /// Cache2.Insert("testCache", 1); 5秒
    /// Msg.Write(Cache2.Get("testCache"));
    /// Cache2.Insert("testCache", 1, 2); 10秒
    /// Cache2.Remove("testCache");
    /// Cache2.RemoveByPattern("(.+?)Cache");
    /// Cache2.Clear();
    /// Cache2.Compress&lt;int>("testCache", 1); 压缩后CACHE 5秒
    /// Msg.Write(Cache2.Decompress&lt;int>("testCache"));
    /// </example>
    /// </code>
    /// </summary>
    public class Cache2 {
        //#region 静态只读变量
        /// <summary>
        /// 启用缓存 缓存分为WebCache/VelocityCache/MemcachedCache/MemcachedCache2
        /// </summary>
        private static readonly string PubClassCache = WebConfig.GetApp("PubClassCache") ?? "WebCache";
        /// <summary>
        /// 不同的缓存调用不同的DLL
        /// </summary>
        private static ICache2 _cache = (ICache2)"Pub.Class.{0},Pub.Class.{0}".FormatWith(PubClassCache).LoadClass();
        //#endregion
        //#region 静态方法
        /// <summary>
        /// 取所有缓存对像
        /// </summary>
        /// <example>
        /// <code>
        /// IList&lt;CachedItem> list = Cache2.GetList();
        /// </code>
        /// </example>
        /// <returns>取所有缓存对像</returns>
        public static IList<CachedItem> GetList() { return _cache.GetList(); }
        /// <summary>
        /// 清空所有缓存项目
        /// </summary>
        /// <example>
        /// <code>
        /// Cache2.Clear();
        /// </code>
        /// </example>
        public static void Clear() { _cache.Clear(); }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <example>
        /// <code>
        /// Cache2.RemoveByPattern("TC_Test_SelectMyTestList_(.+?)");
        /// Cache2.RemoveByPattern("TC_Test_SelectMyTestList_(d+)");
        /// Cache2.RemoveByPattern("TC_Test_SelectMyTestList_[\\s\\S]*");
        /// </code>
        /// </example>
        /// <param name="pattern">缓存键正则匹配模式</param>
        public static void RemoveByPattern(string pattern) { _cache.RemoveByPattern(pattern); }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <example>
        /// <code>
        /// Cache2.Remove("TC_Test_SelectMyTestList_1");
        /// </code>
        /// </example>
        /// <param name="key">缓存键名</param>
        public static void Remove(string key) { _cache.Remove(key); }
        /// <summary>
        /// 增加缓存项目 
        /// </summary>
        /// <example>
        /// <code>
        /// Cache2.Insert("test2", 10);
        /// </code>
        /// </example>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        public static void Insert(string key, object obj) { _cache.Insert(key, obj); }
        /// <summary>
        /// 增加缓存项目
        /// </summary>
        /// <example>
        /// <code>
        /// Cache2.Insert("test2", 10, 10);
        /// </code>
        /// </example>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="seconds">缓存秒数</param>
        public static void Insert(string key, object obj, int seconds) { _cache.Insert(key, obj, seconds); }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <example>
        /// <code>
        /// Cache2.Get("test2");
        /// AC_Ask ask = (AC_Ask)Cache2.Get("AC_AskCache_SelectByID_1");
        /// </code>
        /// </example>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        public static object Get(string key) { return _cache.Get(key); }
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <example>
        /// <code>
        /// Cache2.Get("test2");
        /// AC_Ask ask = (AC_Ask)Cache2.Get("AC_AskCache_SelectByID_1");
        /// </code>
        /// </example>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        public static T Get<T>(string key) { return ContainsKey(key) ? _cache.Get<T>(key) : default(T); }
        /// <summary>
        /// 键是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>true/false</returns>
        public static bool ContainsKey(string key) { return _cache.ContainsKey(key); }
        /// <summary>
        /// 缓存压缩
        /// </summary>
        /// <example>
        /// <code>
        /// Cache2.Compress&lt;IList&lt;CachedItem>>("test", new List&lt;CachedItem>() { new CachedItem() { CacheKey = "1", CacheType = "string" }, new CachedItem() { CacheKey = "1", CacheType = "string" } });
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        public static void Compress<T>(string key, T obj) where T : class  { _cache.Compress<T>(key, obj); }
        /// <summary>
        /// 缓存压缩
        /// </summary>
        /// <example>
        /// <code>
        /// Cache2.Compress&lt;IList&lt;CachedItem>>("test", new List&lt;CachedItem>() { new CachedItem() { CacheKey = "1", CacheType = "string" }, new CachedItem() { CacheKey = "1", CacheType = "string" } }, 100);
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        /// <param name="seconds">缓存秒数</param>
        public static void Compress<T>(string key, T obj, int seconds) where T : class  { _cache.Compress<T>(key, obj, seconds); }
        /// <summary>
        /// 缓存解压
        /// </summary>
        /// <example>
        /// <code>
        /// IList&lt;CachedItem> list = Cache2.Decompress&lt;IList&lt;CachedItem>>("test");
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>缓存解压</returns>
        public static T Decompress<T>(string key) where T : class  { return _cache.Decompress<T>(key); }
        //#endregion
        /// <summary>
        /// 读与写CACHE
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="acquire">动作</param>
        /// <returns></returns>
        public static T Get<T>(string key, Func<T> acquire) {
            return Get(key, 1, acquire);
        }
        /// <summary>
        /// 读与写CACHE
        /// </summary>
        /// <example>
        /// <code>
        /// var list = Cache2.Get&lt;IList&lt;CachedItem>>(key, 1440, () => {
        ///     return Cache2.GetList();
        /// })
        /// </code>
        /// </example>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="cacheTime">时间</param>
        /// <param name="acquire">动作</param>
        /// <returns></returns>
        public static T Get<T>(string key, int cacheTime, Func<T> acquire) {
            if (Cache2.ContainsKey(key)) return Cache2.Get<T>(key);

            T result = acquire();
            _cache.Insert(key, result, cacheTime);
            return result;
        }
        /// <summary>
        /// 删除CACHE
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="delCache"></param>
        public static void Remove(string prefix, string[] delCache) {
            if (delCache.IsNull()) return;
            foreach (string s in delCache) {
                if (s.IndexOf("(") == -1 || s.IndexOf("[") == -1)
                    Cache2.Remove(s.IndexOf("Cache_") == -1 ? prefix + s : s);
                else
                    Cache2.RemoveByPattern(s.IndexOf("Cache_") == -1 ? "(" + prefix + s + ")" : s);
            }
        }
        /// <summary>
        /// 使用外部插件
        /// </summary>
        /// <param name="dllFileName">dll文件名</param>
        /// <param name="className">命名空间.类名</param>
        public static void Use(string dllFileName, string className) {
            _cache = (ICache2)dllFileName.LoadClass(className);
        }
        /// <summary>
        /// 使用外部插件
        /// </summary>
        /// <param name="classNameAndAssembly">命名空间.类名,程序集名称</param>
        public static void Use(string classNameAndAssembly) {
            if (classNameAndAssembly.IsNullEmpty())
                _cache = (ICache2)"Pub.Class.WebCache,Pub.Class.WebCache".LoadClass();
            else
                _cache = (ICache2)classNameAndAssembly.LoadClass();
        }
        /// <summary>
        /// 使用外部插件
        /// </summary>
        public static void Use<T>(T t) where T : ICache2, new() {
            _cache = Singleton<T>.Instance();
        }
    }
}
