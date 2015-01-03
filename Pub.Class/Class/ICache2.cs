//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;

namespace Pub.Class {
    /// <summary>
    /// 缓存接口
    /// 
    /// 修改纪录
    ///     2006.05.01 版本：1.0 livexy 创建此接口
    /// 
    /// </summary>
    public interface ICache2: IAddIn {
        /// <summary>
        /// 返回缓存对像列表
        /// </summary>
        /// <returns></returns>
        IList<CachedItem> GetList();
        /// <summary>
        /// 清空所有缓存项目
        /// </summary>
        void Clear();
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="pattern">缓存键正则匹配模式</param>
        void RemoveByPattern(string pattern);
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存键名</param>
        void Remove(string key);
        /// <summary>
        /// 增加缓存项目 
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        void Insert(string key, object obj);
        /// <summary>
        /// 增加缓存项目
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="seconds">缓存秒数</param>
        void Insert(string key, object obj, int seconds);
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        object Get(string key);
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key">缓存键名</param>
        /// <returns>返回缓存对象</returns>
        T Get<T>(string key);
        /// <summary>
        /// 键是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>true/false</returns>
        bool ContainsKey(string key);
        /// <summary>
        /// 缓存压缩
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        void Compress<T>(string key, T obj) where T : class ;
        /// <summary>
        /// 缓存压缩
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        /// <param name="seconds">缓存秒数</param>
        void Compress<T>(string key, T obj, int seconds) where T : class ;
        /// <summary>
        /// 缓存解压
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        T Decompress<T>(string key) where T : class ;
    }
}
