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
using System.Net;

namespace Pub.Class {
    /// <summary>
    /// long扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class LongExtensions {
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">long扩展</param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this long number, int percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">long扩展</param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this long number, float percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">long扩展</param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this long number, double percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">long扩展</param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this long number, decimal percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">long扩展</param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this long number, long percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position">long扩展</param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this long position, int total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)position / (decimal)total * 100;
            return result;
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position">long扩展</param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this long position, float total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position">long扩展</param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this long position, double total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position">long扩展</param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this long position, decimal total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)position / (decimal)total * 100;
            return result;
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position">long扩展</param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this long position, long total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)position / (decimal)total * 100;
            return result;
        }
        /// <summary>
        /// 文件大小格式化 最小KB
        /// </summary>
        /// <param name="bytes">long扩展</param>
        /// <returns></returns>
        public static string FormatKB(this long bytes) {
            if ((double)bytes <= 999) return bytes.ToString() + " bytes";
            return ThreeNonZeroDigits((double)bytes / 1024) + " KB";
        }
        /// <summary>
        /// 文件大小格式化 最小字节
        /// </summary>
        /// <param name="bytes">long扩展</param>
        /// <returns>文件大小格式化 最小字节 </returns>
        public static string FormatBytes(this long bytes) {
            const double ONE_KB = 1024;
            const double ONE_MB = ONE_KB * 1024;
            const double ONE_GB = ONE_MB * 1024;
            const double ONE_TB = ONE_GB * 1024;
            const double ONE_PB = ONE_TB * 1024;
            const double ONE_EB = ONE_PB * 1024;
            const double ONE_ZB = ONE_EB * 1024;
            const double ONE_YB = ONE_ZB * 1024;

            if ((double)bytes <= 999) return bytes.ToString() + " bytes";
            else if ((double)bytes <= ONE_KB * 999) return ThreeNonZeroDigits((double)bytes / ONE_KB) + " KB";
            else if ((double)bytes <= ONE_MB * 999) return ThreeNonZeroDigits((double)bytes / ONE_MB) + " MB";
            else if ((double)bytes <= ONE_GB * 999) return ThreeNonZeroDigits((double)bytes / ONE_GB) + " GB";
            else if ((double)bytes <= ONE_TB * 999) return ThreeNonZeroDigits((double)bytes / ONE_TB) + " TB";
            else if ((double)bytes <= ONE_PB * 999) return ThreeNonZeroDigits((double)bytes / ONE_PB) + " PB";
            else if ((double)bytes <= ONE_EB * 999) return ThreeNonZeroDigits((double)bytes / ONE_EB) + " EB";
            else if ((double)bytes <= ONE_ZB * 999) return ThreeNonZeroDigits((double)bytes / ONE_ZB) + " ZB";
            else return ThreeNonZeroDigits((double)bytes / ONE_YB) + " YB";
        }
        /// <summary>
        /// 保留2位小数
        /// </summary>
        /// <param name="value">long扩展</param>
        /// <returns>保留2位小数</returns>
        public static string ThreeNonZeroDigits(this double value) {
            if (value >= 100) return ((int)value).ToString();
            else if (value >= 10) return value.ToString("0.0");
            else return value.ToString("0.00");
        }
        /// <summary>
        /// 金额 三位,分割
        /// </summary>
        /// <param name="value">long扩展</param>
        /// <returns>金额 三位,分割</returns>
        public static string ToCurrency(this long value) {
            return value.ToString("N");
        }
        /// <summary>
        /// 人民币转大写
        /// </summary>
        /// <param name="value">long扩展</param>
        /// <returns>人民币转大写</returns>
        public static string ToRMB(this long value) { return ((decimal)value).ToRMB(); }
        /// <summary>
        /// 如果等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static long IfEqual(this long obj, long value, long defaultValue) {
            return obj == value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果不等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static long IfNotEqual(this long obj, long value, long defaultValue) {
            return obj != value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果大于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static long IfMoreThan(this long obj, long value, long defaultValue) {
            return obj > value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果小于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static long IfLessThan(this long obj, long value, long defaultValue) {
            return obj < value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果大于等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static long IfMoreThanOrEqual(this long obj, long value, long defaultValue) {
            return obj >= value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果小于等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static long IfLessThanOrEqual(this long obj, long value, long defaultValue) {
            return obj <= value ? defaultValue : obj;
        }
        /// <summary>
        /// 转时间
        /// </summary>
        /// <param name="seconds">int扩展</param>
        /// <returns>转时间 1:12:12</returns>
        public static string ToTime(this long seconds) {
            var hour = seconds / 3600;
            var minute = (seconds - hour * 3600) / 60;
            seconds = seconds % 60;
            return (hour < 10 ? "0" + hour.ToString() : hour.ToString()) + ":" + (minute < 10 ? "0" + minute.ToString() : minute.ToString()) + ":" + (seconds < 10 ? "0" + seconds.ToString() : seconds.ToString());
        }
        public static string ToIP(this long ip) {
            StringBuilder sb = new StringBuilder();
            var ips = IPAddress.Parse(ip.ToString()).ToString().Split('.');
            for (int i = ips.Length - 1; i >= 0; i--) sb.Append(ips[i]).Append(".");
            return sb.ToString().Trim('.');
        }
    }
}
