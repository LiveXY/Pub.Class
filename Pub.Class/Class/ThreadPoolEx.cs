//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace Pub.Class {
    #region ThreadEx
    /// <summary>
    /// 线程扩展
    /// </summary>
    public class ThreadEx {
        private class WorkItemInfo {
            public AutoResetEvent AutoResetEvent { get; set; }
            public WaitCallback WaitCallback { get; set; }
            public object UserState { get; set; }
        }
        private List<AutoResetEvent> handlerStack = new List<AutoResetEvent>();
        /// <summary>
        /// 队列
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="userstate"></param>
        public ThreadEx QueueWorkItem(WaitCallback callBack, object userstate) {
            WorkItemInfo info = new WorkItemInfo();
            info.AutoResetEvent = new AutoResetEvent(false);
            handlerStack.Add(info.AutoResetEvent);
            info.WaitCallback = callBack;
            info.UserState = userstate;

            ThreadPool.QueueUserWorkItem((state) => {
                WorkItemInfo workItemInfo = (WorkItemInfo)state;
                try {
                    workItemInfo.WaitCallback(workItemInfo.UserState);
                } finally { workItemInfo.AutoResetEvent.Set(); }
            }, info);
            return this;
        }

        public void SetAll() {
            foreach (AutoResetEvent handler in handlerStack) handler.Set();
        }
        /// <summary>
        /// 等待线程执行完成
        /// </summary>
        /// <example>
        ///     <code>
        ///         ThreadEx tpe = new ThreadEx();
        ///         for (int i = 100; i > 0; i--) {
        ///             tpe.QueueWorkItem((state) => {
        ///                 Console.WriteLine(state);
        ///             },i);
        ///         }
        ///         tpe.WaitAllComplete();
        ///         Console.WriteLine('执行完成！');
        ///     </code>
        /// </example>
        public void WaitAllComplete() {
            foreach (AutoResetEvent handler in handlerStack) handler.WaitOne();
        }
        /// <summary>
        /// 多个线程并行执行
        /// </summary>
        /// <param name="tasks">多任务</param>
        public static void InParallel(params ThreadStart[] tasks) {
            IListExtensions.InParallel(new List<ThreadStart>(tasks));
        }
        /// <summary>
        /// 多个线程并行执行
        /// </summary>
        /// <param name="maxThreads">最大线程数</param>
        /// <param name="tasks">多任务</param>
        public static void InParallel(int maxThreads, params ThreadStart[] tasks) {
            IListExtensions.InParallel(new List<ThreadStart>(tasks), maxThreads);
        }
    }
    #endregion
    #region ThreadPoolEx
    /// <summary>
    /// 线程池
    /// </summary>
    public class ThreadPoolEx {
        private readonly object _lockObject = new object();
        private int _nextTask;
        private List<ThreadStart> _tasks;
        /// <summary>
        /// 多线程调用 new ThreadPool().Execute(maxThreads, tasks);
        /// </summary>
        /// <param name="maxThreads"></param>
        /// <param name="tasks"></param>
        public void Execute(int maxThreads, List<ThreadStart> tasks) {
            if ((tasks.IsNull()) || (tasks.Count == 0)) return;
            _tasks = tasks;
            if (tasks.Count < maxThreads) maxThreads = tasks.Count;
            ManualResetEvent[] resetEvents = new ManualResetEvent[maxThreads];
            for (int i = 0; i < maxThreads; i++) {
                resetEvents[i] = new ManualResetEvent(false);
                new Thread(WorkerThreadProc).Start(resetEvents[i]);
            }
            WaitHandle.WaitAll(resetEvents);
        }
        private void WorkerThreadProc(object threadParameter) {
            ManualResetEvent resetEvent = (ManualResetEvent)threadParameter;

            while (true) {
                ThreadStart task;
                lock (_lockObject) {
                    if (_nextTask >= _tasks.Count) break;
                    task = _tasks[_nextTask];
                    _nextTask++;
                }
                task();
            }

            resetEvent.Set();
        }
    }
    #endregion
}
