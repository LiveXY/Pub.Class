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
using System.Collections;
using System.Threading;

namespace Pub.Class {
    /// <summary>
    /// IList扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class IListExtensions {
        /// <summary>
        /// 是否空
        /// </summary>
        /// <param name="self">List扩展</param>
        /// <returns>true/false</returns>
        public static bool IsNullEmpty(this IList self) { return self.IsNull() || self.Count == 0; }
        /// <summary>
        /// 遍历
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <typeparam name="K">返回值类型</typeparam>
        /// <param name="list">IList&lt;T>扩展</param>
        /// <param name="function">Func委托函数</param>
        /// <returns>IList&lt;K></returns>
        public static IList<K> Map<T, K>(this IList<T> list, Func<T, K> function) {
            var newList = new List<K>(list.Count);
            for (var i = 0; i < list.Count; ++i) newList.Add(function(list[i]));
            return newList;
        }
        /// <summary>
        /// 将IList&lt;T>按指定行分组成IList&lt;IList&lt;T>>
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="list">List扩展</param>
        /// <param name="Count">长度</param>
        /// <returns>新列表</returns>
        public static IList<IList<T>> Split<T>(this IList<T> list, int Count) {
            var currentList = new List<T>(Count);
            var returnList = new List<IList<T>>();
            for (int i = 0, j = list.Count; i < j; i++) {
                if (currentList.Count < Count) {
                    currentList.Add(list[i]);
                } else {
                    returnList.Add(currentList);
                    currentList = new List<T>(Count);
                    currentList.Add(list[i]);
                }
            }
            returnList.Add(currentList);
            return returnList;
        }
        /// <summary>
        /// x y对调
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="obj">List扩展</param>
        /// <param name="x">x位置</param>
        /// <param name="y">y位置</param>
        public static void Swap<T>(this IList<T> obj, int x, int y) {
            if (x != y) {
                T xValue = obj[x];
                obj[x] = obj[y];
                obj[y] = xValue;
            }
        }
        /// <summary>
        /// BinarySearch 从已排序的列表中，采用二分查找找到目标在列表中的位置。
        /// 如果刚好有个元素与目标相等，则返回true，且minIndex会被赋予该元素的位置；否则，返回false，且minIndex会被赋予比目标小且最接近目标的元素的位置
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="sortedList">List扩展</param>
        /// <param name="target">值</param>
        /// <param name="minIndex">最小位置</param>
        /// <returns></returns>
        public static bool BinarySearch<T>(this IList<T> sortedList, T target, out int minIndex) where T : IComparable {
            if (target.CompareTo(sortedList[0]) == 0) {
                minIndex = 0;
                return true;
            }

            if (target.CompareTo(sortedList[0]) < 0) {
                minIndex = -1;
                return false;
            }

            if (target.CompareTo(sortedList[sortedList.Count - 1]) == 0) {
                minIndex = sortedList.Count - 1;
                return true;
            }

            if (target.CompareTo(sortedList[sortedList.Count - 1]) > 0) {
                minIndex = sortedList.Count - 1;
                return false;
            }

            //int targetPosIndex = -1;
            int left = 0;
            int right = sortedList.Count - 1;

            while (right - left > 1) {
                int middle = (left + right) / 2;

                if (target.CompareTo(sortedList[middle]) == 0) {
                    minIndex = middle;
                    return true;
                }

                if (target.CompareTo(sortedList[middle]) < 0) {
                    right = middle;
                } else {
                    left = middle;
                }
            }

            minIndex = left;
            return false;
        }
        /// <summary>
        /// GetIntersection 高效地求两个List元素的交集。
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="list1">List扩展</param>
        /// <param name="list2">list2</param>
        /// <returns></returns>
        public static List<T> GetIntersection<T>(this List<T> list1, List<T> list2) where T : IComparable {
            List<T> largList = list1.Count > list2.Count ? list1 : list2;
            List<T> smallList = largList == list1 ? list2 : list1;

            largList.Sort();
            int minIndex = 0;

            List<T> result = new List<T>();
            foreach (T tmp in smallList) {
                if (largList.BinarySearch<T>(tmp, out minIndex)) {
                    result.Add(tmp);
                }
            }

            return result;
        }
        /// <summary>
        /// GetUnion 高效地求两个List元素的并集。
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="list1">List扩展</param>
        /// <param name="list2">list2</param>
        /// <returns></returns>
        public static List<T> GetUnion<T>(this IList<T> list1, IList<T> list2) {
            SortedDictionary<T, int> result = new SortedDictionary<T, int>();
            foreach (T tmp in list1) {
                if (!result.ContainsKey(tmp)) result.Add(tmp, 0);
            }

            foreach (T tmp in list2) {
                if (!result.ContainsKey(tmp)) result.Add(tmp, 0);
            }

            return result.Keys.CopyAllToList<T>().ToList();
        }
        /// <summary>
        /// 多个线程并行执行
        /// </summary>
        /// <param name="tasks">多任务</param>
        public static void InParallel(this List<ThreadStart> tasks) {
            InParallel(tasks, int.MaxValue);
        }
        /// <summary>
        /// 多个线程并行执行
        /// </summary>
        /// <param name="tasks">多任务</param>
        /// <param name="maxThreads">线程数</param>
        public static void InParallel(this List<ThreadStart> tasks, int maxThreads) {
            new ThreadPoolEx().Execute(maxThreads, tasks);
        }
        /// <summary>
        /// 随机打乱顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList<T> Rand<T>(this IList<T> list) { return list.OrderBy(p => Guid.NewGuid()).ToList(); }
        /// <summary>
        /// LIST参数转为URL参数
        /// </summary>
        /// <param name="param">LIST参数</param>
        /// <returns>URL参数</returns>
        public static string ToUrl(this IList<UrlParameter> param) {
            StringBuilder ParameString = new StringBuilder();
            foreach (UrlParameter par in param) ParameString.AppendFormat("{0}={1}&", par.ParameterName, par.ParameterValue);
            ParameString.RemoveLastChar("&");
            return ParameString.ToString();
        }
        /// <summary>
        /// LIST参数转为URL参数
        /// </summary>
        /// <param name="param">LIST参数</param>
        /// <returns>URL参数</returns>
        public static string ToUrlEncode(this IList<UrlParameter> param) {
            StringBuilder ParameString = new StringBuilder();
            foreach (UrlParameter par in param) ParameString.AppendFormat("{0}={1}&", par.ParameterName, par.ParameterValue.UrlEncode());
            ParameString.RemoveLastChar("&");
            return ParameString.ToString();
        }
        /// <summary>
        /// 添加项
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">IList列表</param>
        /// <param name="item">值</param>
        /// <returns>IList列表</returns>
        public static IList<T> Add<T>(this IList<T> list, T item) {
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
        public static IList<T> AddUnique<T>(this IList<T> list, T item) {
            lock (((ICollection)list).SyncRoot) { if (!list.Contains(item)) list.Add(item); }
            return list;
        }
        /// <summary>
        /// 打乱数据顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list) {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
