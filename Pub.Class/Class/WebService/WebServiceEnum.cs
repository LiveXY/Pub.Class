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
    /// WebService 调用类型 Enum
    /// 
    /// 修改纪录
    ///     2011.11.09 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public enum WebServiceEnum {
        /// <summary>
        /// get
        /// </summary>
        get,
        /// <summary>
        /// post
        /// </summary>
        post,
        /// <summary>
        /// soap
        /// </summary>
        soap,
        /// <summary>
        /// dynamic
        /// </summary>
        dynamic
    }
}
