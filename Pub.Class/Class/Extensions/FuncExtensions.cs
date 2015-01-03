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
using System.Threading;

namespace Pub.Class {
    /// <summary>
    /// 函数扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class FuncExtensions {
        ///// <summary>
        ///// 一个参数的咖喱方法
        ///// </summary>
        ///// <example>
        ///// <code>
        ///// Func&lt;int, int> add = (x) => x + 10; 
        ///// var curriedAdd = add.Curry(); 
        ///// curriedAdd(10)
        ///// </code>
        ///// </example>
        ///// <typeparam name="T1"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="func"></param>
        ///// <returns></returns>
        //public static Func<T1, TResult> Curry<T1, TResult>(this Func<T1, TResult> func) {
        //    if (func.IsNull()) throw new ArgumentNullException("func", "func is null.");
        //    return first => func(first);
        //}
        ///// <summary>
        ///// 二个参数的咖喱方法
        ///// </summary>
        ///// <example>
        ///// <code>
        ///// Func&lt;int, int, int> add = (x, y) => x + y; 
        ///// var curriedAdd = add.Curry(); 
        ///// curriedAdd(10)(10)
        ///// </code>
        ///// </example>
        ///// <typeparam name="T1"></typeparam>
        ///// <typeparam name="T2"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="func"></param>
        ///// <returns></returns>
        //public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(this Func<T1, T2, TResult> func) {
        //    if (func.IsNull()) throw new ArgumentNullException("func", "func is null.");
        //    return first => second => func(first, second);
        //}
        ///// <summary>
        ///// 三个参数的咖喱方法
        ///// </summary>
        ///// <example>
        ///// <code>
        ///// Func&lt;int, int, int, int> add = (x, y, z) => x + y + z; 
        ///// var curriedAdd = add.Curry(); 
        ///// curriedAdd(10)(10)(10)
        ///// </code>
        ///// </example>
        ///// <typeparam name="T1"></typeparam>
        ///// <typeparam name="T2"></typeparam>
        ///// <typeparam name="T3"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="func"></param>
        ///// <returns></returns>
        //public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func) {
        //    if (func.IsNull()) throw new ArgumentNullException("func", "func is null.");
        //    return first => second => third => func(first, second, third);
        //}
        ///// <summary>
        ///// 四个参数的咖喱方法
        ///// </summary>
        ///// <example>
        ///// <code>
        ///// Func&lt;int, int, int, int, int> add = (x, y, z, i) => x + y + z + i; 
        ///// var curriedAdd = add.Curry(); 
        ///// curriedAdd(10)(10)(10)(10)
        ///// </code>
        ///// </example>
        ///// <typeparam name="T1"></typeparam>
        ///// <typeparam name="T2"></typeparam>
        ///// <typeparam name="T3"></typeparam>
        ///// <typeparam name="T4"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="func"></param>
        ///// <returns></returns>
        //public static Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> Curry<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func) {
        //    if (func.IsNull()) throw new ArgumentNullException("func", "func is null.");
        //    return first => second => third => fourth => func(first, second, third, fourth);
        //}
        ///// <summary>
        ///// 第一个参数先执行
        ///// </summary>
        ///// <example>
        ///// <code>
        ///// Func&lt;string, int, string> MsgW3 = (s, i) => { return s + i; }; 
        ///// Msg.WriteEnd(MsgW3.ApplyFirst&lt;string, int, string>("test")(9));
        ///// </code>
        ///// </example>
        ///// <typeparam name="T1"></typeparam>
        ///// <typeparam name="T2"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="func"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static Func<T2, TResult> ApplyFirst<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 value) {
        //    if (func.IsNull()) throw new ArgumentNullException("func", "func is null.");
        //    if (value.IsNull()) throw new ArgumentNullException("value", "value is null.");
        //    return second => func(value, second);
        //}
        ///// <summary>
        ///// 第二个参数先执行
        ///// </summary>
        ///// <example>
        ///// <code>
        ///// Func&lt;string, int, string> MsgW3 = (s, i) => { return s + i; }; 
        ///// Msg.WriteEnd(MsgW3.ApplySecond&lt;string, int, string>(9)("test"));
        ///// </code>
        ///// </example>
        ///// <typeparam name="T1"></typeparam>
        ///// <typeparam name="T2"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="func"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static Func<T1, TResult> ApplySecond<T1, T2, TResult>(this Func<T1, T2, TResult> func, T2 value) {
        //    if (func.IsNull()) throw new ArgumentNullException("func", "func is null.");
        //    if (value.IsNull()) throw new ArgumentNullException("value", "value is null.");
        //    return first => func(first, value);
        //}
        ///// <summary>
        ///// 第一个参数先执行
        ///// </summary>
        ///// <typeparam name="T1"></typeparam>
        ///// <typeparam name="T2"></typeparam>
        ///// <typeparam name="T3"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="func"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static Func<T2, T3, TResult> ApplyFirst<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 value) {
        //    if (func.IsNull()) throw new ArgumentNullException("func", "func is null.");
        //    if (value.IsNull()) throw new ArgumentNullException("value", "value is null.");
        //    return (second, third) => func(value, second, third);
        //}
        ///// <summary>
        ///// 第二个参数先执行
        ///// </summary>
        ///// <typeparam name="T1"></typeparam>
        ///// <typeparam name="T2"></typeparam>
        ///// <typeparam name="T3"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="func"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static Func<T1, T3, TResult> ApplySecond<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T2 value) {
        //    if (func.IsNull()) throw new ArgumentNullException("func", "func is null.");
        //    if (value.IsNull()) throw new ArgumentNullException("value", "value is null.");
        //    return (first, third) => func(first, value, third);
        //}
        ///// <summary>
        ///// 第三个参数先执行
        ///// </summary>
        ///// <typeparam name="T1"></typeparam>
        ///// <typeparam name="T2"></typeparam>
        ///// <typeparam name="T3"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="func"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public static Func<T1, T2, TResult> ApplyThird<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T3 value) {
        //    if (func.IsNull()) throw new ArgumentNullException("func", "func is null.");
        //    if (value.IsNull()) throw new ArgumentNullException("value", "value is null.");
        //    return (first, second) => func(first, second, value);
        //}
        ///// <summary>
        ///// Compose
        ///// </summary>
        ///// <typeparam name="T1"></typeparam>
        ///// <typeparam name="T2"></typeparam>
        ///// <typeparam name="T3"></typeparam>
        ///// <param name="f"></param>
        ///// <param name="g"></param>
        ///// <returns></returns>
        //public static Func<T1, T3> Compose<T1, T2, T3>(this Func<T2, T3> f, Func<T1, T2> g) { return x => f(g(x)); }
        ///// <summary>
        ///// 执行递归 记住函数执行结果
        ///// </summary>
        ///// <example>
        ///// <code>
        ///// Func&lt;int, int> fib = (int n) => { int f = n > 1 ? fib(n - 1) + fib(n - 2) : 1; return f; };
        ///// fib = fib.Memoize();
        ///// </code>
        ///// </example>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="func"></param>
        ///// <returns></returns>
        //public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func) {
        //    if (func.IsNull()) throw new ArgumentNullException("func", "func is null.");

        //    var table = new Dictionary<T, TResult>();
        //    return argument => {
        //        if (table.ContainsKey(argument)) {
        //            return table[argument];
        //        } else {
        //            var result = func(argument);
        //            table.Add(argument, result);
        //            return result;
        //        }
        //    };
        //}
        /// <summary>
        /// 执行递归
        /// </summary>
        /// <example>
        /// <code>
        /// var fac = Fix&lt;int, int>(f => x => x &lt;= 1 ? 1 : x * f(x - 1)); // 5 * 4 * 3 * 2 * 1
        /// var fib = Fix&lt;int, int>(f => x => x &lt;= 1 ? 1 : f(x - 1) + f(x - 2)); //菲波纳契数列
        /// var fib1 = FuncExtensions.Fix&lt;int, int>(f => x => x &lt; 2 ? x : f(x - 1) + f(x - 2)); //菲波纳契数列
        ///     public IEnumerable GetCourseSeriesTree(int parentID) {
        ///         IList&lt;CC_CourseSeries> list = CC_CourseSeriesFactory.Instance().SelectCourseSeriesListByCustomerID((int)CurrentUser.CustomerID, true);
        ///         return FuncExtensions.Fix&lt;int, IEnumerable>(f => p => { 
        ///             var list2 = list.Where(o => o.ParentID == p).Select(o => new {
        ///                 id = o.SeriesID,
        ///                 name = o.SeriesName.UnHtml(),
        ///                 node = f((int)o.SeriesID)
        ///             });
        ///             return list2;
        ///         })(parentID);
        ///     }
        /// </code>
        /// </example>
        /// <typeparam name="T">T类型</typeparam>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="f">Func委托函数</param>
        /// <returns>Func委托函数</returns>
        public static Func<T, TResult> Fix<T, TResult>(this Func<Func<T, TResult>, Func<T, TResult>> f) {
            return x => f(Fix(f))(x);
        }
        /// <summary>
        /// 执行递归
        /// </summary>
        /// <example>
        /// <code>
        /// var gcd = Fix&lt;int, int, int>(f => (x, y) => y == 0 ? x : f(y, x % y)); //“辗转相除法”计算最大公约数
        /// </code>
        /// </example>
        /// <typeparam name="T1">T1类型</typeparam>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="f">Func委托函数</param>
        /// <returns>Func委托函数</returns>
        public static Func<T1, T2, TResult> Fix<T1, T2, TResult>(this Func<Func<T1, T2, TResult>, Func<T1, T2, TResult>> f) {
            return (x, y) => f(Fix(f))(x, y);
        }
        /// <summary>
        /// 执行尾递归
        /// </summary>
        /// <typeparam name="T1">T1类型</typeparam>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="f">Func委托函数</param>
        /// <returns>Func委托函数</returns>
        public static Func<T1, TResult> TailFix<T1, TResult>(this Func<Func<T1, TResult>, T1, TResult> f) {
            return (p1) => {
                bool callback = false;
                Func<T1, TResult> self = (q1) => {
                    p1 = q1;
                    callback = true;
                    return default(TResult);
                };
                do {
                    var result = f(self, p1);
                    if (!callback) {
                        return result;
                    }
                    callback = false;
                } while (true);
            };
        }
        /// <summary>
        /// 执行尾递归
        /// </summary>
        /// <example>
        /// <code>
        /// var fac1 = FuncExtensions.TailFix&lt;long, long, long>((f, n, a) => n == 1 ? a : f(n - 1, a * n));
        /// Func&lt;long, long> fac2 = n => n == 0 ? 1 : fac1(n, 1);
        /// Msg.Write(fac1(50, 1));
        /// Msg.Write(fac2(50));
        /// var fib2 = FuncExtensions.TailFix&lt;long, long, long, long>((f, n, a1, a2) => n == 0 ? a1 : f(n - 1, a2, a1 + a2));
        /// Msg.Write(fib2(5, 0, 1));
        /// </code>
        /// </example>
        /// <typeparam name="T1">T1类型</typeparam>
        /// <typeparam name="T2">T2类型</typeparam>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="f">Func委托函数</param>
        /// <returns>Func委托函数</returns>
        public static Func<T1, T2, TResult> TailFix<T1, T2, TResult>(this Func<Func<T1, T2, TResult>, T1, T2, TResult> f) {
            return (p1, p2) => {
                bool callback = false;
                Func<T1, T2, TResult> self = (q1, q2) => {
                    p1 = q1;
                    p2 = q2;
                    callback = true;
                    return default(TResult);
                };
                do {
                    var result = f(self, p1, p2);
                    if (!callback) {
                        return result;
                    }
                    callback = false;
                } while (true);
            };
        }
        /// <summary>
        /// 执行尾递归
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Func<T1, T2, T3, TResult> TailFix<T1, T2, T3, TResult>(this Func<Func<T1, T2, T3, TResult>, T1, T2, T3, TResult> f) {
            return (p1, p2, p3) => {
                bool callback = false;
                Func<T1, T2, T3, TResult> self = (q1, q2, q3) => {
                    p1 = q1;
                    p2 = q2;
                    p3 = q3;
                    callback = true;
                    return default(TResult);
                };
                do {
                    var result = f(self, p1, p2, p3);
                    if (!callback) {
                        return result;
                    }
                    callback = false;
                } while (true);
            };
        }
        /// <summary>
        /// 遍历
        /// </summary>
        /// <typeparam name="T">T类型</typeparam>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="f">Func委托函数</param>
        /// <param name="source">源</param>
        /// <returns></returns>
        public static IEnumerable<TResult> Map<T, TResult>(this Func<T, TResult> f, IEnumerable<T> source) { return Enumerable.Select(source, f); }
        /// <summary>
        /// 重试方法
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="action">方法</param>
        /// <param name="numRetries">重试次数</param>
        /// <param name="retryTimeout">延时多长时间后重试，单位毫秒</param>
        /// <param name="throwIfFail">经过几轮重试操作后依然发生异常时是否将异常抛出</param>
        /// <param name="onFailureAction">操作失败执行的方法</param>
        /// <returns></returns>
        public static T Retry<T>(this Func<T> action, int numRetries, int retryTimeout, bool throwIfFail, Action onFailureAction) {
            if (action.IsNull()) throw new ArgumentNullException("action");
            T retVal = default(T);
            do {
                try {
                    retVal = action();
                    return retVal;
                } catch {
                    if (onFailureAction.IsNotNull()) onFailureAction();
                    if (numRetries <= 0 && throwIfFail) throw;
                    if (retryTimeout > 0) Thread.Sleep(retryTimeout);
                }
            } while (numRetries-- > 0);
            return retVal;
        }
    }
}
