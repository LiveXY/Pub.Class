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
    /// WebService 调用接口
    /// 
    /// 修改纪录
    ///     2011.11.09 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public interface IWebService {
        /// <summary>
        /// WebService调用方法
        /// </summary>
        /// <param name="url">WebService 接口地址</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns>返回字符串</returns>
        string Call(string url, string className, string methodName, Hashtable parms);
        /// <summary>
        /// WebService调用方法
        /// </summary>
        /// <param name="url">WebService 接口地址</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="parms">参数</param>
        /// <returns>返回字符串</returns>
        string Call(string url, string className, string methodName, IList<UrlParameter> parms);
    }
}
