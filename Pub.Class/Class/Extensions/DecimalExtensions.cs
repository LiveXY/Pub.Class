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
using System.Text.RegularExpressions;

namespace Pub.Class {
    /// <summary>
    /// Decimal扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class DecimalExtensions {
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">值</param>
        /// <param name="percent">百分之</param>
        /// <returns>百分率</returns>
        public static decimal PercentageOf(this decimal number, int percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">值</param>
        /// <param name="percent">百分之</param>
        /// <returns>百分率</returns>
        public static decimal PercentageOf(this decimal number, decimal percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="number">值</param>
        /// <param name="percent">百分之</param>
        /// <returns>百分率</returns>
        public static decimal PercentageOf(this decimal number, long percent) {
            return (decimal)(number * percent / 100);
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="total">总数</param>
        /// <returns>百分之</returns>
        public static decimal PercentOf(this decimal position, int total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)position / (decimal)total * 100;
            return result;
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this decimal position, decimal total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)position / (decimal)total * 100;
            return result;
        }
        /// <summary>
        /// 百分之
        /// </summary>
        /// <param name="position"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static decimal PercentOf(this decimal position, long total) {
            decimal result = 0;
            if (position > 0 && total > 0) result = (decimal)position / (decimal)total * 100;
            return result;
        }
        /// <summary>
        /// 保留decimalPoints位小数
        /// </summary>
        /// <param name="val">值</param>
        /// <param name="decimalPoints">小数位数</param>
        /// <returns>保留decimalPoints位小数</returns>
        public static decimal Round(this decimal val, int decimalPoints) {
            return Math.Round(val, decimalPoints);
        }
        /// <summary>
        /// 保留2位小数
        /// </summary>
        /// <param name="val">值</param>
        /// <returns>保留2位小数</returns>
        public static decimal Round2(this decimal val) {
            return Math.Round(val, 2);
        }
        /// <summary>
        /// 金额 三位,分割
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>金额 三位,分割</returns>
        public static string ToCurrency(this decimal value) {
            return value.ToString("N");
        }
        /// <summary>
        /// 人民币转大写
        /// </summary>
        /// <param name="num">人民币</param>
        /// <returns>大写人民币</returns>
        public static string ToRMB2(this decimal num) {
            string str = "零壹贰叁肆伍陆柒捌玖";
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分";
            string str3 = "";
            string str4 = "";
            string str5 = "";
            string str6 = "";
            string str7 = "";
            int num4 = 0;
            num = Math.Round(Math.Abs(num), 2);
            str4 = ((long)(num * 100M)).ToString();
            int length = str4.Length;
            if (length > 15) {
                return "溢出";
            }
            str2 = str2.Substring(15 - length);
            for (int i = 0; i < length; i++) {
                str3 = str4.Substring(i, 1);
                int startIndex = Convert.ToInt32(str3);
                if (((i != (length - 3)) && (i != (length - 7))) && ((i != (length - 11)) && (i != (length - 15)))) {
                    if (str3 == "0") {
                        str6 = "";
                        str7 = "";
                        num4++;
                    } else if ((str3 != "0") && (num4 != 0)) {
                        str6 = "零" + str.Substring(startIndex, 1);
                        str7 = str2.Substring(i, 1);
                        num4 = 0;
                    } else {
                        str6 = str.Substring(startIndex, 1);
                        str7 = str2.Substring(i, 1);
                        num4 = 0;
                    }
                } else if ((str3 != "0") && (num4 != 0)) {
                    str6 = "零" + str.Substring(startIndex, 1);
                    str7 = str2.Substring(i, 1);
                    num4 = 0;
                } else if ((str3 != "0") && (num4 == 0)) {
                    str6 = str.Substring(startIndex, 1);
                    str7 = str2.Substring(i, 1);
                    num4 = 0;
                } else if ((str3 == "0") && (num4 >= 3)) {
                    str6 = "";
                    str7 = "";
                    num4++;
                } else if (length >= 11) {
                    str6 = "";
                    num4++;
                } else {
                    str6 = "";
                    str7 = str2.Substring(i, 1);
                    num4++;
                }
                if ((i == (length - 11)) || (i == (length - 3))) {
                    str7 = str2.Substring(i, 1);
                }
                str5 = str5 + str6 + str7;
                if ((i == (length - 1)) && (str3 == "0")) {
                    str5 = str5 + '整';
                }
            }
            if (num == 0M) {
                str5 = "零元整";
            }
            return str5;
        }
        /// <summary>
        /// 把数字转换大写人民币
        /// </summary>
        /// <param name="input">人民币</param>
        /// <returns>大写人民币</returns>
        public static string ToRMB(this decimal input) {
            string s = input.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            string result = Regex.Replace(d, ".", delegate(Match m) { return "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟萬億兆京垓秭穰"[m.Value[0] - '-'].ToString(); });
            return result;
        }
        /// <summary>
        /// 如果等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal IfEqual(this decimal obj, decimal value, decimal defaultValue) {
            return obj == value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果不等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal IfNotEqual(this decimal obj, decimal value, decimal defaultValue) {
            return obj != value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果大于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal IfMoreThan(this decimal obj, decimal value, decimal defaultValue) {
            return obj > value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果小于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal IfLessThan(this decimal obj, decimal value, decimal defaultValue) {
            return obj < value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果大于等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal IfMoreThanOrEqual(this decimal obj, decimal value, decimal defaultValue) {
            return obj >= value ? defaultValue : obj;
        }
        /// <summary>
        /// 如果小于等于
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="value">目标数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal IfLessThanOrEqual(this decimal obj, decimal value, decimal defaultValue) {
            return obj <= value ? defaultValue : obj;
        }
    }
}
