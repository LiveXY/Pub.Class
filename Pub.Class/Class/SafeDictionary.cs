//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif
using System.Text;
using System.Collections;
using System.Threading;

namespace Pub.Class {
    /// <summary>
    /// 安全Dictionary接口
    /// 
    /// 修改纪录
    ///     2012.03.02 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// private static ISafeDictionary&lt;string, object> classCache = new SafeDictionarySlim&lt;string, object>();
    /// </example>
    /// </code>
    /// </summary>
    /// <typeparam name="K">key类型</typeparam>
    /// <typeparam name="V">value类型</typeparam>
    public interface ISafeDictionary<K, V> : IEnumerable<KeyValuePair<K, V>> {
        /// <summary>
        /// 取所有key
        /// </summary>
        ICollection<K> Keys { get; }
        /// <summary>
        /// 取所有value
        /// </summary>
        ICollection<V> Values { get; }
        /// <summary>
        /// 获取或设置具有指定键的元素
        /// </summary>
        /// <param name="key">要获取或设置的元素的键</param>
        /// <returns>带有指定键的元素</returns>
        V this[K key] { get; set; }
        /// <summary>
        /// 元素数量
        /// </summary>
        /// <returns></returns>
        int Count { get; }
        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        ISafeDictionary<K, V> Add(K key, V value);
        /// <summary>
        /// 清空所有元素
        /// </summary>
        ISafeDictionary<K, V> Clear();
        /// <summary>
        /// 元素是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true/false</returns>
        bool ContainsKey(K key);
        /// <summary>
        /// 删除指定元素
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true/false</returns>
        bool Remove(K key);
        /// <summary>
        /// 取指定的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>true/false</returns>
        bool TryGetValue(K key, out V value);
        bool TryRemove(K key, out V value);
        bool TryUpdate(K key, V value);
        bool TryAdd(K key, V value);
        /// <summary>
        /// 添加或修改元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        ISafeDictionary<K, V> AddOrUpdate(K key, V value);
        /// <summary>
        /// 读与写
        /// </summary>
        /// <param name="key">K key</param>
        /// <param name="acquire">Func&lt;V></param>
        /// <returns>V</returns>
        V Get(K key, Func<V> acquire);
    }
    /// <summary>
    /// 安全Dictionary 使用lock
    /// 
    /// 修改纪录
    ///     2012.03.02 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// private static ISafeDictionary&lt;string, object> classCache = new SafeDictionary&lt;string, object>();
    /// </example>
    /// </code>
    /// </summary>
    /// <typeparam name="K">key类型</typeparam>
    /// <typeparam name="V">value类型</typeparam>
    public class SafeDictionary<K, V> : ISafeDictionary<K, V> {
        private readonly IDictionary<K, V> cache = new Dictionary<K, V>();
        private readonly object cacheLock = new object();
        /// <summary>
        /// 取所有key
        /// </summary>
        public ICollection<K> Keys {
            get {
                lock (cacheLock) {
                    return cache.Keys;
                }
            }
        }
        /// <summary>
        /// 取所有value
        /// </summary>
        public ICollection<V> Values {
            get {
                lock (cacheLock) {
                    return cache.Values;
                }
            }
        }
        /// <summary>
        /// 获取或设置具有指定键的元素
        /// </summary>
        /// <param name="key">要获取或设置的元素的键</param>
        /// <returns>带有指定键的元素</returns>
        public V this[K key] {
            get {
                lock (cacheLock) {
                    return cache[key];
                }
            }
            set {
                lock (cacheLock) {
                    cache[key] = value;
                }
            }
        }
        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count {
            get {
                lock (cacheLock) {
                    return cache.Count;
                }
            }
        }
        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public ISafeDictionary<K, V> Add(K key, V value) {
            lock (cacheLock) {
                cache.Add(key, value);
                return this;
            }
        }
        /// <summary>
        /// 清空所有元素
        /// </summary>
        public ISafeDictionary<K, V> Clear() {
            lock (cacheLock) {
                cache.Clear();
                return this;
            }
        }
        /// <summary>
        /// 元素是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true/false</returns>
        public bool ContainsKey(K key) {
            lock (cacheLock) {
                return cache.ContainsKey(key);
            }
        }
        /// <summary>
        /// 删除指定元素
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true/false</returns>
        public bool Remove(K key) {
            lock (cacheLock) {
                return cache.Remove(key);
            }
        }
        /// <summary>
        /// 取指定的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>true/false</returns>
        public bool TryGetValue(K key, out V value) {
            lock (cacheLock) {
                return cache.TryGetValue(key, out value);
            }
        }
        public bool TryRemove(K key, out V value){
            value = default(V);
            if (!ContainsKey(key)) return false;
            value = this[key];
            Remove(key);
            return true;
        }
        public bool TryUpdate(K key, V value) {
            if (!ContainsKey(key)) return false;
            this[key] = value;
            return true;
        }
        public bool TryAdd(K key, V value) {
            if (ContainsKey(key)) return false;
            Add(key, value);
            return true;
        }
        /// <summary>
        /// 添加或修改元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public ISafeDictionary<K, V> AddOrUpdate(K key, V value) {
            if (!ContainsKey(key)) Add(key, value);
            else this[key] = value;
            return this;
        }
        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() {
            return GetEnumeratorImpl();
        }
        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumeratorImpl();
        }
        private IEnumerator<KeyValuePair<K, V>> GetEnumeratorImpl() {
            lock (cacheLock) {
                return cache.ToList().GetEnumerator();
            }
        }
        /// <summary>
        /// 读与写
        /// </summary>
        /// <param name="key">K key</param>
        /// <param name="acquire">Func&lt;V></param>
        /// <returns>V</returns>
        public V Get(K key, Func<V> acquire) {
            if (cache.ContainsKey(key)) return cache[key];

            V result = acquire();
            cache[key] = result;
            return result;
        }
    }
#if !NET20
    /// <summary>
    /// 安全Dictionary 使用ReaderWriterLockSlim
    /// 
    /// 修改纪录
    ///     2012.03.02 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// private static ISafeDictionary&lt;string, object> classCache = new SafeDictionarySlim&lt;string, object>();
    /// </example>
    /// </code>
    /// </summary>
    /// <typeparam name="K">key类型</typeparam>
    /// <typeparam name="V">value类型</typeparam>
    public class SafeDictionarySlim<K, V> : ISafeDictionary<K, V> {
        private readonly IDictionary<K, V> cache = new Dictionary<K, V>();
        private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        /// <summary>
        /// 取所有key
        /// </summary>
        public ICollection<K> Keys {
            get {
                cacheLock.EnterReadLock();
                try {
                    return cache.Keys;
                } finally {
                    cacheLock.ExitReadLock();
                }
            }
        }
        /// <summary>
        /// 取所有value
        /// </summary>
        public ICollection<V> Values {
            get {
                cacheLock.EnterReadLock();
                try {
                    return cache.Values;
                } finally {
                    cacheLock.ExitReadLock();
                }
            }
        }
        /// <summary>
        /// 获取或设置具有指定键的元素
        /// </summary>
        /// <param name="key">要获取或设置的元素的键</param>
        /// <returns>带有指定键的元素</returns>
        public V this[K key] {
            get {
                cacheLock.EnterReadLock();
                try {
                    return cache[key];
                } finally {
                    cacheLock.ExitReadLock();
                }
            }
            set {
                cacheLock.EnterWriteLock();
                try {
                    cache[key] = value;
                } finally {
                    cacheLock.ExitWriteLock();
                }
            }
        }
        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count {
            get {
                cacheLock.EnterReadLock();
                try {
                    return cache.Count;
                } finally {
                    cacheLock.ExitReadLock();
                }
            }
        }
        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public ISafeDictionary<K, V> Add(K key, V value) {
            cacheLock.EnterWriteLock();
            try {
                cache.Add(key, value);
            } finally {
                cacheLock.ExitWriteLock();
            }
            return this;
        }
        /// <summary>
        /// 清空所有元素
        /// </summary>
        public ISafeDictionary<K, V> Clear() {
            cacheLock.EnterWriteLock();
            try {
                cache.Clear();
            } finally {
                cacheLock.ExitWriteLock();
            }
            return this;
        }
        /// <summary>
        /// 元素是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true/false</returns>
        public bool ContainsKey(K key) {
            cacheLock.EnterReadLock();
            try {
                return cache.ContainsKey(key);
            } finally {
                cacheLock.ExitReadLock();
            }
        }
        /// <summary>
        /// 删除指定元素
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true/false</returns>
        public bool Remove(K key) {
            cacheLock.EnterWriteLock();
            try {
                return cache.Remove(key);
            } finally {
                cacheLock.ExitWriteLock();
            }
        }
        /// <summary>
        /// 取指定的元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>true/false</returns>
        public bool TryGetValue(K key, out V value) {
            cacheLock.EnterReadLock();
            try {
                return cache.TryGetValue(key, out value);
            } finally {
                cacheLock.ExitReadLock();
            }
        }
        public bool TryRemove(K key, out V value){
            value = default(V);
            if (!ContainsKey(key)) return false;
            value = this[key];
            Remove(key);
            return true;
        }
        public bool TryUpdate(K key, V value) {
            if (!ContainsKey(key)) return false;
            this[key] = value;
            return true;
        }
        public bool TryAdd(K key, V value) {
            if (ContainsKey(key)) return false;
            Add(key, value);
            return true;
        }
        /// <summary>
        /// 添加或修改元素
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public ISafeDictionary<K, V> AddOrUpdate(K key, V value) {
            if (!ContainsKey(key)) Add(key, value);
            else this[key] = value;
            return this;
        }
        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() {
            return GetEnumeratorImpl();
        }
        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumeratorImpl();
        }
        private IEnumerator<KeyValuePair<K, V>> GetEnumeratorImpl() {
            cacheLock.EnterReadLock();
            try {
                return cache.ToList().GetEnumerator();
            } finally {
                cacheLock.ExitReadLock();
            }
        }
        /// <summary>
        /// 读与写
        /// </summary>
        /// <param name="key">K key</param>
        /// <param name="acquire">Func&lt;V></param>
        /// <returns>V</returns>
        public V Get(K key, Func<V> acquire) {
            if (cache.ContainsKey(key)) return cache[key];

            V result = acquire();
            cache[key] = result;
            return result;
        }
    }
#endif
    /// <summary>
    /// 安全List接口
    /// 
    /// 修改纪录
    ///     2012.03.02 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// private static ISafeList&lt;string> classCache = new SafeListSlim&lt;string>();
    /// </example>
    /// </code>
    /// </summary>
    /// <typeparam name="V">value类型</typeparam>
    public interface ISafeList<V> : IEnumerable<V> {
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index">要获得或设置的元素从零开始的索引。</param>
        /// <returns>指定索引处的元素。</returns>
        V this[int index] { get; set; }
        /// <summary>
        /// 元素数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="value">value</param>
        ISafeList<V> Add(V value);
        /// <summary>
        /// 添加唯一元素
        /// </summary>
        /// <param name="value">value</param>
        ISafeList<V> AddUnique(V value);
        /// <summary>
        /// 插入元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">value</param>
        ISafeList<V> Insert(int index, V value);
        /// <summary>
        /// 插入唯一元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">value</param>
        ISafeList<V> InsertUnique(int index, V value);
        /// <summary>
        /// 清空所有元素
        /// </summary>
        ISafeList<V> Clear();
        /// <summary>
        /// 元素是否存在
        /// </summary>
        /// <param name="value">元素</param>
        /// <returns>true/false</returns>
        bool Contains(V value);
        /// <summary>
        /// 首元素是否存在
        /// </summary>
        /// <param name="value">元素</param>
        /// <returns>返回索引号</returns>
        int IndexOf(V value);
        /// <summary>
        /// 尾元素是否存在
        /// </summary>
        /// <param name="value">元素</param>
        /// <returns>返回索引号</returns>
        int LastIndexOf(V value);
        /// <summary>
        /// 删除指定元素 只删除第一个匹配项
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>true/false</returns>
        bool Remove(V value);
        /// <summary>
        /// 删除指定元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>true/false</returns>
        ISafeList<V> Remove(int index);
        /// <summary>
        /// 元素的顺序反转
        /// </summary>
        /// <returns>this</returns>
        ISafeList<V> Reverse();
        /// <summary>
        /// 出栈
        /// </summary>
        /// <returns></returns>
        V Pop();
        /// <summary>
        /// 进栈
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        ISafeList<V> Push(V value);
    }
    /// <summary>
    /// 安全List 使用ReaderWriterLockSlim
    /// 
    /// 修改纪录
    ///     2012.03.08 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// private static SafeListSlim&lt;string> classCache = new SafeListSlim&lt;string>();
    /// </example>
    /// </code>
    /// </summary>
    /// <typeparam name="V">value类型</typeparam>
    public class SafeList<V>: ISafeList<V> {
        private readonly List<V> cache = new List<V>();
        private readonly object cacheLock = new object();
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index">要获得或设置的元素从零开始的索引。</param>
        /// <returns>指定索引处的元素。</returns>
        public V this[int index] {
            get {
                lock (cacheLock) {
                    return cache[index];
                }
            }
            set {
                lock (cacheLock) {
                    cache[index] = value;
                }
            }
        }
        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count {
            get {
                lock (cacheLock) {
                    return cache.Count;
                }
            }
        }
        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="value">value</param>
        public ISafeList<V> Add(V value) {
            lock (cacheLock) {
                cache.Add(value);
            }
            return this;
        }
        /// <summary>
        /// 添加唯一元素
        /// </summary>
        /// <param name="value">value</param>
        public ISafeList<V> AddUnique(V value) {
            if (Contains(value)) return this;
            return Add(value);
        }
        /// <summary>
        /// 插入元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">value</param>
        public ISafeList<V> Insert(int index, V value) {
            lock (cacheLock) {
                cache.Insert(index, value);
            }
            return this;
        }
        /// <summary>
        /// 插入唯一元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">value</param>
        public ISafeList<V> InsertUnique(int index, V value) {
            if (Contains(value)) return this;
            return Insert(index, value);
        }
        /// <summary>
        /// 清空所有元素
        /// </summary>
        public ISafeList<V> Clear() {
            lock (cacheLock) {
                cache.Clear();
            }
            return this;
        }
        /// <summary>
        /// 元素是否存在
        /// </summary>
        /// <param name="value">元素</param>
        /// <returns>true/false</returns>
        public bool Contains(V value) {
            return IndexOf(value) >= 0;
        }
        /// <summary>
        /// 首元素是否存在
        /// </summary>
        /// <param name="value">元素</param>
        /// <returns>返回索引号</returns>
        public int IndexOf(V value) {
            lock (cacheLock) {
                return cache.IndexOf(value);
            }
        }
        /// <summary>
        /// 尾元素是否存在
        /// </summary>
        /// <param name="value">元素</param>
        /// <returns>返回索引号</returns>
        public int LastIndexOf(V value) {
            lock (cacheLock) {
                return cache.LastIndexOf(value);
            }
        }
        /// <summary>
        /// 删除指定元素 只删除第一个匹配项
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>true/false</returns>
        public bool Remove(V value) {
            lock (cacheLock) {
                return cache.Remove(value);
            }
        }
        /// <summary>
        /// 删除指定元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>true/false</returns>
        public ISafeList<V> Remove(int index) {
            lock (cacheLock) {
                cache.RemoveAt(index);
            }
            return this;
        }
        /// <summary>
        /// 元素的顺序反转
        /// </summary>
        /// <returns>this</returns>
        public ISafeList<V> Reverse() {
            lock (cacheLock) {
                cache.Reverse();
            }
            return this;
        }
        /// <summary>
        /// 出栈
        /// </summary>
        /// <returns></returns>
        public V Pop() {
            if (cache.Count == 0) return default(V);
            V v = cache[0];
            Remove(0);
            return v;
        }
        /// <summary>
        /// 进栈
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public ISafeList<V> Push(V value) {
            return Add(value);
        }
        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumeratorImpl();
        }
        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator<V> IEnumerable<V>.GetEnumerator() {
            return GetEnumeratorImpl();
        }
        private IEnumerator<V> GetEnumeratorImpl() {
            lock (cacheLock) {
                return cache.ToList().GetEnumerator();
            }
        }
    }
#if !NET20
    /// <summary>
    /// 安全List 使用ReaderWriterLockSlim
    /// 
    /// 修改纪录
    ///     2012.03.08 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// private static SafeListSlim&lt;string> classCache = new SafeListSlim&lt;string>();
    /// </example>
    /// </code>
    /// </summary>
    /// <typeparam name="V">value类型</typeparam>
    public class SafeListSlim<V>: ISafeList<V> {
        private readonly List<V> cache = new List<V>();
        private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index">要获得或设置的元素从零开始的索引。</param>
        /// <returns>指定索引处的元素。</returns>
        public V this[int index] {
            get {
                cacheLock.EnterReadLock();
                try {
                    return cache[index];
                } finally {
                    cacheLock.ExitReadLock();
                }
            }
            set {
                cacheLock.EnterWriteLock();
                try {
                    cache[index] = value;
                } finally {
                    cacheLock.ExitWriteLock();
                }
            }
        }
        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count {
            get {
                cacheLock.EnterReadLock();
                try {
                    return cache.Count;
                } finally {
                    cacheLock.ExitReadLock();
                }
            }
        }
        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="value">value</param>
        public ISafeList<V> Add(V value) {
            cacheLock.EnterWriteLock();
            try {
                cache.Add(value);
            } finally {
                cacheLock.ExitWriteLock();
            }
            return this;
        }
        /// <summary>
        /// 添加唯一元素
        /// </summary>
        /// <param name="value">value</param>
        public ISafeList<V> AddUnique(V value) {
            if (Contains(value)) return this;
            return Add(value);
        }
        /// <summary>
        /// 插入元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">value</param>
        public ISafeList<V> Insert(int index, V value) {
            cacheLock.EnterWriteLock();
            try {
                cache.Insert(index, value);
            } finally {
                cacheLock.ExitWriteLock();
            }
            return this;
        }
        /// <summary>
        /// 插入唯一元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">value</param>
        public ISafeList<V> InsertUnique(int index, V value) {
            if (Contains(value)) return this;
            return Insert(index, value);
        }
        /// <summary>
        /// 清空所有元素
        /// </summary>
        public ISafeList<V> Clear() {
            cacheLock.EnterWriteLock();
            try {
                cache.Clear();
            } finally {
                cacheLock.ExitWriteLock();
            }
            return this;
        }
        /// <summary>
        /// 元素是否存在
        /// </summary>
        /// <param name="value">元素</param>
        /// <returns>true/false</returns>
        public bool Contains(V value) {
            return IndexOf(value) >= 0;
        }
        /// <summary>
        /// 首元素是否存在
        /// </summary>
        /// <param name="value">元素</param>
        /// <returns>返回索引号</returns>
        public int IndexOf(V value) {
            cacheLock.EnterReadLock();
            try {
                return cache.IndexOf(value);
            } finally {
                cacheLock.ExitReadLock();
            }
        }
        /// <summary>
        /// 尾元素是否存在
        /// </summary>
        /// <param name="value">元素</param>
        /// <returns>返回索引号</returns>
        public int LastIndexOf(V value) {
            cacheLock.EnterReadLock();
            try {
                return cache.LastIndexOf(value);
            } finally {
                cacheLock.ExitReadLock();
            }
        }
        /// <summary>
        /// 删除指定元素 只删除第一个匹配项
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>true/false</returns>
        public bool Remove(V value) {
            cacheLock.EnterWriteLock();
            try {
                return cache.Remove(value);
            } finally {
                cacheLock.ExitWriteLock();
            }
        }
        /// <summary>
        /// 删除指定元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>true/false</returns>
        public ISafeList<V> Remove(int index) {
            cacheLock.EnterWriteLock();
            try {
                cache.RemoveAt(index);
            } finally {
                cacheLock.ExitWriteLock();
            }
            return this;
        }
        /// <summary>
        /// 元素的顺序反转
        /// </summary>
        /// <returns>this</returns>
        public ISafeList<V> Reverse() {
            cacheLock.EnterWriteLock();
            try {
                cache.Reverse();
            } finally {
                cacheLock.ExitWriteLock();
            }
            return this;
        }
        /// <summary>
        /// 出栈
        /// </summary>
        /// <returns></returns>
        public V Pop() {
            if (cache.Count == 0) return default(V);
            V v = cache[0];
            Remove(0);
            return v;
        }
        /// <summary>
        /// 进栈
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public ISafeList<V> Push(V value) {
            return Add(value);
        }
        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumeratorImpl();
        }
        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator<V> IEnumerable<V>.GetEnumerator() {
            return GetEnumeratorImpl();
        }
        private IEnumerator<V> GetEnumeratorImpl() {
            cacheLock.EnterReadLock();
            try {
                return cache.ToList().GetEnumerator();
            } finally {
                cacheLock.ExitReadLock();
            }
        }
    }
#endif
}
