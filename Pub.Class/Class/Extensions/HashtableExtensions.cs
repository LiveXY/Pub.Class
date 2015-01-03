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

namespace Pub.Class {
    /// <summary>
    /// Hashtable 扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class HashtableExtensions {
        /// <summary>
        /// Hashtable数据转URL字符串
        /// </summary>
        /// <param name="parameters">Hashtable</param>
        /// <returns></returns>
        public static string ToUrl(this Hashtable parameters) {
            if (parameters.IsNull() || parameters.Count == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (string k in parameters.Keys) sb.AppendFormat("{0}={1}&", k, parameters[k].ToString());
            sb.RemoveLastChar("&");
            return sb.ToString();
        }
        /// <summary>
        /// Hashtable数据转URL字符串
        /// </summary>
        /// <param name="parameters">Hashtable</param>
        /// <returns></returns>
        public static string ToUrlEncode(this Hashtable parameters) {
            if (parameters.IsNull() || parameters.Count == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (string k in parameters.Keys) sb.AppendFormat("{0}={1}&", k.UrlEncode(), parameters[k].ToString().UrlEncode());
            sb.RemoveLastChar("&");
            return sb.ToString();
        }
        public static DataTable ToDataTable(this Hashtable hashtable) {
            var dataTable = new DataTable(hashtable.GetType().Name);
            dataTable.Columns.Add("Key", typeof(object));
            dataTable.Columns.Add("Value", typeof(object));

            foreach (DictionaryEntry var in hashtable){
                dataTable.Rows.Add(var.Key, var.Value);
            }
            return dataTable;
        }
    }
}
