//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
using System.Xml.Linq;
using System.Web.Script.Serialization;
#endif
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Xml;
using System.Drawing;

namespace Pub.Class {
    /// <summary>
    /// 类方法扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class ClassExtensions {
        /// <summary>
        /// 实体类 ToJson 筛选部分字段
        /// </summary>
        /// <example>
        /// <code>
        /// Msg.WriteEnd(LMS_TaskDetailFactory.Instance().SelectByID(id).ToJson(p => new { p.TaskID, p.TaskName }));
        /// </code>
        /// </example>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="entity">源</param>
        /// <param name="selector">选择器</param>
        /// <returns>字符串</returns>
        public static string ToJson<TSource, TResult>(this TSource entity, Func<TSource, TResult> selector) {
            return selector(entity).ToJson();
            //List<TSource> oblist = new List<TSource>();
            //oblist.Add(entity);
            //string str = oblist.Select(selector).ToJson();
            //return str.Substring(1, str.Length - 2);
        }
        /// <summary>
        /// 实体类 筛选部分字段
        /// </summary>
        /// <example>
        /// <code>
        /// var t = LMS_TaskDetailFactory.Instance().SelectByID(id).Select(p => new { p.TaskID, p.TaskName }));
        /// </code>
        /// </example>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="entity">源</param>
        /// <param name="selector">选择器</param>
        /// <returns>实体类</returns>
        public static TResult SelectEntity<TSource, TResult>(this TSource entity, Func<TSource, TResult> selector) {
            return selector(entity);
            //List<TSource> oblist = new List<TSource>();
            //oblist.Add(entity);
            //return oblist.Select(selector).ToList()[0];
        }
        /// <summary>
        /// T 是否在 数组中 "333".EqualsAny("111", "222", "333")
        /// </summary>
        /// <example>
        /// <code>
        /// "333".EqualsAny("111", "222", "333")
        /// </code>
        /// </example>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="obj">参数值</param>
        /// <param name="values">params 数组</param>
        /// <returns>true/false</returns>
        public static bool In<T>(this T obj, params T[] values) {
            return (Array.IndexOf(values, obj) != -1);
        }
        /// <summary>
        /// T 是否 在minValue/maxValue 之间包含minValue/maxValue "222".IsBetween("111", "333")
        /// </summary>
        /// <example>
        /// <code>
        /// "222".IsBetween("111", "333")
        /// </code>
        /// </example>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="value">参数值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns>true/false</returns>
        public static bool IsBetween<T>(this T value, T minValue, T maxValue) where T : IComparable<T> {
            return IsBetween(value, minValue, maxValue, null);
        }
        /// <summary>
        /// T 是否 在minValue/maxValue 之间包含minValue/maxValue 
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="value">参数值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="comparer"></param>
        /// <returns>true/false</returns>
        public static bool IsBetween<T>(this T value, T minValue, T maxValue, IComparer<T> comparer) where T : IComparable<T> {
            comparer = comparer ?? Comparer<T>.Default;

            var minMaxCompare = comparer.Compare(minValue, maxValue);
            if (minMaxCompare < 0) {
                return ((comparer.Compare(value, minValue) >= 0) && (comparer.Compare(value, maxValue) <= 0));
            } else if (minMaxCompare == 0) {
                return (comparer.Compare(value, minValue) == 0);
            } else {
                return ((comparer.Compare(value, maxValue) >= 0) && (comparer.Compare(value, minValue) <= 0));
            }
        }
        /// <summary>
        /// 对像克隆
        /// </summary>
        /// <example>
        /// <code>
        /// "test".Clone&lt;string>();
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="o">值</param>
        /// <returns>新对像</returns>
        public static T Clone<T>(this T o) where T : class {
            Type type = o.GetType().BaseType;

            MethodInfo[] methodInfos = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            MethodInfo cloneMethod = null;
            foreach (var item in methodInfos) {
                if (item.Name == "MemberwiseClone") {
                    cloneMethod = item;
                    break;
                }
            }
            if (cloneMethod.IsNotNull()) return (T)cloneMethod.Invoke(o, null);
            return default(T);
        }
        /// <summary>
        /// 对像序列代成字节
        /// </summary>
        /// <example>
        /// <code>
        /// "test".ToBinary&lt;string>();
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="o">值</param>
        /// <returns>字节</returns>
        //public static byte[] ToBytes<T>(this T o) {
        //    return SerializeBytes.ToBytes<T>(o);
        //    //var formatter = new BinaryFormatter();
        //    //using (MemoryStream ms = new MemoryStream()) {
        //    //    formatter.Serialize(ms, o);
        //    //    return ms.ToArray();
        //    //}
        //}
        /// <summary>
        /// 对像序列化后压缩
        /// </summary>
        /// <typeparam name="T">对像/类型</typeparam>
        /// <param name="o">字节</param>
        /// <returns>对像</returns>
        public static byte[] ToBinaryDeflateCompress<T>(this T o) where T : class {
            return o.ToBytes<T>().DeflateCompress();
        }
        /// <summary>
        /// 管道 T传入函数执行返回R "1".Pipe&lt;string, int>((s) => { return s.ToInt() + 1; })
        /// </summary>
        /// <example>
        /// <code>
        /// "1".Pipe&lt;string, int>((s) => { return s.ToInt() + 1; })
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <typeparam name="R">返回值类型</typeparam>
        /// <param name="o">入口参数值</param>
        /// <param name="action">函数</param>
        /// <returns></returns>
        public static R Pipe<T, R>(this T o, Func<T, R> action) {
            //T buffer = o;
            return action(o);
        }
        /// <summary>
        /// 管道 T传入动作执行 "1".Pipe&lt;string>((s) => { s= s + 1; }) 结果是T
        /// </summary>
        /// <example>
        /// <code>
        /// "1".Pipe&lt;string>((s) => { s= s + 1; })
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="o">入口参数值</param>
        /// <param name="action">动作</param>
        /// <returns></returns>
        public static T Pipe<T>(this T o, Action<T> action) {
            //T buffer = o;
            action(o);
            return o;
        }
        /// <summary>
        /// 默认值
        /// </summary>
        /// <example>
        /// <code>
        /// 0.IsDefault&lt;int>();
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="value">类型值</param>
        /// <returns>true/false</returns>
        public static bool IsDefault<T>(this T value) { return Equals(value, default(T)); }
        /// <summary>
        /// 如果操作 1.If(i => i &lt; 0, 0)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">值</param>
        /// <param name="predicate">条件</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T If<T>(this T obj, Predicate<T> predicate, T defaultValue = default(T)) {
            if (obj.IsNull()) return defaultValue;
            return predicate(obj) ? obj : defaultValue;
        }
        /// <summary>
        /// 非如果操作 1.NotIf(i => i &lt; 0, 0)
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">值</param>
        /// <param name="predicate">条件</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T NotIf<T>(this T obj, Predicate<T> predicate, T defaultValue = default(T)) {
            if (obj.IsNull()) return defaultValue;
            return predicate(obj) ? defaultValue : obj;
        }
        /// <summary>
        /// 如果操作 If 1.If(i => i &lt; 0, i => { })
        /// </summary>
        /// <example>
        /// <code>
        /// If 1.If(i => i &lt; 0, i => { })
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">值</param>
        /// <param name="predicate">条件</param>
        /// <param name="action">动作</param>
        public static void If<T>(this T t, Predicate<T> predicate, Action<T> action) where T : class { if (predicate(t)) action(t); }
        /// <summary>
        /// 如果操作 If int val = 1.If(i => i &lt; 0, i => 0)
        /// </summary>
        /// <example>
        /// <code>
        /// If int val = 1.If(i => i &lt; 0, i => 0)
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">值</param>
        /// <param name="predicate">条件</param>
        /// <param name="func">函数</param>
        /// <returns></returns>
        public static T If<T>(this T t, Predicate<T> predicate, Func<T, T> func) where T : struct { return predicate(t) ? func(t) : t; }
        /// <summary>
        /// 如果操作  1.If(i => i &lt; 0, i => { }, i => { })
        /// </summary>
        /// <example>
        /// <code>
        /// 1.If(i => i &lt; 0, i => { }, i => { })
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">值</param>
        /// <param name="predicate">条件</param>
        /// <param name="action1">动作1</param>
        /// <param name="action2">动作2</param>
        public static void If<T>(this T t, Predicate<T> predicate, Action<T> action1, Action<T> action2) where T : class { if (predicate(t)) action1(t); else action2(t); }
        /// <summary>
        /// 如果操作 int val = 1.If(i => i &lt; 0, i => 0, i => i)
        /// </summary>
        /// <example>
        /// <code>
        /// int val = 1.If(i => i &lt; 0, i => 0, i => i)
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">值</param>
        /// <param name="predicate">条件</param>
        /// <param name="func1">函数1</param>
        /// <param name="func2">函数2</param>
        /// <returns></returns>
        public static T If<T>(this T t, Predicate<T> predicate, Func<T, T> func1, Func<T, T> func2) where T : struct { return predicate(t) ? func1(t) : func2(t); }
        /// <summary>
        /// While循环操作 1.While(i => i &lt; 0, i => { Msg.WriteEnd(i); })
        /// </summary>
        /// <example>
        /// <code>
        /// 1.While(i => i &lt; 0, i => { Msg.WriteEnd(i); })
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t">值</param>
        /// <param name="predicate">条件</param>
        /// <param name="action">动作</param>
        public static void While<T>(this T t, Predicate<T> predicate, Action<T> action) where T : class { while (predicate(t)) action(t); }
#if !NET20
        /// <summary>
        /// T 序列化成 XDocument
        /// </summary>
        /// <example>
        /// <code>
        /// "test".ToXDocument&lt;string>();
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">值</param>
        /// <returns>XDocument</returns>
        public static XDocument ToXDocument<T>(this T obj) {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            XDocument xdoc = new XDocument();
            using (XmlWriter w = xdoc.CreateWriter()) xs.Serialize(w, obj);
            return xdoc;
        }
#endif
        /// <summary>
        /// Color 转16进制字符串
        /// </summary>
        /// <param name="c">Color</param>
        /// <returns>字符串</returns>
        public static string ToHex(this Color c) {
            string f = String.Format("{0:X2}", Convert.ToInt32(c.R));
            f += String.Format("{0:X2}", Convert.ToInt32(c.G));
            f += String.Format("{0:X2}", Convert.ToInt32(c.B));
            return f;
        }
        /// <summary>
        /// 树查询  不包含女性及其子孙：var p1 = people.GetDescendants(p => p.IsMale ? p.Children : Enumerable.Empty&lt;People>(), p =>p.IsMale);
        /// </summary>
        /// <example>
        /// <code>
        /// public abstract class People{
        ///     public bool IsMale { get; private set; }
        ///     public abstract IEnumerable&lt;People> Children { get; }
        /// }
        /// 不包含女性及其子孙：var p1 = people.GetDescendants(p => p.IsMale ? p.Children : Enumerable.Empty&lt;People>(), p =>p.IsMale);
        /// 不包含女性及其子孙：var p1 = people.GetDescendants(p => p.IsMale ? p.Children : Enumerable.Empty&lt;People>(), p =>p.IsMale);
        /// 包含女性但不包含其子孙: var p2 = people.GetDescendants(p => p.IsMale ? p.Children : Enumerable.Empty&lt;People>(), null);
        /// 获取所有子孙: var descendants = people.GetDescendants(p => p.Children, null);
        /// 获取所有男性子孙： var males = people.GetDescendants(p => p.Children, p => p.IsMale);
        /// </code>
        /// </example>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="root">值</param>
        /// <param name="childSelector">函数</param>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public static IEnumerable<T> GetDescendants<T>(this T root, Func<T, IEnumerable<T>> childSelector, Predicate<T> filter) {
            foreach (T t in childSelector(root)) {
                if (filter.IsNull() || filter(t)) yield return t;
                foreach (T child in GetDescendants((T)t, childSelector, filter)) yield return child;
            }
        }
        /// <summary>
        /// 树查询 
        /// </summary>
        /// <example>
        /// <code>
        /// 获取TreeView中所有以“酒”结尾的树结点: var treeViewNode = treeView1.GetDescendants(treeView => treeView.Nodes.Cast&lt;TreeNode>(),treeNode => treeNode.Nodes.Cast&lt;TreeNode>(),treeNode => treeNode.Text.EndsWith("酒"));
        /// </code>
        /// </example>
        /// <typeparam name="TRoot">根类型</typeparam>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="root">根值</param>
        /// <param name="rootChildSelector">函数</param>
        /// <param name="childSelector">子选择</param>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public static IEnumerable<T> GetDescendants<TRoot, T>(this TRoot root, Func<TRoot, IEnumerable<T>> rootChildSelector, Func<T, IEnumerable<T>> childSelector, Predicate<T> filter) {
            foreach (T t in rootChildSelector(root)) {
                if (filter.IsNull() || filter(t)) yield return t;
                foreach (T child in GetDescendants(t, childSelector, filter)) yield return child;
            }
        }
        /// <summary>
        /// 存在
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="c">IEnumerable</param>
        /// <param name="t">值</param>
        /// <returns></returns>
        public static bool In<T>(this T t, IEnumerable<T> c) {
            return c.Any(i => i.Equals(t));
        }
        /// <summary>
        /// 左边连接
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <param name="first">值</param>
        /// <param name="right">IEnumerator</param>
        /// <returns></returns>
        public static IEnumerable<TSource> Concat<TSource>(this TSource first, IEnumerator<TSource> right) {
            yield return first;
            while (right.MoveNext()) yield return right.Current;
        }
        /// <summary>
        /// 如果对象为null则调用函数委托并返回函数委托的返回值。否则返回对象本身
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="func">对象为null时用于调用的函数委托</param>
        /// <returns>如果对象不为null则返回对象本身，否则返回<paramref name="func"/>函数委托的返回值</returns>
        /// <example>
        /// <code>
        /// string v = null;
        /// string d = v.IfNull&lt;string&gt;(()=>"v is null");  //d = "v is null";
        /// string t = d.IfNull(() => "d is null");              //t = "v is null";
        /// </code>
        /// </example>
        public static T IfNull<T>(this T obj, Func<T> func) where T : class {
            if (obj.IsNull() || obj.IsDBNull()) {
                return func.IsNull() ? default(T) : func();
            } else {
                return obj;
            }
        }
        /// <summary>
        /// 如果空
        /// </summary>
        /// <param name="obj">对像</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>object</returns>
        public static object IfNull(this object obj, object defaultValue) {
            return obj.IsNull() || obj.IsDBNull() ? defaultValue : obj;
        }
        /// <summary>
        /// 如果空
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对像</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>T</returns>
        public static T IfNull<T>(this T obj, T defaultValue) {
            return obj.IsNull() || obj.IsDBNull() ? defaultValue : obj;
        }
        /// <summary>
        /// 显示各种输出对象的内容
        /// </summary>
        /// <example>
        /// <code>
        /// var root = new User {
        ///     UserID = 1000,
        ///     Name = "熊华春",
        ///     Child = new User {
        ///         UserID = 1000,
        ///         Name = "熊华春1",
        ///         Child = new User {
        ///             UserID = 1000,
        ///             Name = "熊华春11",
        /// 
        ///         }
        ///     }
        /// };
        /// Msg.WriteEnd(root.Dump("root").Replace(" ","nbsp;").ReplaceRNToBR());
        /// </code>
        /// </example>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="value">对像</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        //public static string Dump<T>(this T value, string name) {
        //    using (var writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture)) {
        //        Dump(value, name, writer);
        //        return writer.ToString();
        //    }
        //}
        /// <summary>
        /// 显示各种输出对象的内容
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="value">对像</param>
        /// <param name="name">名称</param>
        /// <param name="writer">writer</param>
        /// <returns></returns>
        //public static T Dump<T>(this T value, string name, TextWriter writer) {
        //    if (name.IsNullEmpty()) throw new ArgumentNullException("name");
        //    if (writer.IsNull()) throw new ArgumentNullException("writer");
        //    Dumper.Dump(value, name, writer);
        //    return value;
        //}
        /// <summary>
        /// 显示各种输出对象的内容
        /// </summary>
        /// <typeparam name="T">对像类型</typeparam>
        /// <param name="value">对像</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        //public static T DebugDump<T>(this T value, string name) {
        //    if (name.IsNullEmpty()) throw new ArgumentNullException("name");
        //    using (var writer = new DebugWriter()) {
        //        return Dump(value, name, writer);
        //    }
        //}
        ///// <summary>
        ///// T 序列化成 XML字符串
        ///// </summary>
        ///// <example>
        ///// <code>
        ///// "test".ToXml&lt;string>();
        ///// </code>
        ///// </example>
        ///// <typeparam name="T">类型</typeparam>
        ///// <param name="obj">值</param>
        ///// <returns>XML字符串</returns>
        //public static string ToXml<T>(this T obj) {
        //    string s = null;
        //    using (MemoryStream ms = new MemoryStream(1000)) {
        //        XmlSerializer serializer = new XmlSerializer(typeof(T));
        //        XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
        //        xmlnsEmpty.Add("", "");
        //        serializer.Serialize(ms, obj, xmlnsEmpty);
        //        ms.Seek(0, SeekOrigin.Begin);
        //        using (StreamReader reader = new StreamReader(ms)) {
        //            s = reader.ReadToEnd();
        //        }
        //        ms.Close();
        //        serializer = null;
        //    }
        //    return s;
        //}
    }
}
