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

namespace Pub.Class {
    /// <summary>
    /// double扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class DoubleExtensions {
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this double number, int percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this double number, float percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this double number, double percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this double number, long percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this double position, int total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this double position, float total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this double position, double total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this double position, long total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)((decimal)position / (decimal)total * 100);
            return result;
        }
        /// <summary>
        /// 保留decimalPoints位小数
        /// </summary>
        /// <param name="val">值</param>
        /// <param name="decimalPoints">小数位数</param>
        /// <returns>保留decimalPoints位小数</returns>
        public static double Round(this double val, int decimalPoints) {
            return Math.Round(val, decimalPoints);
        }
        /// <summary>
        /// 保留2位小数
        /// </summary>
        /// <param name="val">值</param>
        /// <returns>保留2位小数</returns>
        public static double Round2(this double val) {
            return Math.Round(val, 2);
        }
        /// <summary>
        /// 保留2位小数
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>保留2位小数</returns>
        public static long Round(this double value) {
            if (value >= 0) return (long)Math.Floor(value);
            return (long)Math.Ceiling(value);
        }
        /// <summary>
        /// 金额 三位,分割
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>金额 三位,分割</returns>
        public static string ToCurrency(this double value) {
            return value.ToString("N");
        }
        /// <summary>
        /// 人民币转大写
        /// </summary>
        /// <param name="value">人民币</param>
        /// <returns>大写人民币</returns>
        public static string ToRMB(this double value) { return ((decimal)value).ToRMB(); }
        /// <summary>
        /// 如果等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static double IfEqual(this double obj, double value, double defaultValue) {
            return obj == value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果不等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static double IfNotEqual(this double obj, double value, double defaultValue) {
            return obj != value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果大于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static double IfMoreThan(this double obj, double value, double defaultValue) {
            return obj > value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果小于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static double IfLessThan(this double obj, double value, double defaultValue) {
            return obj < value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果大于等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static double IfMoreThanOrEqual(this double obj, double value, double defaultValue) {
            return obj >= value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果小于等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static double IfLessThanOrEqual(this double obj, double value, double defaultValue) {
            return obj <= value ? defaultValue : obj;
        }
    }
}
