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
using System.Text;

namespace Pub.Class {
    /// <summary>
    /// WebService Get方式调用
    /// 
    /// 修改纪录
    ///     2011.11.09 版本：1.0 livexy 创建此类
    /// 
    /// <code>
    /// <example>
    /// Hashtable pas = new Hashtable(); pas["i"] = 100;
    /// new GetWebService().Call("http://www.test.com/default.asmx", "WebService", "test2", pas);
    /// </example>
    /// </code>
    /// </summary>
    public class GetWebService : IWebService {
        /// <summary>
        /// WebService Get方式调用
        /// </summary>
        /// <param name="url">WebService 接口地址</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns>返回字符串</returns>
        public string Call(string url, string className, string methodName, Hashtable parms) {
            return WebService.GetWebService(url, methodName, parms);
        }
        /// <summary>
        /// WebService Get方式调用
        /// </summary>
        /// <param name="url">WebService 接口地址</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns>返回字符串</returns>
        public string Call(string url, string className, string methodName, IList<UrlParameter> parms) {
            return WebService.GetWebService(url, methodName, parms);
        }
    }
}
