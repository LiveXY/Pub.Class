//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;

namespace Pub.Class {
    /// <summary>
    /// 自动释放
    /// 
    /// 修改纪录
    ///     2010.05.01 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// //使用方法：
    /// public class PagerSQLHelper : Disposable { } 
    /// //需要重写InternalDispose方法
    /// //可用using调用： 
    /// using (PagerSQLHelper s = new PagerSQLHelper(PagerSQLEnum.top_top)) { }
    /// </code>
    /// </example>
    /// </summary>
    public abstract class Disposable : IDisposable {
        private bool disposed;
        /// <summary>
        /// 析构函数
        /// </summary>
        [DebuggerStepThrough]
        ~Disposable() {
            Dispose(false);
        }
        /// <summary>
        /// 释放
        /// </summary>
        [DebuggerStepThrough]
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 内部释放 可重写
        /// </summary>
        [DebuggerStepThrough]
        protected virtual void InternalDispose() { }
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">disposing</param>
        [DebuggerStepThrough]
        private void Dispose(bool disposing) {
            if (!disposed && disposing) InternalDispose();
            disposed = true;
        }
    }
#if !NET20
    /// <summary>
    /// 写锁自动释放
    /// 
    /// 修改纪录
    ///     2012.03.06 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
    /// using (new WriteLockSlimDisposable(Locker)) { }
    /// </code>
    /// </example>
    /// </summary>
    public class WriterLockSlimDisposable : IDisposable {
        private readonly ReaderWriterLockSlim _rwLock;
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="rwLock">ReaderWriterLockSlim 锁</param>
        public WriterLockSlimDisposable(ReaderWriterLockSlim rwLock) {
            _rwLock = rwLock;
            _rwLock.EnterWriteLock();
        }
        /// <summary>
        /// 释放
        /// </summary>
        void IDisposable.Dispose() {
            _rwLock.ExitWriteLock();
        }
    }
    /// <summary>
    /// 读锁自动释放
    /// 
    /// 修改纪录
    ///     2012.03.06 版本：1.0 livexy 创建此类
    /// 
    /// <example>
    /// <code>
    /// private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
    /// using (new ReaderLockSlimDisposable(Locker)) { }
    /// </code>
    /// </example>
    /// </summary>
    public class ReaderLockSlimDisposable : IDisposable {
        private readonly ReaderWriterLockSlim _rwLock;
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="rwLock">ReaderWriterLockSlim 锁</param>
        public ReaderLockSlimDisposable(ReaderWriterLockSlim rwLock) {
            _rwLock = rwLock;
            _rwLock.EnterReadLock();
        }
        /// <summary>
        /// 释放
        /// </summary>
        void IDisposable.Dispose() {
            _rwLock.ExitReadLock();
        }
    }
#endif
}
