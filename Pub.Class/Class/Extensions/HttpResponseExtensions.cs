//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.IO;
using System.IO.Compression;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
#endif
using System.Web;

namespace Pub.Class {
    /// <summary>
    /// HttpResponse扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class HttpResponseExtensions {
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="response">HttpResponse扩展</param>
        public static void Reload(this HttpResponse response) {
            response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
        }
        /// <summary>
        /// 跳转
        /// </summary>
        /// <param name="response">HttpResponse扩展</param>
        /// <param name="urlFormat">url format</param>
        /// <param name="values">值</param>
        public static void Redirect(this HttpResponse response, string urlFormat, params object[] values) {
            Redirect(response, urlFormat, true, values);
        }
        /// <summary>
        /// 跳转
        /// </summary>
        /// <param name="response">HttpResponse扩展</param>
        /// <param name="urlFormat">url format</param>
        /// <param name="endResponse">是否终止</param>
        /// <param name="values">值</param>
        public static void Redirect(this HttpResponse response, string urlFormat, bool endResponse, params object[] values) {
            var url = string.Format(urlFormat, values);
            response.Redirect(url, endResponse);
        }
        /// <summary>
        /// FileNotFound 错误提示
        /// </summary>
        /// <param name="response">HttpResponse扩展</param>
        public static void SetFileNotFound(this HttpResponse response) {
            SetFileNotFound(response, true);
        }
        /// <summary>
        /// 404 错误提示
        /// </summary>
        /// <param name="response">HttpResponse扩展</param>
        /// <param name="endResponse">是否终止</param>
        public static void SetFileNotFound(this HttpResponse response, bool endResponse) {
            SetStatus(response, 404, "Not Found", endResponse);
        }
        /// <summary>
        /// InternalServer 错误提示
        /// </summary>
        /// <param name="response">HttpResponse扩展</param>
        public static void SetInternalServerError(this HttpResponse response) {
            SetInternalServerError(response, true);
        }
        /// <summary>
        /// 500 错误提示
        /// </summary>
        /// <param name="response">HttpResponse扩展</param>
        /// <param name="endResponse">是否终止</param>
        public static void SetInternalServerError(this HttpResponse response, bool endResponse) {
            SetStatus(response, 500, "Internal Server Error", endResponse);
        }
        /// <summary>
        /// 出错
        /// </summary>
        /// <param name="response">HttpResponse扩展</param>
        /// <param name="code">HTML 状态代码</param>
        /// <param name="description">HTML 状态字符串</param>
        /// <param name="endResponse">是否终止</param>
        public static void SetStatus(this HttpResponse response, int code, string description, bool endResponse) {
            response.StatusCode = code;
            response.StatusDescription = description;

            if (endResponse) response.End();
        }
    }
}