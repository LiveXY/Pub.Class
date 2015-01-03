//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2011 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;
using System.Data;

namespace Pub.Class {
    /// <summary>
    /// WebService操作类
    /// 
    /// 修改纪录
    ///     2011.11.09 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    ///     Hashtable pas = new Hashtable(); pas["i"] = 100;
    ///     new WebServiceHelper(WebServiceEnum.get).Call("http://www.test.com/default.asmx", "WebService", "test2", pas)
    ///     new WebServiceHelper(WebServiceEnum.post).Call("http://www.test.com/default.asmx", "WebService", "test2", pas)
    ///     new WebServiceHelper(WebServiceEnum.soap).Call("http://www.test.com/default.asmx", "WebService", "test2", pas)
    ///     new WebServiceHelper(WebServiceEnum.dynamic).Call("http://www.test.com/default.asmx", "WebService", "test2", pas)
    /// </example>
    /// </code>
    /// </summary>
    public class WebServiceHelper : Disposable {
        private WebServiceEnum WebServiceEnum;
        private IWebService WebService = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="WebServiceEnum">WebService 调用类型 Enum string</param>
        public WebServiceHelper(string WebServiceEnum) {
            this.WebServiceEnum = WebServiceEnum.ToEnum<WebServiceEnum>();
            init();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="WebServiceEnum">WebService 调用类型 Enum</param>
        public WebServiceHelper(WebServiceEnum WebServiceEnum) {
            this.WebServiceEnum = WebServiceEnum;
            init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void init() { 
            switch (this.WebServiceEnum) {
                case WebServiceEnum.get: this.WebService = new GetWebService(); break;
                case WebServiceEnum.post: this.WebService = new PostWebService(); break;
                case WebServiceEnum.soap: this.WebService = new SoapWebService(); break;
                case WebServiceEnum.dynamic: this.WebService = new DynamicWebService(); break;
                default: this.WebService = new GetWebService(); break;
            }
        }
        /// <summary>
        /// 用using 自动释放
        /// </summary>
        protected override void InternalDispose() {
            WebService = null;
            base.InternalDispose();
        }
        /// <summary>
        /// WebService调用方法
        /// </summary>
        /// <param name="url">WebService 接口地址</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns>返回字符串</returns>
        public string Call(string url, string className, string methodName, Hashtable parms) {
            return this.WebService.Call(url, className, methodName, parms);
        }
        /// <summary>
        /// WebService调用方法
        /// </summary>
        /// <param name="url">WebService 接口地址</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns>返回字符串</returns>
        public string Call(string url, string className, string methodName, IList<UrlParameter> parms) {
            return this.WebService.Call(url, className, methodName, parms);
        }
    }
}
