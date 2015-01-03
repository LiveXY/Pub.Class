//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Net;

namespace Pub.Class {
    /// <summary>
    /// HttpListenerContext 扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class HttpListenerContextExtensions {
        /// <summary>
        /// HttpListenerContext 开启压缩/缓存
        /// </summary>
        /// <param name="context">HttpListenerContext扩展</param>
        /// <param name="allowZip">是否压缩</param>
        /// <param name="buffered">是否BUFFER</param>
        /// <param name="allowCache">是否CACHE</param>
        /// <returns>Stream</returns>
        public static Stream GetResponseStream(this HttpListenerContext context, bool allowZip = true, bool buffered = true, bool allowCache = true) {
            var gzip = (context.Request.Headers["Accept-Encoding"] ?? String.Empty).Contains("gzip");
            var deflate = (context.Request.Headers["Accept-Encoding"] ?? String.Empty).Contains("deflate");

            if (!allowCache) {
                context.Response.AddHeader("Date", DateTime.UtcNow.ToString("R"));
                context.Response.AddHeader("Expires", DateTime.UtcNow.AddHours(-1).ToString("R"));
                context.Response.AddHeader("Cache-Control", "no-cache");
                context.Response.AddHeader("Pragma", "no-cache");
            }

            var stream = context.Response.OutputStream;

            if (allowZip) {
                context.Response.AddHeader("Vary", "Accept-Encoding");
                if (gzip) {
                    stream = new GZipStream(stream, CompressionMode.Compress);
                    context.Response.AddHeader("Content-Encoding", "gzip");
                } else if (deflate) {
                    stream = new DeflateStream(stream, CompressionMode.Compress);
                    context.Response.AddHeader("Content-Encoding", "deflate");
                }
            }

            if (buffered) stream = new BufferedStream(stream);

            return stream;
        }
    }
}