//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Data.Common;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Pub.Class;
using System.Web;
using System.Configuration;
using System.Xml.Serialization;

namespace Pub.Class {
    /// <summary>
    /// URLRewriter配置
    /// 
    /// 修改纪录
    ///     2012.03.07 版本：1.0 livexy 修改添加SafeDictionary cache
    ///     2011.07.09 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    [Serializable()]
    [XmlRoot("URLRewriter")]
    public class RewriterConfiguration {
        /// <summary>
        /// 重写扩展名
        /// </summary>
        public static readonly string RewriteExt = WebConfig.GetApp("RewriteExt").ToLower();
#if NET20
        private static readonly ISafeDictionary<string, RewriterConfiguration> cache = new SafeDictionary<string, RewriterConfiguration>();
#else
        private static readonly ISafeDictionary<string, RewriterConfiguration> cache = new SafeDictionarySlim<string, RewriterConfiguration>();
#endif
        private RewriterRules rules;
        /// <summary>
        /// rules 节点
        /// </summary>
        public RewriterRules Rules { get { return rules; } set { rules = value; } }
        /// <summary>
        /// CACHE web.config rewrite节点
        /// </summary>
        /// <returns></returns>
        public static RewriterConfiguration GetConfig() {
            string key = "URLRewriter";
            return cache.Get(key, () => { return (RewriterConfiguration)ConfigurationManager.GetSection(key); });

            //if (cache.ContainsKey(key)) return cache[key];
            //RewriterConfiguration conn = (RewriterConfiguration)ConfigurationManager.GetSection(key);
            //cache[key] = conn;
            //return conn;
        }
    }
}
