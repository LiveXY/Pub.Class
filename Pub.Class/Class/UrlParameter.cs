//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif
using System.Text;

namespace Pub.Class {
    /// <summary>
    /// 参数
    /// 
    /// 修改纪录
    ///     2011.12.01 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// List&lt;UrlParameter> param = new List&lt;UrlParameter>();
    /// param.Add(new UrlParameter("oauth_callback", config.RedirectUrl.UrlEncode2()));
    /// param.Add(new UrlParameter("oauth_consumer_key", config.AppKey));
    /// param.Add(new UrlParameter("oauth_nonce", OAuthCommon.GetGUID32()));
    /// param.Add(new UrlParameter("oauth_signature_method", "HMAC-SHA1"));
    /// param.Add(new UrlParameter("oauth_timestamp", OAuthCommon.GetTimestamp()));
    /// param.Add(new UrlParameter("oauth_version", "1.0"));
    /// param.Add(new UrlParameter("scope", "create_records"));
    /// param.Sort(new UrlParameterCompre());
    /// </example>
    /// </code>
    /// </summary>
    public class UrlParameter {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName;
        /// <summary>
        /// 参数值
        /// </summary>
        public string ParameterValue;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Name">参数名称</param>
        /// <param name="Value">参数值</param>
        public UrlParameter(string Name, string Value) {
            this.ParameterName = Name;
            this.ParameterValue = Value;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Name">参数名称</param>
        /// <param name="Value">参数值</param>
        public UrlParameter(string Name, object Value) {
            this.ParameterName = Name;
            this.ParameterValue = Value.ToString();
        }
        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <returns>返回字符串</returns>
        public override string ToString() {
            return string.Format("{0}={1}", this.ParameterName, this.ParameterValue);
        }
        /// <summary>
        /// 返回Url Encode字符串
        /// </summary>
        /// <returns>返回字符串</returns>
        public string ToEncodeString() {
            return string.Format("{0}={1}", this.ParameterName, this.ParameterValue.UrlEncode());
        }
    }
    /// <summary>
    /// 参数排序
    /// 
    /// 修改纪录
    ///     2011.12.01 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class UrlParameterCompre : IComparer<UrlParameter> {
        /// <summary>
        /// 参数排序
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(UrlParameter x, UrlParameter y) {
            if (x.ParameterName == y.ParameterName) {
                return string.Compare(x.ParameterValue, y.ParameterValue);
            } else {
                return string.Compare(x.ParameterName, y.ParameterName);
            }
        }
    }
}
