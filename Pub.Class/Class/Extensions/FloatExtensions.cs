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
    /// Float扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class FloatExtensions {
        /// <summary>
        /// 百分率 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="percentOf"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this float value, int percentOf) {
            return (decimal)(value / percentOf * 100);
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="value"></param>
        /// <param name="percentOf"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this float value, float percentOf) {
            return (decimal)(value / percentOf * 100);
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="value"></param>
        /// <param name="percentOf"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this float value, double percentOf) {
            return (decimal)(value / percentOf * 100);
        }
        /// <summary>
        /// 百分率
        /// </summary>
        /// <param name="value"></param>
        /// <param name="percentOf"></param>
        /// <returns></returns>
        public static decimal PercentageOf(this float value, long percentOf) {
            return (decimal)(value / percentOf * 100);
        }
        /// <summary>
        /// 金额 三位,分割
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>金额 三位,分割</returns>
        public static string ToCurrency(this int value) {
            return value.ToString("N");
        }
        /// <summary>
        /// 保留decimalPoints位小数
        /// </summary>
        /// <param name="val">值</param>
        /// <param name="decimalPoints">小数位数</param>
        /// <returns>保留decimalPoints位小数</returns>
        public static float Round(this float val, int decimalPoints) {
            return (float)Math.Round((double)val, decimalPoints);
        }
        /// <summary>
        /// 保留2位小数
        /// </summary>
        /// <param name="val">值</param>
        /// <returns>保留2位小数</returns>
        public static float Round2(this float val) {
            return (float)Math.Round((double)val, 2);
        }
    }
}
