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
    /// HttpContext扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class HttpContextExtensions {
        /// <summary>
        /// gzip压缩
        /// </summary>
        /// <param name="instance">HttpContext扩展</param>
        public static void Compress(this HttpContext instance) {
            instance.CheckOnNull("instance");
            HttpRequest httpRequest = instance.Request;
            if ((httpRequest.Browser.MajorVersion < 7) && httpRequest.Browser.IsBrowser("IE")) return; //IE7以下版本不支持

            if (instance.IsEncodingAccepted("gzip")) {
                instance.Response.Filter = new GZipStream(instance.Response.Filter, CompressionMode.Compress);
                instance.SetEncoding("gzip");
            } else if (instance.IsEncodingAccepted("deflate")) {
                instance.Response.Filter = new DeflateStream(instance.Response.Filter, CompressionMode.Compress);
                instance.SetEncoding("deflate");
            }
        }
        /// <summary>
        /// 是否支持压缩
        /// </summary>
        /// <param name="instance">HttpContext扩展</param>
        /// <param name="encoding">gzip/deflate压缩</param>
        /// <returns></returns>
        public static bool IsEncodingAccepted(this HttpContext instance, string encoding) {
            if (instance.IsNull()) return false;
            return instance.Request.Headers["Accept-encoding"].IsNotNull() && instance.Request.Headers["Accept-encoding"].Contains(encoding);
        }
        /// <summary>
        /// 重置压缩
        /// </summary>
        /// <param name="instance">HttpContext扩展</param>
        /// <param name="encoding">gzip/deflate压缩</param>
        public static void SetEncoding(this HttpContext instance, string encoding) {
            instance.Response.AppendHeader("Content-encoding", encoding);
        }
        /// <summary>
        /// 缓存
        /// </summary>
        /// <param name="instance">HttpContext扩展</param>
        /// <param name="duration">时间差</param>
        public static void Cache(this HttpContext instance, TimeSpan duration) {
            instance.CheckOnNull("instance");
            HttpCachePolicy cache = instance.Response.Cache;
            cache.SetCacheability(HttpCacheability.Public);
            cache.SetOmitVaryStar(true);
            cache.SetExpires(instance.Timestamp.Add(duration));
            cache.SetMaxAge(duration);
            cache.SetValidUntilExpires(true);
            cache.SetLastModified(instance.Timestamp);
            cache.SetLastModifiedFromFileDependencies();
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        }
        /// <summary>
        /// 缓存
        /// </summary>
        /// <param name="instance">HttpContext扩展</param>
        /// <param name="durationInMinutes">时间</param>
        public static void CacheKD(this HttpContext instance, int durationInMinutes) {
            instance.CheckOnNull("instance");
            TimeSpan duration = TimeSpan.FromMinutes(durationInMinutes);
            HttpCachePolicy cache = instance.Response.Cache;
            cache.SetCacheability(HttpCacheability.Public);
            cache.SetExpires(DateTime.Now.Add(duration));
            cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            cache.SetMaxAge(duration);
        }
        /// <summary>
        /// 取消CACHE
        /// </summary>
        /// <param name="instance">HttpContext扩展</param>
        public static void NoCache(this HttpContext instance) {
            instance.Response.Cache.SetNoServerCaching();
            instance.Response.Cache.SetNoStore();
            instance.Response.Cache.SetMaxAge(TimeSpan.Zero);
            instance.Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            instance.Response.Cache.SetExpires(DateTime.Now.AddYears(-1));
        }
    }
}