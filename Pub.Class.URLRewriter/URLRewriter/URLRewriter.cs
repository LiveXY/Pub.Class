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
using System.Text.RegularExpressions;

namespace Pub.Class {
    /// <summary>
    /// URL重写HttpModule
    /// 
    /// 修改纪录
    ///     2011.07.09 版本：1.0 livexy 创建此类
    ///     
    /// </summary>
    public class URLRewriter : System.Web.IHttpModule {
        /// <summary>
        /// 实现接口的Init方法
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context) {
            context.BeginRequest += new EventHandler(RewriterUrl_BeginRequest);
        }
        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Application_OnError(Object sender, EventArgs e) {

        }
        /// <summary>
        /// 实现接口的Dispose方法
        /// </summary>
        public void Dispose() {

        }
        /// <summary>
        /// 重写Url
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RewriterUrl_BeginRequest(object sender, EventArgs e) {
            HttpContext context = ((HttpApplication)sender).Context;
            string requestPath = context.Request.Path;
            RewriterRules rules = RewriterConfiguration.GetConfig().Rules;

            for (int i = 0; i < rules.Count; i++) {
                string matchUrl = "^" + RewriterHelper.ResolveUrl(context.Request.ApplicationPath, rules[i].Match) + "$";
                Regex re = new Regex(matchUrl, RegexOptions.IgnoreCase);
                //matchUrl.ToFile("~/log.txt".GetMapPath(), Encoding.UTF8, false);
                if (re.IsMatch(requestPath)) {
                    string actionUrl = RewriterHelper.ResolveUrl(context.Request.ApplicationPath, re.Replace(requestPath, rules[i].Action));
                    //actionUrl.ToFile("~/log.txt".GetMapPath(), Encoding.UTF8, false);
                    RewriterHelper.RewriteUrl(context, actionUrl);
                    break;
                }
            }
        }
    }
}
