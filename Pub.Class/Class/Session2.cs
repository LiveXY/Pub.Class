//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Pub.Class {
    /// <summary>
    /// Session操作类
    /// 
    /// 修改纪录
    ///     2006.05.11 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Session2 {
        //#region Set
        /// <summary>
        /// 设置Session值
        /// </summary>
        /// <param name="key">Session名称</param>
        /// <param name="value">Session名称对应的值</param>
        public static void Set(string key, string value) {
            string _key = "9cf8d21d394a8919d2f9706dfdc6421e";
            key = (_key + key).MD5();
            value = value.DESEncode(_key);
            HttpContext.Current.Session[key] = value;
            if (value.Length == 0) HttpContext.Current.Session.Remove(key);
        }
        //#endregion
        //#region Get
        /// <summary>
        /// 取Session值
        /// </summary>
        /// <param name="key">Session名称</param>
        /// <returns>Session名称对应的值</returns>
        public static String Get(string key) {
            string _Value = string.Empty;
            string _key = "9cf8d21d394a8919d2f9706dfdc6421e";
            key = (_key + key).MD5();
            if (HttpContext.Current.Session[key].IsNotNull()) { _Value = Convert.ToString(HttpContext.Current.Session[key]); _Value = _Value.DESDecode(_key); }
            return _Value;
        }
        //#endregion
    }
}
