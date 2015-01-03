//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Text;
using System.Web;
using System.Collections.Generic;

namespace Pub.Class {
    /// <summary>
    /// Cookie操作
    /// 
    /// 修改纪录
    ///     2012.01.20 版本：1.5 livexy 删除Set2/Get2
    ///     2010.02.11 版本：1.4 livexy 添加清除Cookie
    ///     2009.07.20 版本：1.3 livexy 添加Set2/Get2
    ///     2009.06.01 版本：1.2 livexy 修改对COOKIES数据进行AES加密
    ///     2009.04.20 版本：1.1 livexy 添加对P3P支持
    ///     2006.05.01 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// Cookie2.Set("UserName", "cexo255");
    /// Cookie2.Get("UserName");
    /// Cookie2.SetP3PHeader();
    /// Cookie2.Set("HJ.CurrentUser", "UserName", "cexo255");
    /// Cookie2.Get("HJ.CurrentUser", "UserName");
    /// Cookie2.Clear();
    /// </example>
    /// </code>
    /// </summary>
    public class Cookie2 {
        //#region Set
        /// <summary>
        /// 设置 Cookie P3P Header 解决跨域访问 Cookie 。
        /// </summary>
        public static void SetP3PHeader() {
            HttpContext.Current.Response.AddHeader("P3P", "CP=CURa ADMa DEVa PSAo PSDo OUR BUS UNI PUR INT DEM STA PRE COM NAV OTC NOI DSP COR");
        }
        /// <summary>
        /// 设置 Cookie
        /// </summary>
        /// <param name="key">名称</param>
        /// <param name="value">值</param>
        public static void Set(string key, string value) {
            Set("Pub.Class.Cookies", key, value);
        }
        /// <summary>
        /// 设置 Cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expires">到期日期</param>
        public static void Set(string key, string value, DateTime expires) {
            Set("Pub.Class.Cookies", key, value, expires, null);
        }
        /// <summary>
        /// 设置Cookies值
        /// </summary>
        /// <param name="key">Cookies名称</param>
        /// <param name="value">Cookies名称对应的值</param>
        /// <param name="days">有效期 天数</param>
        public static void Set(string key, string value, int days) { Set("Pub.Class.Cookies", key, value, days > 0 ? DateTime.Now.AddDays(days) : DateTime.MinValue); }
        /// <summary>
        /// 设置 Cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expires">到期日期</param>
        /// <param name="domain">域</param>
        public static void Set(string key, string value, DateTime expires, string domain) {
            Set("Pub.Class.Cookies", key, value, expires, domain);
        }
        /// <summary>
        /// 设置 Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void Set(string name, string key, string value) {
            Set(name, key, value, DateTime.MinValue);
        }
        /// <summary>
        /// 设置 Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="domain">域</param>
        public static void Set(string name, string key, string value, string domain) {
            Set(name, key, value, DateTime.MinValue, domain);
        }
        /// <summary>
        /// 设置 Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expires">到期日期</param>
        public static void Set(string name, string key, string value, DateTime expires) {
            Set(name, key, value, expires, null);
        }
        /// <summary>
        /// 设置Cookies值
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="days">有效期 天数</param>
        public static void Set(string name, string key, string value, int days) { Set(name, key, value, days > 0 ? DateTime.Now.AddDays(days) : DateTime.MinValue); }
        /// <summary>
        /// 设置 Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expires">到期日期</param>
        /// <param name="domain">域</param>
        public static void Set(string name, string key, string value, DateTime expires, string domain) {
            Set(name, key, value, expires, domain, null);
        }
        /// <summary>
        /// 设置 Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expires">到期日期</param>
        /// <param name="domain">域</param>
        /// <param name="path">虚拟路径</param>
        /// <param name="httpOnly">是否可通过客户端脚本访问</param>
        public static void Set(string name, string key, string value, DateTime expires, string domain, string path, bool httpOnly) {
            Set(name, key, value, expires, domain, path, httpOnly, false);
        }
        /// <summary>
        /// 设置 Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expires">到期日期</param>
        /// <param name="domain">域</param>
        /// <param name="path">虚拟路径</param>
        public static void Set(string name, string key, string value, DateTime expires, string domain, string path) {
            Set(name, key, value, expires, domain, path, false);
        }
        /// <summary>
        /// 设置 Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expires">到期日期</param>
        /// <param name="domain">域</param>
        /// <param name="path">虚拟路径</param>
        /// <param name="httpOnly">是否可通过客户端脚本访问</param>
        /// <param name="secure">是否使用安全套接字层 (SSL)（即仅通过 HTTPS）传输 Cookie</param>
        public static void Set(string name, string key, string value, DateTime expires, string domain, string path, bool httpOnly, bool secure) {
            string _key = "9cf8d21d394a8919d2f9706dfdc6421e";
            string encryptName = (_key + name).MD5();
            string encryptKey = (_key + key).MD5();
            string encryptValue = value.AESEncode(_key);
            HttpCookie cookie = HttpContext.Current.Request.Cookies[encryptName];

            if (cookie.IsNull()) cookie = new HttpCookie(encryptName);
            cookie.Values[encryptKey] = encryptValue;
            if (expires > DateTime.MinValue) cookie.Expires = expires;
            if (!string.IsNullOrEmpty(domain)) cookie.Domain = domain;
            if (!string.IsNullOrEmpty(path)) cookie.Path = path;
            cookie.HttpOnly = httpOnly;
            cookie.Secure = secure;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        //#endregion
        //#region Get
        /// <summary>
        /// 取Cookies值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string Get(string key) {
            return Get("Pub.Class.Cookies", key);
        }
        /// <summary>
        /// 取Cookies值
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string Get(string name, string key) {
            string _key = "9cf8d21d394a8919d2f9706dfdc6421e";
            string encryptName = (_key + name).MD5();
            string encryptKey = (_key + key).MD5();
            string decryptValue = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[encryptName];

            if (cookie.IsNotNull()) {
                if (!string.IsNullOrEmpty(cookie.Values[encryptKey])) {
                    decryptValue = cookie.Values[encryptKey].ToString().AESDecode(_key);
                }
            }

            return decryptValue.IsNullEmpty() ? string.Empty : decryptValue;
        }
        //#endregion
        //#region Clear
        /// <summary>
        /// 清除Cookies
        /// </summary>
        public static void Clear() {
            IList<string> cookies = new List<string>();
            foreach (string name in HttpContext.Current.Request.Cookies) {
                cookies.Add(name);
            }
            foreach (string name in cookies) {
                HttpCookie cookie = new HttpCookie(name);
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.AppendCookie(cookie);
            }
        }
        //#endregion
    }
}
