//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
using System.Web.Script.Serialization;
#endif
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Pub.Class {
    /// <summary>
    /// 方法扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class ActionExtensions {
#if !MONO40
        /// <summary>
        /// 无参数动作执行时间 返回详细信息
        /// </summary>
        /// <example>
        /// <code>
        /// Msg.WriteEnd(ActionExtensions.Time(() => { Msg.Write(1); }, "测试", 1000));
        /// </code>
        /// </example>
        /// <param name="action">动作</param>
        /// <param name="name">测试名称</param>
        /// <param name="iteration">执行次数</param>
        /// <returns>返回执行时间毫秒(ms)</returns>
        public static string Time(this Action action, string name = "", int iteration = 1) {
            if (name.IsNullEmpty()) {
                var watch = Stopwatch.StartNew();
                long cycleCount = WinApi.GetCycleCount();
                for (int i = 0; i < iteration; i++) action();
                long cpuCycles = WinApi.GetCycleCount() - cycleCount;
                watch.Stop();
                return watch.Elapsed.ToString();
            } else {
                StringBuilder sb = new StringBuilder();

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                int[] gcCounts = new int[GC.MaxGeneration + 1];
                for (int i = 0; i <= GC.MaxGeneration; i++) gcCounts[i] = GC.CollectionCount(i);

                var watch = Stopwatch.StartNew();
                long cycleCount = WinApi.GetCycleCount();
                for (int i = 0; i < iteration; i++) action();
                long cpuCycles = WinApi.GetCycleCount() - cycleCount;
                watch.Stop();

                sb.AppendFormat("{0} 循环{1}次测试结果：<br />", name, iteration);
                sb.AppendFormat("使用时间：{0}<br />", watch.Elapsed.ToString());
                sb.AppendFormat("CPU周期：{0}<br />", cpuCycles.ToString("N0"));

                for (int i = 0; i <= GC.MaxGeneration; i++) sb.AppendFormat("Gen　　{0}：{1}<br />", i, GC.CollectionCount(i) - gcCounts[i]);
                sb.Append("<br />");
                return sb.ToString();
            }
        }
#endif
        /// <summary>
        /// 重试方法
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="numRetries">重试次数</param>
        /// <param name="retryTimeout">延时多长时间后重试，单位毫秒</param>
        /// <param name="throwIfFail">经过几轮重试操作后依然发生异常时是否将异常抛出</param>
        /// <param name="onFailureAction">操作失败执行的方法</param>
        /// <returns></returns>
        public static void Retry(this Action action, int numRetries, int retryTimeout, bool throwIfFail, Action<Exception> onFailureAction) {
            if (action.IsNull()) throw new ArgumentNullException("action");
            numRetries--;
            do {
                bool istrue = false;
                try {
                    action();
                    istrue = true;
                } catch (Exception ex) {
                    istrue = false;
                    if (onFailureAction.IsNotNull()) onFailureAction(ex);
                    if (numRetries <= 0 && throwIfFail) throw ex;
                }
                if (retryTimeout > 0 && !istrue) Thread.Sleep(retryTimeout);
            } while (numRetries-- > 0);
        }
    }
}
