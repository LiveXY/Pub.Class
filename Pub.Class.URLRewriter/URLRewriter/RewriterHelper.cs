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
    /// URL重写助手
    /// 
    /// 修改纪录
    ///     2011.07.09 版本：1.0 livexy 创建此类
    /// </summary>
    internal class RewriterHelper {
        //#region RewriteUrl
        /// <summary>
        /// URL重写
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sendToUrl"></param>
        internal static void RewriteUrl(HttpContext context, string sendToUrl) {
            string x, y;
            RewriteUrl(context, sendToUrl, out x, out y);
        }
        /// <summary>
        /// URL重写
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sendToUrl"></param>
        /// <param name="sendToUrlLessQString"></param>
        /// <param name="filePath"></param>
        internal static void RewriteUrl(HttpContext context, string sendToUrl, out string sendToUrlLessQString, out string filePath) {
            if (context.Request.QueryString.Count > 0) {
                if (sendToUrl.IndexOf('?') != -1)
                    sendToUrl += "&" + context.Request.QueryString.ToString();
                else
                    sendToUrl += "?" + context.Request.QueryString.ToString();
            }

            string queryString = String.Empty;
            sendToUrlLessQString = sendToUrl;
            if (sendToUrl.IndexOf('?') > 0) {
                sendToUrlLessQString = sendToUrl.Substring(0, sendToUrl.IndexOf('?'));
                queryString = sendToUrl.Substring(sendToUrl.IndexOf('?') + 1);
            }

            filePath = string.Empty;
            filePath = context.Server.MapPath(sendToUrlLessQString);

            context.RewritePath(sendToUrlLessQString, String.Empty, queryString);
        }
        //#endregion
        internal static string ResolveUrl(string appPath, string url) {
            if (url.Length == 0 || url[0] != '~') return url;
            if (url.Length == 1) return appPath;

            if (url[1] == '/' || url[1] == '\\') {
                if (appPath.Length > 1) return appPath + "/" + url.Substring(2); else return "/" + url.Substring(2);
            } else {
                if (appPath.Length > 1) return appPath + "/" + url.Substring(1); else return appPath + url.Substring(1);
            }
        }
    }
}
