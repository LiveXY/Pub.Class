//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
using System.Linq.Expressions;
using System.Web.Script.Serialization;
#endif
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.ObjectModel;

namespace Pub.Class {
    /// <summary>
    /// IEnumerable 扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class IEnumerableExtensions {
        /// <summary>
        /// IsNullEmpty
        /// </summary>
        /// <param name="source">IEnumerable扩展</param>
        /// <returns>true/false</returns>
        public static bool IsNullEmpty(this IEnumerable source) {
            if (source.IsNull()) return true;
            foreach (var item in source) return false;
            return true;
        }
#if !MONO40
        /// <summary>
        /// 转DataTable
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="varlist">IEnumerable扩展</param>
        /// <returns>DataTable</returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> varlist) {
            DataTable dtReturn = new DataTable();
            PropertyInfo[] oProps = null;
            if (varlist.IsNull()) return dtReturn;

            foreach (T rec in varlist) {
                if (oProps.IsNull()) {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps) {
                        Type colType = pi.PropertyType;
                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>))) colType = colType.GetGenericArguments()[0];
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                DataRow dr = dtReturn.NewRow();
                foreach (PropertyInfo pi in oProps) dr[pi.Name] = pi.GetValue(rec, null).IsNull() ? DBNull.Value : pi.GetValue(rec, null);
                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }
        /// <summary>
        /// 转DataTable
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="enumerable">IEnumerable扩展</param>
        /// <returns>DataTable</returns>
        public static DataTable ToDataTable2<T>(this IEnumerable<T> enumerable) {
            DataTable table = new DataTable();

            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties();

            #region If T Is String Or Has No Properties

            if (properties.IsNull() || properties.Count() == 0 || typeof(T) == typeof(string)) {
                table.Columns.Add(new DataColumn("Value", typeof(string)));

                foreach (T item in enumerable) {
                    DataRow row = table.NewRow();

                    row["Value"] = item.ToString();

                    table.Rows.Add(row);
                }

                return table;
            }

            #endregion

            #region Else Normal Collection

            foreach (PropertyInfo property in properties) {
                table.Columns.Add(new DataColumn(property.Name, property.PropertyType));
            }

            foreach (T item in enumerable) {
                DataRow row = table.NewRow();

                foreach (PropertyInfo property in properties) {
                    row[property.Name] = property.GetValue(item, null);
                }

                table.Rows.Add(row);
            }

            #endregion

            return table;
        }
#endif
        /// <summary>
        /// 分页
        /// </summary>
        /// <example>
        /// <code>
        /// IList&lt;UC_Member> list = LMS_TaskFactory.Instance().SelectTaskUserDetail(taskid).ToPage&lt;UC_Member>(1, 10, out totals);
        /// </code>
        /// </example>
        /// <typeparam name="TSource">实体</typeparam>
        /// <param name="varlist">IEnumerable扩展</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页多少条记录</param>
        /// <param name="totalRecords">总记录数</param>
        /// <returns>实体列表</returns>
        public static List<TSource> ToPage<TSource>(this IEnumerable<TSource> varlist, int pageIndex, int pageSize, out int totalRecords) {
            totalRecords = varlist.Count();
            return varlist.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        /// <summary>
        /// ToJson 
        /// </summary>
        /// <example>
        /// <code>
        /// Msg.Write(LMS_TaskFactory.Instance().SelectTaskUserDetail(taskid).ToJson(p=> new { id = p.MemberID, name = p.RealName, account = p.Account }))
        /// </code>
        /// </example>
        /// <typeparam name="TSource">实体</typeparam>
        /// <typeparam name="TResult">目标类型</typeparam>
        /// <param name="varlist">IEnumerable扩展</param>
        /// <param name="selector">选择器</param>
        /// <returns>ToJson字符串</returns>
        public static string ToJson<TSource, TResult>(this IEnumerable<TSource> varlist, Func<TSource, TResult> selector) {
            return varlist.Select(selector).ToJson();
        }
        /// <summary>
        /// LIST 类型转换 性能低
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <returns>LIST</returns>
        public static IEnumerable<TTarget> ConvertTo<TSource, TTarget>(this IEnumerable<TSource> source) {
            foreach (var value in source) {
                yield return value.ConvertTo<TTarget>();
            }
        }
        /// <summary>
        /// 遍历LIST
        /// </summary>
        /// <example>
        /// <code>
        /// list3 = list3.ForEach(p => {
        ///     switch (p.TaskType) {
        ///         case 1: 
        ///             var l = cclist.Where(c => c.TaskDetailID == p.TaskDetailID).ToList(); 
        ///             p.ResourceName = l.Count==0 ? "" : l.FirstOrDefault().ResourceName; break;
        ///         case 2: 
        ///             l = ctlist.Where(c => c.TaskDetailID == p.TaskDetailID).ToList(); 
        ///             p.ResourceName = l.Count==0 ? "" : l.FirstOrDefault().ResourceName; break;
        ///         case 3: 
        ///              l = tclist.Where(c => c.TaskDetailID == p.TaskDetailID).ToList(); 
        ///              p.ResourceName = l.Count==0 ? "" : l.FirstOrDefault().ResourceName; break;
        ///         case 4: 
        ///             l = talist.Where(c => c.TaskDetailID == p.TaskDetailID).ToList(); 
        ///             p.ResourceName = l.Count==0 ? "" : l.FirstOrDefault().ResourceName; break;
        ///         case 5: p.ResourceName = list5.Where(c => c.TaskDetailID == p.TaskDetailID).ToList().FirstOrDefault().ResourceName; break;
        ///         case 6: p.ResourceName = list6.Where(c => c.TaskDetailID == p.TaskDetailID).ToList().FirstOrDefault().ResourceName; break;
        ///         case 7: p.ResourceName = list7.Where(c => c.TaskDetailID == p.TaskDetailID).ToList().FirstOrDefault().ResourceName; break;
        ///         case 8: p.ResourceName = list8.Where(c => c.TaskDetailID == p.TaskDetailID).ToList().FirstOrDefault().ResourceName; break;
        ///     }
        /// }).ToList();
        /// </code>
        /// </example>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <param name="action">动作</param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            foreach (T element in source) {
                action(element);
                yield return element;
            }
        }
        /// <summary>
        /// 遍历LIST
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <param name="action">动作</param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T, int> action) {
            int i = 0;
            foreach (T element in source) {
                action(element, i);
                i++;
                yield return element;
            }
        }
        /// <summary>
        /// 遍历LIST
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="items">IEnumerable扩展</param>
        /// <param name="odd">奇数动作</param>
        /// <param name="even">偶数动作</param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> odd, Action<T> even) {
            bool isOdd = true;
            return ForEach(items, item => {
                if (isOdd) odd(item); else even(item);
                isOdd = !isOdd;
            });
        }
        ///// <summary>
        ///// 合并
        ///// </summary>
        ///// <typeparam name="T">源类型</typeparam>
        ///// <param name="target">IEnumerable扩展</param>
        ///// <param name="data">目标数据</param>
        ///// <returns></returns>
        //public static IEnumerable<T> MergeWith<T>(this IEnumerable<T> target, params IEnumerable<T>[] data) {
        //    List<T> list = new List<T>(target);
        //    data.ForEach(array => list.AddRange(array));
        //    return list;
        //}
        ///// <summary>
        ///// 和...一起
        ///// </summary>
        ///// <typeparam name="T">源类型</typeparam>
        ///// <param name="source">IEnumerable扩展</param>
        ///// <param name="item">项</param>
        ///// <returns></returns>
        //public static IEnumerable<T> With<T>(this IEnumerable<T> source, T item) {
        //    foreach (T t in source) yield return t;
        //    yield return item;
        //}
        ///// <summary>
        ///// 包含最少的
        ///// </summary>
        ///// <typeparam name="T">源类型</typeparam>
        ///// <param name="enumeration">IEnumerable扩展</param>
        ///// <param name="count">记录数</param>
        ///// <returns></returns>
        //public static bool ContainsAtLeast<T>(this IEnumerable<T> enumeration, int count) {
        //    if (enumeration.IsNull()) throw new ArgumentNullException("enumeration不能为null");
        //    return (from t in enumeration.Take(count) select t).Count() == count;
        //}
        /// <summary>
        /// 分组
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="enumeration">IEnumerable扩展</param>
        /// <param name="count">记录数</param>
        /// <returns></returns>
        public static IEnumerable<T[]> GroupEvery<T>(this IEnumerable<T> enumeration, int count) {
            if (enumeration.IsNull()) throw new ArgumentNullException("enumeration不能为null");
            if (count <= 0) throw new ArgumentOutOfRangeException("count不能小于0");
            int current = 0;
            T[] array = new T[count];
            foreach (var item in enumeration) {
                array[current++] = item;
                if (current == count) {
                    yield return array;
                    current = 0;
                    array = new T[count];
                }
            }
            if (current != 0) yield return array;
        }
        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="enumeration">IEnumerable扩展</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> enumeration, T value) where T : IEquatable<T> {
            return enumeration.IndexOf<T>(value, EqualityComparer<T>.Default);
        }
        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="enumeration">IEnumerable扩展</param>
        /// <param name="value">值</param>
        /// <param name="comparer">比较</param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> enumeration, T value, IEqualityComparer<T> comparer) {
            int index = 0;
            foreach (var item in enumeration) {
                if (comparer.Equals(item, value)) return index;
                index++;
            }
            return -1;
        }
        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="enumeration">IEnumerable扩展</param>
        /// <param name="value">值</param>
        /// <param name="startIndex">开始位置</param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> enumeration, T value, int startIndex) where T : IEquatable<T> {
            return enumeration.IndexOf<T>(value, startIndex, EqualityComparer<T>.Default);
        }
        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="enumeration">IEnumerable扩展</param>
        /// <param name="value">值</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="comparer">比较</param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> enumeration, T value, int startIndex, IEqualityComparer<T> comparer) {
            for (int i = startIndex; i < enumeration.Count(); i++) {
                T item = enumeration.ElementAt(i);
                if (comparer.Equals(item, value)) return i;
            }
            return -1;
        }
        /// <summary>
        /// 上一个存在
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="items">IEnumerable扩展</param>
        /// <param name="value">值</param>
        /// <param name="fromIndex">位置</param>
        /// <returns></returns>
        public static int IndexOfPrevious<T>(this IEnumerable<T> items, T value, int fromIndex) {
            for (int i = fromIndex - 1; i > -1; i--) {
                T item = items.ElementAt(i);
                if (EqualityComparer<T>.Default.Equals(item, value)) return i;
            }
            return -1;
        }
        ///// <summary>
        ///// 包含
        ///// </summary>
        ///// <typeparam name="T">源类型</typeparam>
        ///// <param name="items">IEnumerable扩展</param>
        ///// <param name="value">值</param>
        ///// <returns></returns>
        //public static bool Contains2<T>(this IEnumerable<T> items, T value) {
        //    if (items.IsNull()) throw new ArgumentNullException("items");

        //    ICollection<T> c = items as ICollection<T>;
        //    if (c.IsNotNull()) return c.Contains(value);

        //    throw new NotImplementedException();
        //}
        ///// <summary>
        ///// 构造器 速度慢 反射
        ///// </summary>
        ///// <typeparam name="T">源类型</typeparam>
        ///// <param name="Parameters">IEnumerable扩展</param>
        ///// <param name="Properties"></param>
        ///// <returns></returns>
        //public static T Constructor<T>(this IEnumerable<object> Parameters, IDictionary<string, object> Properties) where T : class {
        //    Type ttype = typeof(T);
        //    T obj = (T)Activator.CreateInstance(typeof(T), Parameters);
        //    foreach (string key in Properties.Keys) {
        //        PropertyInfo prop = ttype.GetProperty(key);
        //        if (prop.IsNotNull()) prop.SetValue(obj, Properties[key], null);
        //    }
        //    return obj;
        //}
        /// <summary>
        /// Flatten
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="inputs">IEnumerable扩展</param>
        /// <param name="enumerate">Func委托函数</param>
        /// <returns></returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> inputs, Func<T, IEnumerable<T>> enumerate) {
            if (inputs.IsNotNull()) {
                var stack = new Stack<T>(inputs);
                while (stack.Count > 0) {
                    var current = stack.Pop();
                    if (current.IsNull()) continue;
                    yield return current;
                    var enumerable = enumerate.IsNotNull() ? enumerate(current) : null;
                    if (enumerable.IsNotNull()) {
                        foreach (var child in enumerable) stack.Push(child);
                    }
                }
            }
        }
        /// <summary>
        /// Flatten
        /// </summary>
        /// <param name="inputs">IEnumerable扩展</param>
        /// <param name="enumerate">Func委托函数</param>
        /// <returns></returns>
        public static IEnumerable Flatten(this IEnumerable inputs, Func<object, System.Collections.IEnumerable> enumerate) {
            return Flatten<object>(inputs.Cast<object>(), o => (enumerate(o) ?? new object[0]).Cast<object>());
        }
        //public static bool In<T>(this T source, IEnumerable<T> values) where T : IEquatable<T> {
        //    if (values.IsNull()) return false;

        //    foreach (T listValue in values) {
        //        if ((default(T).Equals(source) && default(T).Equals(listValue)) || (!default(T).Equals(source) && source.Equals(listValue))) return true;
        //    }

        //    return false;
        //}
        /// <summary>
        /// 遍历
        /// </summary>
        /// <example>
        /// <code>
        /// list3.Do&lt;UC_Member>((s)=>{ Msg.Write(s.MemberID); });
        /// </code>
        /// </example>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <param name="action">动作</param>
        /// <returns></returns>
        public static void Do<TSource>(this IEnumerable<TSource> source, Action<TSource> action) {
            foreach (var x in source) action(x);
        }
        /// <summary>
        /// 遍历 (s)
        /// </summary>
        /// <example>
        /// <code>
        /// list3.Do((s)=>{ Msg.Write(s); });
        /// </code>
        /// </example>
        /// <param name="source">IEnumerable扩展</param>
        /// <param name="action">动作</param>
        /// <returns></returns>
        public static void Do(this IEnumerable source, Action<object> action) {
            foreach (var x in source) action(x);
        }
        /// <summary>
        /// 遍历 (s, i)
        /// </summary>
        /// <example>
        /// <code>
        /// list3.Do&lt;UC_Member>((s, i)=>{ Msg.Write(s.MemberID + "|" + i); });
        /// </code>
        /// </example>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <param name="action">动作</param>
        /// <returns></returns>
        public static void Do<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action) {
            var i = 0;
            foreach (var x in source) action(x, i++);
        }
        /// <summary>
        /// 遍历 (s,i)
        /// </summary>
        /// <param name="source">IEnumerable扩展</param>
        /// <param name="action">动作</param>
        /// <returns></returns>
        public static void Do(this IEnumerable source, Action<object, int> action) {
            var i = 0;
            foreach (var x in source) action(x, i++);
        }
        /// <summary>
        /// 左边两个LIST连接
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <param name="left">IEnumerable扩展</param>
        /// <param name="right">IEnumerable</param>
        /// <returns></returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerator<TSource> left, IEnumerator<TSource> right) {
            while (left.MoveNext()) yield return left.Current;
            while (right.MoveNext()) yield return right.Current;
        }
        /// <summary>
        /// 右边连接
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <param name="left">IEnumerable扩展</param>
        /// <param name="last">IEnumerable</param>
        /// <returns></returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerator<TSource> left, TSource last) {
            while (left.MoveNext()) yield return left.Current;
            yield return last;
        }
        /// <summary>
        /// 分割
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size) {
            int index = 1;
            IEnumerable<T> partition = source.Take(size).AsEnumerable();
            while (partition.Any()) {
                yield return partition;
                partition = source.Skip(index++ * size).Take(size).AsEnumerable();
            }
        }
        /// <summary>
        /// 分隔 start
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <param name="start">开始位置</param>
        /// <returns></returns>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int? start) { return Slice<T>(source, start, null, null); }
        /// <summary>
        /// 分隔 start/end
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <param name="start">开始</param>
        /// <param name="stop">结束</param>
        /// <returns></returns>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int? start, int? stop) { return Slice<T>(source, start, stop, null); }
        /// <summary>
        /// 分隔 start/end/step
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <param name="start">开始</param>
        /// <param name="stop">结束</param>
        /// <param name="step">步长</param>
        /// <returns></returns>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int? start, int? stop, int? step) {
            if (source.IsNull()) throw new ArgumentNullException("source");
            if (step == 0) throw new ArgumentException("step不能为0", "step");

            IList<T> sourceCollection = source as IList<T>;
            if (sourceCollection.IsNull()) sourceCollection = new List<T>(source);

            if (sourceCollection.Count == 0) yield break;

            int stepCount = step ?? 1;
            int startIndex = start ?? ((stepCount > 0) ? 0 : sourceCollection.Count - 1);
            int stopIndex = stop ?? ((stepCount > 0) ? sourceCollection.Count : -1);

            if (start < 0) startIndex = sourceCollection.Count + startIndex;
            if (stop < 0) stopIndex = sourceCollection.Count + stopIndex;

            startIndex = Math.Max(startIndex, (stepCount > 0) ? 0 : int.MinValue);
            startIndex = Math.Min(startIndex, (stepCount > 0) ? sourceCollection.Count : sourceCollection.Count - 1);
            stopIndex = Math.Max(stopIndex, -1);
            stopIndex = Math.Min(stopIndex, sourceCollection.Count);

            for (int i = startIndex; (stepCount > 0) ? i < stopIndex : i > stopIndex; i += stepCount) yield return sourceCollection[i];

            yield break;
        }
        /// <summary>
        /// 复制TOLIST
        /// </summary>
        /// <typeparam name="TObject">源类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <returns></returns>
        public static IList<TObject> CopyAllToList<TObject>(this IEnumerable<TObject> source) {
            IList<TObject> copy = new List<TObject>();
            copy.ForEach(t => { copy.Add(t); });
            return copy;
        }
        /// <summary>
        /// 转换符合条件的第一个元素
        /// </summary>
        /// <typeparam name="TObject">源类型</typeparam>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <param name="converter">Func委托函数</param>
        /// <param name="predicate">字段</param>
        /// <returns></returns>
        public static TResult ConvertFirstSpecification<TObject, TResult>(this IEnumerable<TObject> source, Func<TObject, TResult> converter, Predicate<TObject> predicate) {
            TObject target = source.FindFirstSpecification<TObject>(predicate);
            if (target.IsNull()) return default(TResult);
            return converter(target);
        }
        /// <summary>
        /// FindFirstSpecification 返回符合条件的第一个元素
        /// </summary>
        /// <typeparam name="TObject">源类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <param name="predicate">字段</param>
        /// <returns></returns>
        public static TObject FindFirstSpecification<TObject>(this IEnumerable<TObject> source, Predicate<TObject> predicate) {
            foreach (TObject element in source) {
                if (predicate(element)) return element;
            }
            return default(TObject);
        }
        /// <summary>
        /// 只读ReadOnlyCollection
        /// </summary>
        /// <typeparam name="TDestination">源类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <returns></returns>
        public static ReadOnlyCollection<TDestination> ToReadOnlyCollection<TDestination>(this IEnumerable source) {
            List<TDestination> sourceAsDestination = new List<TDestination>();
            if (source.IsNotNull()) {
                foreach (TDestination toAdd in source) sourceAsDestination.Add(toAdd);
            }
            return new ReadOnlyCollection<TDestination>(sourceAsDestination);
        }
        /// <summary>
        /// 只读ReadOnlyCollection
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="this">IEnumerable扩展</param>
        /// <returns></returns>
        public static ReadOnlyCollection<T> AsReadOnly<T>(this IEnumerable<T> @this) {
            return new ReadOnlyCollection<T>(new List<T>(@this));
        }
        /// <summary>
        /// 转HashSet
        /// </summary>
        /// <typeparam name="TDestination">源类型</typeparam>
        /// <param name="source">IEnumerable扩展</param>
        /// <returns></returns>
        public static HashSet<TDestination> ToHashSet<TDestination>(this IEnumerable<TDestination> source) {
            return new HashSet<TDestination>(source);
        }
        /// <summary>
        /// ToStack
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="enumerable">IEnumerable扩展</param>
        /// <returns></returns>
        public static Stack<T> ToStack<T>(this IEnumerable<T> enumerable) {
            Stack<T> stack = new Stack<T>();
            foreach (T item in enumerable.Reverse()) stack.Push(item);
            return stack;
        }
        /// <summary>
        /// ToQueue
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="enumerable">IEnumerable扩展</param>
        /// <returns></returns>
        public static Queue<T> ToQueue<T>(this IEnumerable<T> enumerable) {
            Queue<T> queue = new Queue<T>();
            foreach (T item in enumerable) queue.Enqueue(item);
            return queue;
        }
        /// <summary>
        /// 转CSV结构
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="collection">IEnumerable扩展</param>
        /// <param name="delim">分隔符</param>
        /// <returns></returns>
        public static string ToCSV<T>(this IEnumerable<T> collection, string delim) {
            if (collection.IsNull()) return string.Empty;

            StringBuilder result = new StringBuilder();
            foreach (T value in collection) {
                result.Append(value);
                result.Append(delim);
            }
            if (result.Length > 0) result.Length -= delim.Length;
            return result.ToString();
        }
        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="coll">IEnumerable扩展</param>
        /// <param name="predicate">字段</param>
        /// <returns></returns>
        public static bool Exist<T>(this IEnumerable<T> coll, Predicate<T> predicate) {
            var it = coll.GetEnumerator();
            while (it.MoveNext())
                if (predicate(it.Current)) return true;

            return false;
        }
        /// <summary>
        /// 转换
        /// </summary>
        /// <typeparam name="TInput">源类型</typeparam>
        /// <typeparam name="TOutput">返回类型</typeparam>
        /// <param name="coll">IEnumerable扩展</param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static IEnumerable<TOutput> ConvertAll<TInput, TOutput>(this IEnumerable<TInput> coll, Converter<TInput, TOutput> converter) {
            return from input in coll select converter(input);
        }
        /// <summary>
        /// 连接成字符串
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="target">IEnumerable扩展</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> target, string separator) {
            StringBuilder sb = new StringBuilder();
            foreach (T info in target) sb.AppendFormat("{0}{1}", info.ToString(), separator);
            return sb.ToString().Left(sb.Length - separator.Length);
        }
        /// <summary>
        /// 连接成字符串
        /// </summary>
        /// <param name="target">IEnumerable扩展</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string Join(this IEnumerable target, string separator) {
            StringBuilder sb = new StringBuilder();
            foreach (object info in target) sb.AppendFormat("{0}{1}", info.ToString(), separator);
            return sb.ToString().Left(sb.Length - separator.Length);
        }
        /// <summary>
        /// 去重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector) {
            return source.GroupBy(keySelector).Select(g => g.FirstOrDefault());
        }
        /// <summary>
        /// 去重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector, IEqualityComparer<V> comparer) {
            return source.GroupBy(keySelector, comparer).Select(g => g.FirstOrDefault());
        }
#if !NET20
        /// <summary>
        /// 去重复 加上Linq to XXX的支持
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IQueryable<T> Distinct<T, V>(this IQueryable<T> source, Expression<Func<T, V>> keySelector) {
            return source.GroupBy(keySelector).Select(g => g.FirstOrDefault()).AsQueryable<T>();
        }
#endif
        /// <summary>
        /// 随机打乱顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> Rand<T>(this IEnumerable<T> list) { return list.OrderBy(p => Guid.NewGuid()); }
        /// <summary>
        /// 添加项
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">IList列表</param>
        /// <param name="item">值</param>
        /// <returns>IList列表</returns>
        public static IEnumerable<T> Add<T>(this IEnumerable<T> list, T item) {
            list.Add(item);
            return list;
        }
        /// <summary>
        /// 添加唯一项
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">IList列表</param>
        /// <param name="item">值</param>
        /// <returns>IList列表</returns>
        public static IEnumerable<T> AddUnique<T>(this IEnumerable<T> list, T item) {
            lock (((ICollection)list).SyncRoot) { if (!list.Contains(item)) list.Add(item); }
            return list;
        }

    }
}
