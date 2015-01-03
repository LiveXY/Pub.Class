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
    /// 登录授权 Enum
    /// 
    /// 修改纪录
    ///     2011.11.17 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public enum OAuthEnum {
        /// <summary>
        /// sina
        /// </summary>
        sina,
        /// <summary>
        /// msn
        /// </summary>
        msn,
        /// <summary>
        /// qq
        /// </summary>
        qq,
        /// <summary>
        /// netease
        /// </summary>
        netease,
        /// <summary>
        /// sohu
        /// </summary>
        sohu,
        /// <summary>
        /// kaixin
        /// </summary>
        kaixin,
        /// <summary>
        /// renren
        /// </summary>
        renren
    }
}
