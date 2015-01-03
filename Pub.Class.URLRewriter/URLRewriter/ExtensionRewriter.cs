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
    public class ExtensionRewriter : System.Web.IHttpModule {
        /// <summary>
        /// 实现接口的Init方法
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context) {
            if (RewriterConfiguration.RewriteExt == ".aspx") return;

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
            string ext = requestPath.GetExtension().ToLower();
            if (ext != RewriterConfiguration.RewriteExt) return;

            string matchUrl = RewriterHelper.ResolveUrl(context.Request.ApplicationPath, requestPath);
            string actionUrl = matchUrl.ChangeExtension(".aspx");
            //("1|" + matchUrl + "|" + actionUrl).ToFile("e:\\log.txt", Encoding.UTF8, false);
            RewriterHelper.RewriteUrl(context, actionUrl);
        }
    }
}
