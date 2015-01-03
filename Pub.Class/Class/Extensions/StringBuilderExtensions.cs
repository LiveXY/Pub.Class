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

namespace Pub.Class {
    /// <summary>
    /// StringBuilder 扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class StringBuilderExtensions {
        /// <summary>
        /// 是否空或null
        /// </summary>
        /// <param name="sb">StringBuilder扩展</param>
        /// <returns>true/false</returns>
        public static bool IsNullEmpty(this StringBuilder sb) {
            if (sb.IsNull()) return true;
            if (sb.Length == 0) return true;
            return false;
        }
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sb">StringBuilder扩展</param>
        public static void Clear(this StringBuilder sb) {
            sb.Remove(0, sb.Length);
        }
        /// <summary>
        /// 删除最后N个字符
        /// </summary>
        /// <param name="sb">StringBuilder扩展</param>
        /// <param name="lastchar">最后一个字符串</param>
        public static void RemoveLastChar(this StringBuilder sb, string lastchar) {
            if (sb.IsNull() || sb.Length == 0 || lastchar.IsNull()) return;
            int len = lastchar.Length;
            if (sb.ToString().EndsWith(lastchar)) sb.Remove(sb.Length - len, len);
            //string str = sb.ToString();
            //int length = str.LastIndexOf(lastchar);
            //if (length > 0) { sb.Clear(); sb.Append(str.Substring(0, length)); }
        }
        /// <summary>
        /// 删除最后1个字符
        /// </summary>
        /// <param name="sb">StringBuilder扩展</param>
        public static void RemoveLastChar(this StringBuilder sb) {
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
        }
        /// <summary>
        /// 删除最后N个字符
        /// </summary>
        /// <param name="sb">StringBuilder扩展</param>
        /// <param name="len">长度</param>
        public static void RemoveLastChar(this StringBuilder sb, int len) {
            if (sb.Length >= len) sb.Remove(sb.Length - len, len);
        }
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sb">StringBuilder扩展</param>
        public static StringBuilder ClearAll(this StringBuilder sb) {
            sb.Remove(0, sb.Length);
            return sb;
        }
        /// <summary>
        /// 删除最后N个字符
        /// </summary>
        /// <param name="sb">StringBuilder扩展</param>
        /// <param name="lastchar">最后一个字符串</param>
        public static StringBuilder RemoveLast(this StringBuilder sb, string lastchar) {
            if (sb.IsNull() || sb.Length == 0 || lastchar.IsNull()) return sb;
            int len = lastchar.Length;
            if (sb.ToString().EndsWith(lastchar)) sb.Remove(sb.Length - len, len);
            return sb;
            //string str = sb.ToString();
            //int length = str.LastIndexOf(lastchar);
            //if (length > 0) { sb.Clear(); sb.Append(str.Substring(0, length)); }
        }
        /// <summary>
        /// 删除最后1个字符
        /// </summary>
        /// <param name="sb">StringBuilder扩展</param>
        public static StringBuilder RemoveLast(this StringBuilder sb) {
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb;
        }
        /// <summary>
        /// 删除最后N个字符
        /// </summary>
        /// <param name="sb">StringBuilder扩展</param>
        /// <param name="len">长度</param>
        public static StringBuilder RemoveLast(this StringBuilder sb, int len) {
            if (sb.Length >= len) sb.Remove(sb.Length - len, len);
            return sb;
        }
        /// <summary>
        /// GoogleSitemap
        /// </summary>
        /// <param name="code">StringBuilder扩展</param>
        /// <param name="loc">网址</param>
        /// <param name="changefreq">更改的频率</param>
        /// <param name="priority">记录优先级</param>
        /// <returns></returns>
        public static StringBuilder GetGoogleSitemap(this StringBuilder code, string loc, string changefreq, string priority) {
            code.Append("	<url>" + Environment.NewLine);
            code.Append("		<loc>" + loc + "</loc>" + Environment.NewLine);
            code.Append("		<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>" + Environment.NewLine);
            code.Append("		<changefreq>" + changefreq + "</changefreq>" + Environment.NewLine);
            code.Append("		<priority>" + priority + "</priority>" + Environment.NewLine);
            code.Append("	</url>" + Environment.NewLine);
            return code;
        }
        /// <summary>
        /// Google Sitemap
        /// </summary>
        /// <param name="code">StringBuilder扩展</param>
        /// <param name="loc">网址</param>
        /// <returns></returns>
        public static StringBuilder GetGoogleSitemap(this StringBuilder code, string loc) {
            code.Append("	<url>" + Environment.NewLine);
            code.Append("		<loc>" + loc + "</loc>" + Environment.NewLine);
            code.Append("		<lastmod>" + DateTime.Now.ToString("yyyy-MM-dd") + "</lastmod>" + Environment.NewLine);
            code.Append("		<changefreq>daily</changefreq>" + Environment.NewLine);
            code.Append("		<priority>0.6</priority>" + Environment.NewLine);
            code.Append("	</url>" + Environment.NewLine);
            return code;
        }
        /// <summary>
        /// Baidu Sitemap
        /// </summary>
        /// <param name="code">StringBuilder扩展</param>
        /// <param name="title">标题</param>
        /// <param name="link">链接</param>
        /// <param name="description">描述</param>
        /// <param name="text">内容</param>
        /// <param name="image">图片</param>
        /// <param name="keywords">关键字</param>
        /// <param name="category">分类</param>
        /// <param name="author">作者</param>
        /// <param name="source">源地址</param>
        /// <param name="pubDate">发布时间</param>
        /// <returns></returns>
        public static StringBuilder GetBaiduSitemap(this StringBuilder code, string title, string link, string description, string text, string image, string keywords, string category, string author, string source, string pubDate) {
            code.Append("	<item>" + Environment.NewLine);
            code.Append("		<title>" + title.ShowXmlHtml() + "</title>" + Environment.NewLine);
            code.Append("		<link>" + link.ShowXmlHtml() + "</link>" + Environment.NewLine);
            if (!description.Equals("")) code.Append("		<description>" + description.ShowXmlHtml() + "</description>" + Environment.NewLine);
            if (!text.Equals("")) code.Append("		<text>" + text.ShowXmlHtml() + "</text>" + Environment.NewLine);
            if (!image.Equals("")) code.Append("		<image>" + image.ShowXmlHtml() + "</image>" + Environment.NewLine);
            if (!keywords.Equals("")) code.Append("		<keywords>" + keywords.ShowXmlHtml() + "</keywords>" + Environment.NewLine);
            if (!category.Equals("")) code.Append("		<category>" + category.ShowXmlHtml() + "</category>" + Environment.NewLine);
            if (!author.Equals("")) code.Append("		<author>" + author.ShowXmlHtml() + "</author>" + Environment.NewLine);
            if (!source.Equals("")) code.Append("		<source>" + source.ShowXmlHtml() + "</source>" + Environment.NewLine);
            if (!pubDate.Equals("")) code.Append("		<pubDate>" + pubDate + "</pubDate>" + Environment.NewLine);
            code.Append("	</item>" + Environment.NewLine);
            return code;
        }
    }
}
