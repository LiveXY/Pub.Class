//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Web;
using System.Text;

namespace Pub.Class {
    /// <summary>
    /// 输出消息类
    /// 
    /// 修改纪录
    ///     2006.05.08 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public class Msg {
        //#region Write
        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="msg">消息内容</param>
        public static void Write(string msg) { HttpContext.Current.Response.Write(msg); }
        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="args">消息内容</param>
        public static void Write(params object[] args) { HttpContext.Current.Response.Write(string.Concat(args)); }
        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="format">格式化</param>
        /// <param name="args">参数值</param>
        public static void Write(string format, params object[] args) { HttpContext.Current.Response.Output.Write(format, args); }
        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Write(object msg) { Write(msg.ToString()); }
        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="msg">消息</param>
        public static void WriteLine(string msg) { HttpContext.Current.Response.Write(msg + "<br />"); }
        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="msg">消息</param>
        public static void WriteLine(object msg) { WriteLine(msg.ToString()); }
        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="format">格式化</param>
        /// <param name="args">参数值</param>
        public static void WriteLine(string format, params object[] args) { HttpContext.Current.Response.Output.Write(format + "<br />", args); }
        /// <summary>
        /// 输出消息 并结束执行
        /// </summary>
        /// <param name="msg">消息</param>
        public static void WriteEnd(string msg) { Msg.Write(msg); Msg.End(); }
        /// <summary>
        /// 输出消息 并结束执行
        /// </summary>
        /// <param name="format">格式化</param>
        /// <param name="args">参数值</param>
        public static void WriteEnd(string format, params object[] args) { Msg.Write(format, args); Msg.End(); }
        /// <summary>
        /// 输出消息 并结束执行
        /// </summary>
        /// <param name="msg">消息</param>
        public static void WriteEnd(object msg) { WriteEnd(msg.ToString()); }
        /// <summary>
        /// 输出消息到HtmlGenericControl控件
        /// </summary>
        /// <param name="Control">HtmlGenericControl控件</param>
        /// <param name="msg">消息内容</param>
        /// <param name="isAdd">是否累记</param>
        public static void Write(System.Web.UI.HtmlControls.HtmlGenericControl Control, string msg, bool isAdd) {
            if (isAdd) Control.InnerHtml += msg; else Control.InnerHtml = msg;
        }
        /// <summary>
        /// 向页面输出xml内容 并结束
        /// </summary>
        /// <param name="xmlnode">xml内容</param>
        public static void WriteXMLEnd(string xmlnode) {
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ContentType = "text/xml";
            System.Web.HttpContext.Current.Response.Expires = 0;
            System.Web.HttpContext.Current.Response.Cache.SetNoStore();
            System.Web.HttpContext.Current.Response.Write(xmlnode.ToString());
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// 输出json内容 并结束
        /// </summary>
        /// <param name="json">json内容</param>
        public static void WriteJSONEnd(string json) {
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ContentType = "application/json";
            System.Web.HttpContext.Current.Response.Expires = 0;
            System.Web.HttpContext.Current.Response.Cache.SetNoStore();
            System.Web.HttpContext.Current.Response.Write(json);
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// 输出jsonp内容 并结束
        /// </summary>
        /// <param name="json">json内容</param>
        public static void WriteJSONPEnd(string json) {
            string callback = Request2.GetQ("callback");
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ContentType = "application/javascript";
            System.Web.HttpContext.Current.Response.Expires = 0;
            System.Web.HttpContext.Current.Response.Cache.SetNoStore();
            System.Web.HttpContext.Current.Response.Write("{0}({1});".FormatWith(callback, json));
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// 输出html内容 并结束
        /// </summary>
        /// <param name="html">html内容</param>
        public static void WriteHTMLEnd(string html) {
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ContentType = "text/html";
            System.Web.HttpContext.Current.Response.Expires = 0;
            System.Web.HttpContext.Current.Response.Cache.SetNoStore();
            System.Web.HttpContext.Current.Response.Write(html);
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// 输出文本内容 并结束
        /// </summary>
        /// <param name="text">text内容</param>
        public static void WriteTextEnd(string text) {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(text);
            HttpContext.Current.Response.End();
        }
        //#endregion
        //#region End/CompleteRequest
        /// <summary>
        /// 终止页面的运行
        /// </summary>
        public static void End() { HttpContext.Current.Response.End(); }
        /// <summary>
        /// 终止线程的运行
        /// </summary>
        public static void CompleteRequest() { HttpContext.Current.ApplicationInstance.CompleteRequest(); }
        //#endregion
        //#region Redirect/Transfer/NoCache
        /// <summary>
        /// 目录条转
        /// </summary>
        /// <param name="url">url</param>
        public static void Redirect(string url) { HttpContext.Current.Response.Redirect(url); }
        /// <summary>
        /// 服务器端目录条转 
        /// </summary>
        /// <param name="url">url</param>
        public static void Transfer(string url) { HttpContext.Current.Server.Transfer(url); }
        /// <summary>
        /// 不使用页面CACHE
        /// </summary>
        public static void NoCache() {
            System.Web.HttpContext.Current.Response.BufferOutput = false;
            System.Web.HttpContext.Current.Response.ExpiresAbsolute = DateTime.Now.AddDays(-1);
            System.Web.HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            System.Web.HttpContext.Current.Response.Expires = 0;
            System.Web.HttpContext.Current.Response.CacheControl = "no-cache";
            System.Web.HttpContext.Current.Response.Cache.SetNoStore();
        }
        /// <summary>
        /// 分块输出
        /// </summary>
        public static void Flush() {
            HttpContext.Current.Response.Flush();
        }
        //#endregion
    }
}
