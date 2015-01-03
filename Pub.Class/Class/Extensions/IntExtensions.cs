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
    /// int扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class IntExtensions {
        /// <summary>
        /// 整除否
        /// </summary>
        /// <param name="iInt">int扩展</param>
        /// <param name="val">除数</param>
        /// <returns>整除否</returns>
        public static bool IsMod(this int iInt, int val) {
            if ((iInt % val) == 0) return true;
            return false;
        }
        /// <summary>
        /// 奇数的, 单数的
        /// </summary>
        /// <param name="value">int扩展</param>
        /// <returns>奇数的, 单数的</returns>
        public static bool IsOdd(this int value) {
            return ((value & 1) == 1);
        }
        /// <summary>
        /// 被2整除否 偶数
        /// </summary>
        /// <param name="iInt">int扩展</param>
        /// <returns>被2整除否 偶数</returns>
        public static bool IsEven(this int iInt) {
            return iInt.IsMod(2);
        }
        /// <summary>
        /// 素数否
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ToBeChecked">int扩展</param>
        /// <returns>素数否</returns>
        public static bool IsPrime<T>(this int ToBeChecked) {
            System.Collections.BitArray numbers = new System.Collections.BitArray(ToBeChecked + 1, true);
            for (Int32 i = 2; i < ToBeChecked + 1; i++)
                if (numbers[i]) {
                    for (Int32 j = i * 2; j < ToBeChecked + 1; j += i) numbers[j] = false;
                    if (numbers[i]) {
                        if (ToBeChecked == i) return true;
                    }
                }
            return false;
        }
        /// <summary>
        /// 多少年前
        /// </summary>
        /// <param name="years">int扩展</param>
        /// <returns>多少年前</returns>
        public static DateTime YearsAgo(this int years) {
            return DateTime.Now.AddYears(-years);
        }
        /// <summary>
        /// 多少月前
        /// </summary>
        /// <param name="months">int扩展</param>
        /// <returns>多少月前</returns>
        public static DateTime MonthsAgo(this int months) {
            return DateTime.Now.AddMonths(-months);
        }
        /// <summary>
        /// 多少天前
        /// </summary>
        /// <param name="days">int扩展</param>
        /// <returns>多少天前</returns>
        public static DateTime DaysAgo(this int days) {
            return DateTime.Now.AddDays(-days);
        }
        /// <summary>
        /// 多少小时前
        /// </summary>
        /// <param name="hours">int扩展</param>
        /// <returns>多少小时前</returns>
        public static DateTime HoursAgo(this int hours) {
            return DateTime.Now.AddHours(-hours);
        }
        /// <summary>
        /// 多少分钟前
        /// </summary>
        /// <param name="minutes">int扩展</param>
        /// <returns>多少分钟前</returns>
        public static DateTime MinutesAgo(this int minutes) {
            return DateTime.Now.AddMinutes(-minutes);
        }
        /// <summary>
        /// 多少秒前
        /// </summary>
        /// <param name="seconds">int扩展</param>
        /// <returns>多少秒前</returns>
        public static DateTime SecondsAgo(this int seconds) {
            return DateTime.Now.AddSeconds(-seconds);
        }
        /// <summary>
        /// 大于0 TRUE
        /// </summary>
        /// <param name="val">int扩展</param>
        /// <returns>大于0 TRUE</returns>
        public static bool ToBool(this int val) {
            return val > 0 ? true : false;
        }
        /// <summary>
        /// True
        /// </summary>
        /// <param name="iBool">int扩展</param>
        /// <returns></returns>
        public static bool True(this int iBool) { return ToBool(iBool) == true; }
        /// <summary>
        /// False
        /// </summary>
        /// <param name="iBool">int扩展</param>
        /// <returns></returns>
        public static bool False(this int iBool) { return ToBool(iBool) == false; }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">int扩展</param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this int number, int percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position">int扩展</param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this int position, int total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)position / (decimal)total * 100;
            return result;
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position">int扩展</param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this int? position, int total) {
            if (position.IsNull()) return 0;
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">int扩展</param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this int number, float percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position">int扩展</param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this int position, float total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">int扩展</param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this int number, double percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position">int扩展</param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this int position, double total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">int扩展</param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this int number, decimal percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position">int扩展</param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this int position, decimal total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)position / (decimal)total * 100;
            return result;
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">int扩展</param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this int number, long percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position">int扩展</param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this int position, long total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)position / (decimal)total * 100;
            return result;
        }
        /// <summary>
        /// 延时 1000为一秒
        /// </summary>
        /// <param name="ms">int扩展</param>
        /// <returns>延时 1000为一秒</returns>
        public static int Sleep(this int ms) { Thread.Sleep(ms); return ms; }
        /// <summary>
        /// 生成干扰码
        /// </summary>
        /// <param name="len">int扩展</param>
        /// <returns>生成干扰码</returns>
        public static string Jammer(this int len) {
            string randStr = string.Empty;
            for (int i = 0; i < len; i++) {
                randStr += ((char)(Rand.RndInt(0, 33))).ToString() + ((char)(Rand.RndInt(63, 126))).ToString();
            }

            return randStr;
        }
        /// <summary>
        /// IsBetween
        /// </summary>
        /// <param name="i">int扩展</param>
        /// <param name="lowerBound">最小</param>
        /// <param name="upperBound">最大</param>
        /// <returns></returns>
        public static bool IsBetween(this int i, int lowerBound, int upperBound) {
            return i.IsBetween(lowerBound, upperBound, false);
        }
        /// <summary>
        /// IsBetween
        /// </summary>
        /// <param name="i">int扩展</param>
        /// <param name="lowerBound">最小</param>
        /// <param name="upperBound">最大</param>
        /// <param name="includeBounds">是否包含最小最大值</param>
        /// <returns></returns>
        public static bool IsBetween(this int i, int lowerBound, int upperBound, bool includeBounds) {
            if (includeBounds)
                return i >= lowerBound && i <= upperBound;
            else
                return i > lowerBound && i < upperBound;
        }
        /// <summary>
        /// 时间差
        /// </summary>
        /// <param name="integer">int扩展</param>
        /// <returns>时间差</returns>
        public static TimeSpan ToTimeSpan(this int integer) {
            int hours = integer / 100;
            int minutes = integer - hours * 100;
            return new TimeSpan(hours, minutes, 0);
        }
        /// <summary>
        /// 文件大小格式化 最小KB
        /// </summary>
        /// <param name="bytes">int扩展</param>
        /// <returns>文件大小格式化 最小KB</returns>
        public static string FormatKB(this int bytes) { return ((long)bytes).FormatKB(); }
        /// <summary>
        /// 文件大小格式化
        /// </summary>
        /// <param name="bytes">int扩展</param>
        /// <returns>文件大小格式化 最小KB</returns>
        public static string FormatBytes(this int bytes) { return ((long)bytes).FormatBytes(); }
        /// <summary>
        /// 生成 start/end数组
        /// </summary>
        /// <param name="start">int扩展</param>
        /// <param name="end">end</param>
        /// <returns>生成 start/end数组</returns>
        public static IEnumerable<int> Range(this int start, int end) {
            for (int i = start; i <= end; i++)
                yield return i;
        }
        /// <summary>
        /// 8进制
        /// </summary>
        /// <param name="n">int扩展</param>
        /// <returns>8进制</returns>
        public static string ToOctal(this int n) {
            return string.Format("{0:X8}", n);
        }
        /// <summary>
        /// 8进制
        /// </summary>
        /// <param name="n">int扩展</param>
        /// <returns>8进制</returns>
        public static string ToOctal(this long n) {
            return string.Format("{0:X8}", n);
        }
        /// <summary>
        /// 转时间
        /// </summary>
        /// <param name="seconds">int扩展</param>
        /// <returns>转时间 1:12:12</returns>
        public static string ToTime(this int seconds) {
            return ((long)seconds).ToTime();
        }
        /// <summary>
        /// 转Enum
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="number">int扩展</param>
        /// <returns></returns>
        public static T ToEnum<T>(this int number) {
            return (T)Enum.ToObject(typeof(T), number);
        }
        /// <summary>
        /// 金额 三位,分割
        /// </summary>
        /// <param name="value">int扩展</param>
        /// <returns>金额 三位,分割</returns>
        public static string ToCurrency(this int value) {
            return value.ToString("N");
        }
        /// <summary>
        /// 人民币转大写
        /// </summary>
        /// <param name="value">int扩展</param>
        /// <returns>人民币转大写</returns>
        public static string ToRMB(this int value) { return ((decimal)value).ToRMB(); }
        /// <summary>
        /// 如果等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int IfEqual(this int obj, int value, int defaultValue) {
            return obj == value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果不等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int IfNotEqual(this int obj, int value, int defaultValue) {
            return obj != value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果大于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int IfMoreThan(this int obj, int value, int defaultValue) {
            return obj > value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果小于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int IfLessThan(this int obj, int value, int defaultValue) {
            return obj < value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果大于等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int IfMoreThanOrEqual(this int obj, int value, int defaultValue) {
            return obj >= value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果小于等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int IfLessThanOrEqual(this int obj, int value, int defaultValue) {
            return obj <= value ? defaultValue : obj;
        }
    }
}
