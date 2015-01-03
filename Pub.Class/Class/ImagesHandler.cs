//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml;
using Pub.Class;

namespace Pub.Class {
    /// <summary>
    /// 图片防止盗链
    /// 
    /// 修改纪录
    ///     2011.07.10 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class ImagesHandler : IHttpHandler {
        /// <summary>
        /// ProcessRequest
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context) {
            string url = context.Request.FilePath;
            string refUrl = Request2.GetReferrer().ToLower();
            string host = "http://" + Request2.GetHost().ToLower();
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(refUrl) || refUrl.IndexOf(host) != 0 || url.IndexOf(host) != 0) {
                context.Response.ContentType = "image/JPEG";
                context.Response.WriteFile("/no.jpg");
            }
        }
        /// <summary>
        /// IsReusable
        /// </summary>
        public bool IsReusable {
            get { return true; }
        }
    }
}
