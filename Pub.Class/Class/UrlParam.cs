//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Security.Cryptography;

namespace Pub.Class {
    /// <summary>
    /// 参数类
    /// 
    /// 修改纪录
    ///     2006.05.14 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class UrlParam : IComparable {
        private string name;
        private object value;
        /// <summary>
        /// 获取参数名称
        /// </summary>
        public string Name { get { return name; } }
        /// <summary>
        /// 获取参数值
        /// </summary>
        public string Value {
            get {
                if (value is Array) return ConvertArrayToString(value as Array);
                else return value.ToString();
            }
        }
        /// <summary>
        /// 获取参数值
        /// </summary>
        public string EncodedValue {
            get {
                if (value is Array) return HttpUtility.UrlEncode(ConvertArrayToString(value as Array));
                else return HttpUtility.UrlEncode(value.ToString());
            }
        }
        /// <summary>
        /// 构造参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        protected UrlParam(string name, object value) {
            this.name = name;
            this.value = value;
        }
        /// <summary>
        /// 生成字符串
        /// </summary>
        /// <returns>返回字符串的名值对</returns>
        public override string ToString() {
            return string.Format("{0}={1}", Name, Value);
        }
        /// <summary>
        /// 生成encode字符串
        /// </summary>
        /// <returns>生成encode字符串</returns>
        public string ToEncodedString() {
            return string.Format("{0}={1}", Name, EncodedValue);
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>返回参数</returns>
        public static UrlParam Create(string name, object value) {
            return new UrlParam(name, value);
        }
        /// <summary>
        /// 比较参数是否相同
        /// </summary>
        /// <param name="obj">要同当前参数比较的参数</param>
        /// <returns>0相同,非0则不同</returns>
        public int CompareTo(object obj) {
            if (!(obj is UrlParam)) return -1;
            return this.name.CompareTo((obj as UrlParam).name);
        }
        /// <summary>
        /// 将参数数组转换为名值串
        /// </summary>
        /// <param name="a">参数数组</param>
        /// <returns>转换的名值串,名值串之间用逗号分隔</returns>
        private static string ConvertArrayToString(Array a) {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < a.Length; i++) {
                if (i > 0) builder.Append(",");
                builder.Append(a.GetValue(i).ToString());
            }
            return builder.ToString();
        }
        /// <summary>
        /// php time()
        /// </summary>
        /// <returns></returns>
        public static long Time() {
            DateTime timeStamp = new DateTime(1970, 1, 1);
            return (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000;
        }
        /// <summary>
        /// php microtime()
        /// </summary>
        /// <returns></returns>
        private static string MicroTime() {
            long sec = Time();
            int msec = DateTime.UtcNow.Millisecond;
            string strMsec = "0." + msec.ToString().PadRight(8, '0');
            return strMsec + " " + sec.ToString();
        }
        /// <summary>
        /// 将参数绑定到url
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="secret"></param>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string GetUrl(string apiUrl, string secret, string action, UrlParam[] parameters) {
            List<UrlParam> list = new List<UrlParam>(parameters);
            list.Add(UrlParam.Create("time", Time()));
            list.Add(UrlParam.Create("action", action));
            list.Sort();
            StringBuilder values = new StringBuilder();
            foreach (UrlParam param in list) {
                if (!string.IsNullOrEmpty(param.Value)) values.Append(param.ToString());
            }
            values.Append(secret);
            byte[] md5_result = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(values.ToString()));
            StringBuilder sig_builder = new StringBuilder();
            foreach (byte b in md5_result) sig_builder.Append(b.ToString("x2"));
            list.Add(UrlParam.Create("sig", sig_builder.ToString()));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < list.Count; i++) {
                if (i > 0) builder.Append("&");
                builder.Append(list[i].ToEncodedString());
            }
            return string.Format("{0}?{1}", apiUrl, builder.ToString());
        }
    }
}
